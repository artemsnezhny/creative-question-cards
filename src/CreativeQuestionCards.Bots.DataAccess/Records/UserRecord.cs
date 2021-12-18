#pragma warning disable CS8618
namespace CreativeQuestionCards.Bots.DataAccess.Records
{
    using LiteDB;

    internal sealed class UserRecord
    {
        public UserRecord(string userName, HashSet<int> usedQuestions)
        {
            this.UserName = userName;
            this.UsedQuestions = usedQuestions;
        }

        private UserRecord()
        {
        }

        public ObjectId Id { get; set; }

        public string UserName { get; set; }

        public HashSet<int> UsedQuestions { get; set; }
    }
}
