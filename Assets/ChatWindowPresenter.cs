public class ChatWindowPresenter : WindowPresenter
{
    private IChatWindowView View => (IChatWindowView)_view;
    private IChatDataProvider Model => (IChatDataProvider)_model;

    public ChatWindowPresenter(IChatWindowView view, IChatDataProvider model, IWindowsDirector windowsDirector) : base(view, model, windowsDirector) { }

    protected override void Init()
    {

    }

    protected override void OnDisable()
    {

    }

    protected override void OnEnable()
    {
    }
}
