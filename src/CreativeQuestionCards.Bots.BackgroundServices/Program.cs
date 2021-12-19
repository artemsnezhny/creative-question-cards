using CreativeQuestionCards.Bots.BackgroundServices.Extensions;
using CreativeQuestionCards.Bots.DataAccess.Extensions;
using CreativeQuestionCards.Bots.Infrastructure.Telegram.Extensions;
using CreativeQuestionCards.Bots.Infrastructure.Telegram.Settings;

using Serilog;

var builder = Host.CreateDefaultBuilder(args)
    .UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

builder.ConfigureServices((context, services) =>
    {
        services.AddOptions<TelegramSettings>()
            .Bind(context.Configuration.GetSection(TelegramSettings.SectionKey))
            .ValidateOnStart();

        services.AddBackgroundServices();
        services.AddTelegramInfrastructureServices();
        services.AddDataAccessServices();
    });

var app = builder.Build();

await app.RunAsync();