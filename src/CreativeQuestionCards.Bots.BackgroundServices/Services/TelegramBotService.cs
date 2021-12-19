namespace CreativeQuestionCards.Bots.BackgroundServices.Services
{
    using CreativeQuestionCards.Bots.Core.Bots;

    internal sealed class TelegramBotService : BackgroundService
    {
        private readonly ILogger<TelegramBotService> logger;

        private readonly ITelegramBot telegramBot;

        public TelegramBotService(
            ILogger<TelegramBotService> logger,
            ITelegramBot telegramBot)
        {
            this.logger = logger;
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
                this.logger.LogError($"TelegramBotService failed to start: {e}");
                throw;
            }

            return Task.CompletedTask;
        }
    }
}
