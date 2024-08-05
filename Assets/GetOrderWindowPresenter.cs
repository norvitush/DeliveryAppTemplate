using System;

public class GetOrderWindowPresenter : WindowPresenter
{
    private IGetOrderView View => (IGetOrderView)_view;
    private IOrdersDataProvider Model => (IOrdersDataProvider)_model;

    public GetOrderWindowPresenter(IGetOrderView view, IOrdersDataProvider model, IWindowsDirector windowsDirector) : base(view, model, windowsDirector) { }

    protected override void Init()
    {
        
    }

    protected override void OnDisable()
    {
        
    }

    protected override void OnEnable()
    {
        
    }

    public void OnClickBack()
    {
        _windowsDirector.GoPreviousWindow();
    }
}
