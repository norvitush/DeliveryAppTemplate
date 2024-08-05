using UnityEngine;

public class AppStateMachine : BaseStateMachine
{
    public AppStateMachine(IServiceProvider allServices, SceneInstancesLinkerBase sceneObjectsLinker, AppGeneralSettingsSO settingsSO, MonoBehaviour coroutineRunner)
    {
        AddState(new BootstrapState (allServices, this, sceneObjectsLinker, settingsSO));
        AddState(new AppLoopState   (allServices, this, coroutineRunner));
        AddState(new SignInState    (allServices, this));
        AddState(new LogoutState    (allServices, this));
    }
}
