using UnityEngine;

public class BootstrapState : State
{
    private readonly RectTransform _uiContainer;
    private readonly SceneInstancesLinkerBase _sceneObjectsLinker;
    private readonly AppGeneralSettingsSO _settingsSO;

    private IModel _appData;
    private IWindowsDirector _windowDirector;

    private IServiceProvider AllServices => (IServiceProvider)_allServices;

    public BootstrapState(IServiceProvider     allServices,
                          IGameStateMachine    stateMachine,
                          SceneInstancesLinkerBase uiLinker,
                          AppGeneralSettingsSO settingsSO) : base(allServices, stateMachine)
    {
        _sceneObjectsLinker = uiLinker;
        _windowDirector     = uiLinker.WindowsDirector;
        _uiContainer        = (RectTransform)_windowDirector.gameObject.transform;
        _settingsSO         = settingsSO;

    }

    public override void Enter()
    {
        RegisterServices();    
        Leave();
    }

    private void RegisterServices()
    {
        RegisterSettings();
        RegisterAppData();
        RegisterUIServices();        
        RegisterUIFactory();       
    }

    private void RegisterSettings()
    {
        AllServices.Register<AppGeneralSettingsSO>(() => { return  _settingsSO; });
    }   
    private void RegisterAppData()
    {
        //get or init App model
        _appData = ServiceLocator.Instance.Resolve<IDataKeeper>();
        if (_appData == null)
        {
            _appData = new AppDataKeeper(_settingsSO);
            AllServices.Register<IModel>(() => { return _appData; });
        }
        else
        {
            Debug.Log($"_appData: {_appData} {(_appData as AppDataKeeper).DefaultMapPoint}");            
        }

    }
    private void RegisterUIServices()
    {
        AllServices.Register<SceneInstancesLinkerBase>(() => { return _sceneObjectsLinker; });
        AllServices.Register<IWindowsDirector>(()=> { return _windowDirector; });
    }
    private void RegisterUIFactory()
    {
        var factory = new WIndowsMVPFactory(_appData, _settingsSO, _uiContainer, _sceneObjectsLinker);
        AllServices.Register<IMVPFactory>(() => { return factory; });
    }

    public override void Leave()
    {
        _stateMachine.Enter<AppLoopState>();
    }
}
