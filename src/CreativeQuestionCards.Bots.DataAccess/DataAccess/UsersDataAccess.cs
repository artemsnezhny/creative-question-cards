namespace CreativeQuestionCards.Bots.DataAccess.DataAccess
{
    using CreativeQuestionCards.Bots.Core.DataAccess;
    using CreativeQuestionCards.Bots.Core.Entities;
    using CreativeQuestionCards.Bots.DataAccess.Records;

    using LiteDB;

    internal sealed class UsersDataAccess : IUsersDataAccess
    {
        private readonly LiteDatabase liteDatabase;

        public UsersDataAccess(ILiteDatabaseContext liteDatabaseContext)
        {
            this.liteDatabase = liteDatabaseContext.Database;
            this.liteDatabase.GetCollection<UserRecord>().EnsureIndex(x => x.UserName);
            this.liteDatabase.GetCollection<AdminUserRecord>().EnsureIndex(x => x.UserName);
        }

        public UserEntity? FindUser(string userName)
        {
            var userRecord = this.FindUserRecord(userName);

            return userRecord != null
                ? new UserEntity(userRecord.UserName, userRecord.UsedQuestions)
                : null;
        }

        public void AddUser(string userName)
        {
            this.liteDatabase.GetCollection<UserRecord>()
                .Insert(new UserRecord(userName, new HashSet<int>()));
        }

        public void DeleteUser(string userName)
        {
            var userRecord = this.FindUserRecord(userName);

            if (userRecord == null)
            {
                return;
            }

            this.liteDatabase.GetCollection<UserRecord>()
                .Delete(userRecord.Id);
        }

        public void AddAdminUser(string userName)
        {
            this.liteDatabase.GetCollection<AdminUserRecord>()
                .Insert(new AdminUserRecord(userName));
        }

        public void DeleteAdminUser(string userName)
        {
            var userRecord = this.FindAdminUserRecord(userName);

            if (userRecord == null)
            {
                return;
            }

            this.liteDatabase.GetCollection<AdminUserRecord>()
                .Delete(userRecord.Id);
        }

        public void AddUsedQuestion(string userName, int questionId)
        {
            var userRecord = this.FindUserRecord(userName);

            if (userRecord == null)
            {
                throw new InvalidOperationException($"User '{userName}' not found");
            }

            userRecord.UsedQuestions.Add(questionId);

            this.liteDatabase.GetCollection<UserRecord>().Update(userRecord);
        }

        public void ResetUsedQuestions(string userName)
        {
            var userRecord = this.FindUserRecord(userName);

            if (userRecord == null)
            {
                return;
            }

            userRecord.UsedQuestions = new HashSet<int>();
            this.liteDatabase.GetCollection<UserRecord>().Update(userRecord);
        }

        public bool IsAdminUser(string userName)
        {
            return this.liteDatabase.GetCollection<AdminUserRecord>()
                .Exists(x => x.UserName == userName);
        }

        private UserRecord? FindUserRecord(string userName)
        {
            return this.liteDatabase.GetCollection<UserRecord>()
                .FindOne(x => x.UserName == userName);
        }

        private AdminUserRecord? FindAdminUserRecord(string userName)
        {
            return this.liteDatabase.GetCollection<AdminUserRecord>()
                .FindOne(x => x.UserName == userName);
        }
    }
}
