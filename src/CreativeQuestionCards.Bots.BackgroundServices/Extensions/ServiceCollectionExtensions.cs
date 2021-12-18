namespace CreativeQuestionCards.Bots.BackgroundServices.Extensions
{
    using CreativeQuestionCards.Bots.BackgroundServices.Providers;
    using CreativeQuestionCards.Bots.BackgroundServices.Services;
    using CreativeQuestionCards.Bots.Core.Providers;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBackgroundServices(this IServiceCollection services)
        {
            services.AddHostedService<TelegramBotService>();

            services.AddSingleton<IQuestionsProvider, QuestionsProvider>();

            return services;
        }
    }
}
