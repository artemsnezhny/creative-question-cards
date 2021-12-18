namespace CreativeQuestionCards.Bots.DataAccess.DataAccess
{
    using System.Reflection;

    using LiteDB;

    internal sealed class LiteDatabaseContext : ILiteDatabaseContext
    {
        public LiteDatabaseContext(string fileName)
        {
            var basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
            this.Database = new LiteDatabase(Path.Combine(basePath, fileName));
        }

        public LiteDatabase Database { get; }
    }
}
