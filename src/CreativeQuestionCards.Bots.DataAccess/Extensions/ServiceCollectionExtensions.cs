namespace CreativeQuestionCards.Bots.DataAccess.Extensions
{
    using CreativeQuestionCards.Bots.Core.DataAccess;
    using CreativeQuestionCards.Bots.DataAccess.DataAccess;
    using CreativeQuestionCards.Bots.DataAccess.Settings;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDataAccessServices(this IServiceCollection services)
        {
            services.AddSingleton<ILiteDatabaseContext>(sp =>
            {
                return new LiteDatabaseContext(sp.GetRequiredService<IOptions<LiteDatabaseSettings>>().Value.Location!);
            });

            services.AddSingleton<IUsersDataAccess, UsersDataAccess>();

            return services;
        }
    }
}
