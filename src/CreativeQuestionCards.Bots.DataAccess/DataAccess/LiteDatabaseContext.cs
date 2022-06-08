namespace CreativeQuestionCards.Bots.DataAccess.DataAccess
{
    using LiteDB;

    internal sealed class LiteDatabaseContext : ILiteDatabaseContext
    {
        public LiteDatabaseContext(string fileName)
        {
            this.Database = new LiteDatabase($"Filename={fileName};connection=shared");
        }

        public LiteDatabase Database { get; }
    }
}
