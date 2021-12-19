namespace CreativeQuestionCards.Bots.Core.DataAccess
{
    using CreativeQuestionCards.Bots.Core.Entities;

    public interface IUsersDataAccess
    {
        UserEntity? FindUser(string userName);

        void AddUsedQuestion(string userName, int questionId);

        void ResetUsedQuestions(string userName);

        bool IsAdminUser(string userName);

        void AddUser(string userName);

        void DeleteUser(string userName);

        void AddAdminUser(string userName);

        void DeleteAdminUser(string userName);
    }
}
