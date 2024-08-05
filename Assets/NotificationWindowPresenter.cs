using System;

public class NotificationWindowPresenter : WindowPresenter
{
    private INotificationsView View => (INotificationsView)_view;
    private INotificationsDataProvider Model => (INotificationsDataProvider)_model;

    public NotificationWindowPresenter(INotificationsView view, INotificationsDataProvider model, IWindowsDirector windowsDirector) : base(view, model, windowsDirector) { }

    protected override void Init() { IsInited = true; }

    protected override void OnDisable() { }

    protected override void OnEnable() { }

    internal void OnClickBack()
    {
        if (!IsActive) return;
        IsActive = false;
        _windowsDirector.GoPreviousWindow();
    }
}
