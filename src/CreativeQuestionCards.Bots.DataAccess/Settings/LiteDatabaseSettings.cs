namespace CreativeQuestionCards.Bots.DataAccess.Settings
{
    using System.ComponentModel.DataAnnotations;

    public sealed record LiteDatabaseSettings
    {
        public const string SectionKey = "LiteDatabase";

        [Required]
        public string? Location { get; init; }
    }
}
