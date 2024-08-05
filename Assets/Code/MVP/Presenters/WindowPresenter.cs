using System;

public abstract class WindowPresenter
{
    
    protected readonly IWindowsDirector _windowsDirector;
    protected readonly IModel _model;
    protected readonly IView _view;

    public bool IsInited { get; protected set; }
    public bool IsActive { get; protected set; }
    public IView GetView() => _view;

    protected WindowPresenter(IView view, IModel model, IWindowsDirector windowsDirector)
    {
        _windowsDirector = windowsDirector;
        _model = model;
        _view = view;
    }

    public virtual void SetActive(bool value)
    {
        if (value && IsInited == false)
            Init();

        if (value)
        {            
            OnEnable();
        }
        else
            OnDisable();

        _view.SetActive(value);
        IsActive = value;
    }

    protected abstract void Init();    
    protected abstract void OnEnable();    
    protected abstract void OnDisable();

    public virtual void LogicUpdate() { }

    public virtual void OnFirstViewUpdate() { }

}
