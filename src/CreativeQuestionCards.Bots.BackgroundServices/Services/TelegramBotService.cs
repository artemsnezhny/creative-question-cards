namespace CreativeQuestionCards.Bots.BackgroundServices.Services
{
    using CreativeQuestionCards.Bots.Core.Bots;

    internal sealed class TelegramBotService : BackgroundService
    {
        private readonly ITelegramBot telegramBot;

        public TelegramBotService(ITelegramBot telegramBot)
        {
            this.telegramBot = telegramBot;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                this.telegramBot.StartReceiving(stoppingToken);
            }
            catch (Exception e)
            {
                // TODO: log exception
                throw;
            }

            return Task.CompletedTask;
        }
    }
}
