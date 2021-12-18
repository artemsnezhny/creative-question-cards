namespace CreativeQuestionCards.Bots.Core.Entities
{
    public sealed record UserEntity(string UserName, HashSet<int> UsedQuestions);
}
