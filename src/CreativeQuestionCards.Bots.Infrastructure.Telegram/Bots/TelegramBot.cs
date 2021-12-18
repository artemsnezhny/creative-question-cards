namespace CreativeQuestionCards.Bots.Infrastructure.Telegram.Bots
{
    using CreativeQuestionCards.Bots.Core.Bots;
    using CreativeQuestionCards.Bots.Core.DataAccess;
    using CreativeQuestionCards.Bots.Core.Providers;

    using global::Telegram.Bot;
    using global::Telegram.Bot.Extensions.Polling;
    using global::Telegram.Bot.Types;
    using global::Telegram.Bot.Types.Enums;

    internal sealed class TelegramBot : ITelegramBot
    {
        private readonly ITelegramBotClient telegramBotClient;

        private readonly IQuestionsProvider questionsProvider;

        private readonly IUsersDataAccess usersDataAccess;

        public TelegramBot(
            ITelegramBotClient telegramBotClient,
            IQuestionsProvider questionsProvider,
            IUsersDataAccess usersDataAccess)
        {
            this.telegramBotClient = telegramBotClient;
            this.questionsProvider = questionsProvider;
            this.usersDataAccess = usersDataAccess;
        }

        public void StartReceiving(CancellationToken cancellationToken)
        {
            this.telegramBotClient.StartReceiving(
                this.HandleUpdateAsync,
                this.HandleErrorAsync,
                new ReceiverOptions { AllowedUpdates = { } },
                cancellationToken: cancellationToken);
        }

        private async Task HandleUpdateAsync(
            ITelegramBotClient botClient,
            Update update,
            CancellationToken cancellationToken)
        {
            if (update.Type != UpdateType.Message)
            {
                return;
            }

            if (update.Message!.Type != MessageType.Text)
            {
                return;
            }

            if (string.IsNullOrEmpty(update.Message.From?.Username))
            {
                await botClient.SendTextMessageAsync(
                    update.Message.Chat.Id,
                    "Упс! Тебе нужно заполнить username в Telegram",
                    cancellationToken: cancellationToken);
                return;
            }

            var user = this.usersDataAccess.FindUser(update.Message.From.Username);

            if (user == null)
            {
                // TODO: message with purchase instruction
                await botClient.SendTextMessageAsync(
                    update.Message.Chat.Id,
                    "Упс! Похоже мы ещё не знакомы 😞",
                    cancellationToken: cancellationToken);
                return;
            }

            var question = this.questionsProvider.FindRandomQuestion(user.UsedQuestions);

            if (question == null)
            {
                await botClient.SendTextMessageAsync(
                    update.Message.Chat.Id,
                    "Воу! Все вопросы разобраны, начинаем сначала 🎉",
                    cancellationToken: cancellationToken);

                this.usersDataAccess.ResetUsedQuestions(user.UserName);
                question = this.questionsProvider.FindRandomQuestion();

                if (question == null)
                {
                    return;
                }
            }

            this.usersDataAccess.AddUsedQuestion(user.UserName, question.Value.Key);

            await botClient.SendTextMessageAsync(
                update.Message.Chat.Id,
                question.Value.Value,
                cancellationToken: cancellationToken);
        }

        private Task HandleErrorAsync(
            ITelegramBotClient botClient,
            Exception exception,
            CancellationToken cancellationToken)
        {
            // TODO: Log exception
            return Task.CompletedTask;
        }
    }
}
