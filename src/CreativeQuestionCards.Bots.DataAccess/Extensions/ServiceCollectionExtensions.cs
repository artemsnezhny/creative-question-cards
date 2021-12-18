namespace CreativeQuestionCards.Bots.DataAccess.Extensions
{
    using CreativeQuestionCards.Bots.Core.DataAccess;
    using CreativeQuestionCards.Bots.DataAccess.DataAccess;

    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDataAccessServices(this IServiceCollection services)
        {
            services.AddSingleton<ILiteDatabaseContext>(new LiteDatabaseContext("database.db"));

            services.AddSingleton<IUsersDataAccess, UsersDataAccess>();

            return services;
        }
    }
}
