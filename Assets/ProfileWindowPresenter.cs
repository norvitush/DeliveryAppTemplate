using System;

public class ProfileWindowPresenter: WindowPresenter
{
    private IProfileWindowView View => (IProfileWindowView)_view;
    private IProfileDataProvider Model => (IProfileDataProvider)_model;

    public ProfileWindowPresenter(IProfileWindowView view, IProfileDataProvider model, IWindowsDirector windowsDirector) : base(view, model, windowsDirector) { }

    protected override void Init()
    {

    }

    protected override void OnDisable()
    {

    }

    protected override void OnEnable()
    {
    }

    internal void OpenNotifications()
    {
        View.SetActive(false);
        _windowsDirector.OpenWindow(WindowType.Notifications, asRootWindow: false, OnComeBackFromNotifications);
    }

    private void OnComeBackFromNotifications()
    {
        View.SetActive(true);
    }
}
