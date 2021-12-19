namespace CreativeQuestionCards.Bots.Infrastructure.Telegram.Bots
{
    using CreativeQuestionCards.Bots.Core.Bots;
    using CreativeQuestionCards.Bots.Core.DataAccess;
    using CreativeQuestionCards.Bots.Core.Entities;
    using CreativeQuestionCards.Bots.Core.Providers;

    using global::Telegram.Bot;
    using global::Telegram.Bot.Extensions.Polling;
    using global::Telegram.Bot.Types;
    using global::Telegram.Bot.Types.Enums;
    using global::Telegram.Bot.Types.ReplyMarkups;

    using Microsoft.Extensions.Logging;

    internal sealed class TelegramBot : ITelegramBot
    {
        private readonly ILogger<TelegramBot> logger;

        private readonly ITelegramBotClient telegramBotClient;

        private readonly IQuestionsProvider questionsProvider;

        private readonly IUsersDataAccess usersDataAccess;

        public TelegramBot(
            ILogger<TelegramBot> logger,
            ITelegramBotClient telegramBotClient,
            IQuestionsProvider questionsProvider,
            IUsersDataAccess usersDataAccess)
        {
            this.logger = logger;
            this.telegramBotClient = telegramBotClient;
            this.questionsProvider = questionsProvider;
            this.usersDataAccess = usersDataAccess;
        }

        public void StartReceiving(CancellationToken cancellationToken)
        {
            this.telegramBotClient.StartReceiving(
                this.HandleUpdateAsync,
                this.HandleErrorAsync,
                new ReceiverOptions { AllowedUpdates = new[] { UpdateType.Message, UpdateType.CallbackQuery } },
                cancellationToken: cancellationToken);
        }

        private async Task HandleUpdateAsync(
            ITelegramBotClient botClient,
            Update update,
            CancellationToken cancellationToken)
        {
            try
            {
                await this.HandleUpdateAsyncInternal(botClient, update, cancellationToken);
            }
            catch (Exception e)
            {
                await this.HandleErrorAsync(botClient, e, cancellationToken);
            }
        }

        private async Task HandleUpdateAsyncInternal(
            ITelegramBotClient botClient,
            Update update,
            CancellationToken cancellationToken)
        {
            if (update.Type == UpdateType.CallbackQuery)
            {
                var user = this.FindUser(update.CallbackQuery!.From!.Username!);

                if (user == null)
                {
                    await this.ProcessUserNotFound(update.CallbackQuery.Message!.Chat.Id, botClient, cancellationToken);
                    return;
                }

                if (update.CallbackQuery.Data == "/newquestion")
                {
                    await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, string.Empty);

                    await this.SendQuestion(user, update.CallbackQuery.Message!.Chat.Id, botClient, cancellationToken);
                    return;
                }

                return;
            }

            if (update.Type == UpdateType.Message)
            {
                if (update.Message!.Text == "/help")
                {
                    await botClient.SendTextMessageAsync(
                        update.Message.Chat.Id,
                        $"Если у тебя появились вопросы или что-то не работает, смело пиши в директ",
                        replyMarkup: new InlineKeyboardMarkup(
                            new[]
                            {
                                InlineKeyboardButton.WithUrl("meriva_stilllife", "https://www.instagram.com/meriva_stilllife/"),
                                InlineKeyboardButton.WithUrl("limonnaya", "https://www.instagram.com/limonnaya/"),
                            }),
                        cancellationToken: cancellationToken);

                    if (update.Message.From?.Username != null && this.usersDataAccess.IsAdminUser(update.Message.From.Username))
                    {
                        await botClient.SendTextMessageAsync(
                            update.Message.Chat.Id,
                            $"Секретные команды администратора:{Environment.NewLine}/adduser \\- дать доступ пользователю{Environment.NewLine}/deleteuser \\- отозвать доступ",
                            parseMode: ParseMode.MarkdownV2,
                            cancellationToken: cancellationToken);
                    }

                    return;
                }

                if (string.IsNullOrEmpty(update.Message.From?.Username))
                {
                    await botClient.SendTextMessageAsync(
                        update.Message.Chat.Id,
                        "Упс! Тебе нужно заполнить username в Telegram 🧐",
                        cancellationToken: cancellationToken);
                    return;
                }

                var user = this.FindUser(update.Message!.From!.Username!);

                if (user == null)
                {
                    await this.ProcessUserNotFound(update.Message.Chat.Id, botClient, cancellationToken);
                    return;
                }

                if (update.Message.Type != MessageType.Text)
                {
                    await botClient.SendTextMessageAsync(
                        update.Message.Chat.Id,
                        "Упс! Я бы рад поболтать, но пока умею только предлагать вопросы 🙌",
                        replyMarkup: new InlineKeyboardMarkup(
                            InlineKeyboardButton.WithCallbackData("Хочу вопрос!", "/newquestion")),
                        cancellationToken: cancellationToken);
                    return;
                }

                if (update.Message.Text == "/start")
                {
                    this.usersDataAccess.ResetUsedQuestions(user.UserName);

                    await botClient.SendTextMessageAsync(
                        update.Message.Chat.Id,
                        $"Привет! Мы — Маша и Крис 🖤{Environment.NewLine}У нас для тебя здесь спрятано 100 вопросов.{Environment.NewLine}{Environment.NewLine}✨ Можешь задавать их другу, любимому, родителям, врагам или самому себе — интересно будет в любой компании;{Environment.NewLine}✨ Хочешь, пройдись по всем за один вечер, хочешь, подели на два выходных, или вовсе отвечай на один вопрос раз в неделю;{Environment.NewLine}✨ Советуем предварительно налить себе бокал молока, чая с лимоном или чего-то, что тебя радует больше, зажечь свечи или гирлянду и приготовиться удивляться, задумываться и улыбаться.{Environment.NewLine}{Environment.NewLine}Правило лишь одно: воспринимай этого бота, как толчок к обсуждению. Нам бы очень хотелось, чтобы большинство вопросов приводили к дискуссиям, возможно, небольшим спорам и волшебным открытиям о человеке, с которым ты играешь. А ещё почти каждый вопрос — это отличная тема для поста, пользуйся этим! Пожалуйста, не руби односложные ответы в попытке скорее тыкнуть кнопку — дай шанс теме раскрыться.{Environment.NewLine}{Environment.NewLine}А ещё люби, пиши, фотографируй и будь собой 🍋🥛",
                        replyMarkup: new InlineKeyboardMarkup(
                            InlineKeyboardButton.WithCallbackData("Хочу вопрос!", "/newquestion")),
                        cancellationToken: cancellationToken);

                    return;
                }

                if (update.Message.Text == "/newquestion")
                {
                    await this.SendQuestion(user, update.Message.Chat.Id, botClient, cancellationToken);

                    return;
                }

                if (update.Message.Text == "/adduser" && this.usersDataAccess.IsAdminUser(update.Message.From.Username))
                {
                    await botClient.SendTextMessageAsync(
                        update.Message.Chat.Id,
                        "Напиши telegram username пользователя, которого нужно добавить",
                        replyMarkup: new ForceReplyMarkup() { Selective = true },
                        cancellationToken: cancellationToken);

                    return;
                }

                if (update.Message.ReplyToMessage != null
                    && update.Message.ReplyToMessage.Text == "Напиши telegram username пользователя, которого нужно добавить"
                    && this.usersDataAccess.IsAdminUser(update.Message.From.Username))
                {
                    var me = await botClient.GetMeAsync();

                    if (update.Message.ReplyToMessage.From?.Username != me.Username)
                    {
                        return;
                    }

                    var userToAdd = update.Message.Text!.Replace("@", string.Empty).Trim();
                    this.usersDataAccess.AddUser(userToAdd);

                    await botClient.SendTextMessageAsync(
                        update.Message.Chat.Id,
                        $"Пользователь @{userToAdd} успешно добавлен",
                        cancellationToken: cancellationToken);

                    return;
                }

                if (update.Message.Text == "/deleteuser" && this.usersDataAccess.IsAdminUser(update.Message.From.Username))
                {
                    await botClient.SendTextMessageAsync(
                        update.Message.Chat.Id,
                        "Напиши telegram username пользователя, которого нужно удалить",
                        replyMarkup: new ForceReplyMarkup() { Selective = true },
                        cancellationToken: cancellationToken);

                    return;
                }

                if (update.Message.ReplyToMessage != null
                    && update.Message.ReplyToMessage.Text == "Напиши telegram username пользователя, которого нужно удалить"
                    && this.usersDataAccess.IsAdminUser(update.Message.From.Username))
                {
                    var me = await botClient.GetMeAsync();

                    if (update.Message.ReplyToMessage.From?.Username != me.Username)
                    {
                        return;
                    }

                    var userToDelete = update.Message.Text!.Replace("@", string.Empty).Trim();
                    this.usersDataAccess.DeleteUser(userToDelete);

                    await botClient.SendTextMessageAsync(
                        update.Message.Chat.Id,
                        $"Пользователь @{userToDelete} успешно удалён",
                        cancellationToken: cancellationToken);

                    return;
                }

                if (update.Message.Text == "/addadminuser" && this.usersDataAccess.IsAdminUser(update.Message.From.Username))
                {
                    await botClient.SendTextMessageAsync(
                        update.Message.Chat.Id,
                        "Добавить админа",
                        replyMarkup: new ForceReplyMarkup() { Selective = true },
                        cancellationToken: cancellationToken);

                    return;
                }

                if (update.Message.ReplyToMessage != null
                    && update.Message.ReplyToMessage.Text == "Добавить админа"
                    && this.usersDataAccess.IsAdminUser(update.Message.From.Username))
                {
                    var me = await botClient.GetMeAsync();

                    if (update.Message.ReplyToMessage.From?.Username != me.Username)
                    {
                        return;
                    }

                    var userToAdd = update.Message.Text!.Replace("@", string.Empty).Trim();
                    this.usersDataAccess.AddAdminUser(userToAdd);

                    await botClient.SendTextMessageAsync(
                        update.Message.Chat.Id,
                        $"Пользователь @{userToAdd} успешно добавлен",
                        cancellationToken: cancellationToken);

                    return;
                }

                if (update.Message.Text == "/deleteadminuser"
                    && this.usersDataAccess.IsAdminUser(update.Message.From.Username))
                {
                    await botClient.SendTextMessageAsync(
                        update.Message.Chat.Id,
                        "Удалить админа",
                        replyMarkup: new ForceReplyMarkup() { Selective = true },
                        cancellationToken: cancellationToken);

                    return;
                }

                if (update.Message.ReplyToMessage != null
                    && update.Message.ReplyToMessage.Text == "Удалить админа"
                    && this.usersDataAccess.IsAdminUser(update.Message.From.Username))
                {
                    var me = await botClient.GetMeAsync();

                    if (update.Message.ReplyToMessage.From?.Username != me.Username)
                    {
                        return;
                    }

                    var userToDelete = update.Message.Text!.Replace("@", string.Empty).Trim();
                    this.usersDataAccess.DeleteAdminUser(userToDelete);

                    await botClient.SendTextMessageAsync(
                        update.Message.Chat.Id,
                        $"Пользователь @{userToDelete} успешно удалён",
                        cancellationToken: cancellationToken);

                    return;
                }
            }
        }

        private Task HandleErrorAsync(
            ITelegramBotClient botClient,
            Exception exception,
            CancellationToken cancellationToken)
        {
            this.logger.LogError($"Telegram bot error: {exception}");
            return Task.CompletedTask;
        }

        private UserEntity? FindUser(string userName)
        {
            return this.usersDataAccess.FindUser(userName);
        }

        private async Task ProcessUserNotFound(
            long chatId,
            ITelegramBotClient botClient,
            CancellationToken cancellationToken)
        {
            await botClient.SendTextMessageAsync(
                chatId,
                $"Упс, похоже, мы ещё не знакомы. Пиши скорее нам в директ, чтобы получить доступ 🙌{Environment.NewLine}А после снова жми /start",
                replyMarkup: new InlineKeyboardMarkup(
                    new[]
                    {
                        InlineKeyboardButton.WithUrl("meriva_stilllife", "https://www.instagram.com/meriva_stilllife/"),
                        InlineKeyboardButton.WithUrl("limonnaya", "https://www.instagram.com/limonnaya/"),
                    }),
                cancellationToken: cancellationToken);
        }

        private async Task SendQuestion(
            UserEntity user,
            long chatId,
            ITelegramBotClient botClient,
            CancellationToken cancellationToken)
        {
            var question = this.questionsProvider.FindRandomQuestion(user.UsedQuestions);

            if (question == null)
            {
                await botClient.SendTextMessageAsync(
                    chatId,
                    $"Поздравляем, ты прошёл игру и осилил все 100 вопросов, ура! Надеемся, ты узнал о себе что-то новое 🖤 Подари себе в честь этого что-нибудь приятное 💌{Environment.NewLine}{Environment.NewLine}Спасибо, что был с нами ✨",
                    replyMarkup: new InlineKeyboardMarkup(
                    new[]
                    {
                        InlineKeyboardButton.WithUrl("meriva_stilllife", "https://www.instagram.com/meriva_stilllife/"),
                        InlineKeyboardButton.WithUrl("limonnaya", "https://www.instagram.com/limonnaya/"),
                    }),
                    cancellationToken: cancellationToken);

                return;
            }

            this.usersDataAccess.AddUsedQuestion(user.UserName, question.Value.Key);

            await botClient.SendTextMessageAsync(
                chatId,
                question.Value.Value,
                replyMarkup: new InlineKeyboardMarkup(
                    InlineKeyboardButton.WithCallbackData("Ещё вопрос!", "/newquestion")),
                cancellationToken: cancellationToken);
        }
    }
}
