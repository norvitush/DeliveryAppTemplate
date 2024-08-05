using System;
using System.Collections;
using UnityEngine;

public class AppLoopState : State
{
    private MonoBehaviour _coroutineStarter;
    public AppLoopState(IServiceProvider allServices,
                          IGameStateMachine stateMachine, MonoBehaviour coroutineStarter) : base(allServices, stateMachine) {
        _coroutineStarter = coroutineStarter;
    }

    public override void Enter()
    {
        //when all services ready:
        InitServices();        
    }

    private void InitServices()
    {
        var uiHendler   = _allServices.Resolve<IWindowsDirector>();
        var data        = _allServices.Resolve<IModel>();        
        var settings    = _allServices.Resolve<AppGeneralSettingsSO>();
        var uiFactory   = _allServices.Resolve<IMVPFactory>();

        var uiContainer = (RectTransform)uiHendler.gameObject.transform;
        var overlay     = GameObject.Instantiate(settings.WindowsAnimatorPrefab, uiContainer);

        InitData(data);

        _coroutineStarter.StartCoroutine(WaitingThirdPartDependencesInited(()=> {
            uiHendler.Init((IWindowsDataProvider)data, overlay, uiFactory);
        }));
       
    }

    private IEnumerator WaitingThirdPartDependencesInited(Action startAter)
    {
        yield return null;
        //yield return new WaitUntil(() => OnlineMaps.instance != null);
        
        startAter?.Invoke();
    }

    private void InitData(IModel data)
    {
        //Set mark active number
        (data as IMapDataProvider).SetSelectedMark(1, withNotify: false);
        //Set new Messages count
        (data as IChatDataProvider).SetMessages(null, 1);
        //Set selected Menu
        IWindowsDataProvider windowsProvider = (data as IWindowsDataProvider);
        windowsProvider.ChangeMenuSelect(MenuButtonType.HomeMenu);
        windowsProvider.SceneObjectsLinker = _allServices.Resolve<SceneInstancesLinkerBase>();
        windowsProvider.SceneObjectsLinker.MapsCameraObject.SetActive(false);
    }

    public override void Update()
    {
    }

    public override void Leave()
    {
        throw new NotImplementedException();
    }
}
