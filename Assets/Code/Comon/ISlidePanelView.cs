public interface ISlidePanelView: IView
{
    SliderState CurrentState { get; }
    void Open(SliderState startState = SliderState.Middle, bool needIgnoreMiddlePosition = false);
    void Close();
}

public interface ICancelOrderView : ISlidePanelView
{ }
