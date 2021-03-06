namespace CreativeQuestionCards.Bots.BackgroundServices.Providers
{
    using System.Reflection;
    using System.Text.Json;

    using CreativeQuestionCards.Bots.Core.Providers;

    internal sealed class QuestionsProvider : IQuestionsProvider
    {
        private static readonly Dictionary<int, string> Questions;

        static QuestionsProvider()
        {
            var basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
            var filePath = Path.Combine(basePath, "Questions", "NewYearQuestions.json");
            var json = File.ReadAllText(filePath);
            Questions = JsonSerializer.Deserialize<Dictionary<int, string>>(json)
                ?? new Dictionary<int, string>();
        }

        public IReadOnlyDictionary<int, string> GetQuestions()
        {
            return Questions;
        }

        public KeyValuePair<int, string>? FindRandomQuestion(HashSet<int>? excludeIds)
        {
            var availableKeys = excludeIds == null
                ? Questions.Keys.ToArray()
                : Questions.Keys.Where(x => !excludeIds.Contains(x)).ToArray();

            if (!availableKeys.Any())
            {
                return null;
            }

            var key = new Random().Next(availableKeys.Length);

            return new KeyValuePair<int, string>(availableKeys[key], Questions[availableKeys[key]]);
        }
    }
}
