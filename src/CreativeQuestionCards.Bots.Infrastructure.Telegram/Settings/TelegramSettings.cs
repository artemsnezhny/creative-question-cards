namespace CreativeQuestionCards.Bots.Infrastructure.Telegram.Settings
{
    using System.ComponentModel.DataAnnotations;

    public sealed record TelegramSettings
    {
        public const string SectionKey = "Telegram";

        [Required]
        public string? AccessToken { get; init; }
    }
}
