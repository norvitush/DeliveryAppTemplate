using System;

public class OrderDetailesWindowPresenter : WindowPresenter
{
    private IOrderDetailesView View => (IOrderDetailesView)_view;
    private IOrdersDataProvider Model => (IOrdersDataProvider)_model;

    public OrderDetailesWindowPresenter(IOrderDetailesView view, IOrdersDataProvider model, IWindowsDirector windowsDirector) : base(view, model, windowsDirector) { }

    protected override void Init()
    {

    }

    public void OnClickBack()
    {
        _windowsDirector.GoPreviousWindow();
    }

    public void OnClickGetOrder()
    {
        View.SetActive(false);
        _windowsDirector.OpenWindow(WindowType.GetOrderByQR, asRootWindow: false, OnCloseGetQR);
    }

    internal void OnClickCancelOrder()
    {
        _windowsDirector.OpenSlider(SliderPanelType.CancelOrder, () => { });
    }

    private void OnCloseGetQR()
    {
        View.SetActive(true);
    }


    protected override void OnDisable()
    {

    }

    protected override void OnEnable()
    {
    }
}
