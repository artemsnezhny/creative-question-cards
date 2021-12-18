namespace CreativeQuestionCards.Bots.Infrastructure.Telegram.Extensions
{
    using CreativeQuestionCards.Bots.Core.Bots;
    using CreativeQuestionCards.Bots.Infrastructure.Telegram.Bots;
    using CreativeQuestionCards.Bots.Infrastructure.Telegram.Settings;

    using global::Telegram.Bot;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTelegramInfrastructureServices(this IServiceCollection services)
        {
            services.AddSingleton<ITelegramBotClient>(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<TelegramSettings>>().Value;
                return new TelegramBotClient(settings.AccessToken!);
            });

            services.AddSingleton<ITelegramBot, TelegramBot>();

            return services;
        }
    }
}
