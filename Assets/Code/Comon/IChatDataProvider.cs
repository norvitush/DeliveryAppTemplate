public interface IChatDataProvider : IModel
{

    public int NewMessages { get; }

    void SetMessages(object messages, int newMessageCount, bool withNotify = true);
}
