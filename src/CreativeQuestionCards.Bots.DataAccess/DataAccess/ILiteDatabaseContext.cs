namespace CreativeQuestionCards.Bots.DataAccess.DataAccess
{
    using LiteDB;

    internal interface ILiteDatabaseContext
    {
        public LiteDatabase Database { get; }
    }
}
