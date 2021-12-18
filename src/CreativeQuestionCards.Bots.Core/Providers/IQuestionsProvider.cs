namespace CreativeQuestionCards.Bots.Core.Providers
{
    public interface IQuestionsProvider
    {
        IReadOnlyDictionary<int, string> GetQuestions();

        KeyValuePair<int, string>? FindRandomQuestion(HashSet<int>? excludeIds = null);
    }
}
