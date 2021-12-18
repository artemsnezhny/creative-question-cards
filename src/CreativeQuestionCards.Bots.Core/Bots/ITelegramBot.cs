namespace CreativeQuestionCards.Bots.Core.Bots
{
    public interface ITelegramBot
    {
        void StartReceiving(CancellationToken cancellationToken);
    }
}
