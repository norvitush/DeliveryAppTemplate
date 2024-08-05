using System;

public class CancelOrderPresenter : SliderPresenter
{
    private ICancelOrderView View => (ICancelOrderView)_view;

    public CancelOrderPresenter(ICancelOrderView view, IOrdersDataProvider model,  IWindowsDirector windowsDirector) : base(view, model, windowsDirector)
    {
    }

}



public class SliderPresenter : WindowPresenter
{
    private ISlidePanelView View => (ISlidePanelView)_view;

    public SliderPresenter(ISlidePanelView view, IModel model, IWindowsDirector windowsDirector) : base(view, model, windowsDirector)
    {
    }

    public override void SetActive(bool value)
    {
        if (value && IsInited == false)
            Init();

        if (value)
        {
            OnEnable();
        }
        else
        {
            IsActive = false;
            View.Close();
        }
    }

    protected override void Init()
    {
        IsInited = true;
    }

    protected override void OnEnable()
    {
        //_view.SetActive(value);
        IsActive = true;
        View.Open();
    }

    protected override void OnDisable()
    {
        if (!IsActive) return;
        IsActive = false;
        _windowsDirector.GoPreviousWindow();
    }

    public void OnViewDisable() => OnDisable();
}
