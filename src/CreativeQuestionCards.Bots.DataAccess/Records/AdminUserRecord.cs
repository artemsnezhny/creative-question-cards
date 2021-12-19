#pragma warning disable CS8618
namespace CreativeQuestionCards.Bots.DataAccess.Records
{
    using LiteDB;

    internal sealed class AdminUserRecord
    {
        public AdminUserRecord(string userName)
        {
            this.UserName = userName;
        }

        private AdminUserRecord()
        {
        }

        public ObjectId Id { get; set; }

        public string UserName { get; set; }
    }
}
