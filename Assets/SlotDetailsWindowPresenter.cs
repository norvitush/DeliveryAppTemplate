using System;

public class SlotDetailsWindowPresenter : WindowPresenter
{
    private ISlotDetailsView View => (ISlotDetailsView)_view;
    private ISlotDetailsDataProvider Model => (ISlotDetailsDataProvider)_model;

    public SlotDetailsWindowPresenter(ISlotDetailsView view, ISlotDetailsDataProvider model, IWindowsDirector windowsDirector) : base(view, model, windowsDirector) { }


    protected override void Init()
    {
        //throw new System.NotImplementedException();
    }

    protected override void OnDisable()
    {
        //throw new System.NotImplementedException();
    }

    internal void OnClickBack()
    {
        if (!IsActive) return;
        IsActive = false;

        _windowsDirector.GoPreviousWindow();
    }

    protected override void OnEnable()
    {
        //throw new System.NotImplementedException();
    }
}
