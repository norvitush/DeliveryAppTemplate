using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneBootstrap : MonoBehaviour
{

    [SerializeField] private SceneInstancesLinkerBase _sceneObjectsLinker;
    [SerializeField] private AppGeneralSettingsSO _settingsSO;

    private IGameStateMachine _stateMachine;    

    private void Awake()
    {
        _stateMachine = new AppStateMachine(ServiceLocator.Instance, _sceneObjectsLinker, _settingsSO, this);
        _stateMachine.IsDebugOn  = true;

        _stateMachine.Enter<BootstrapState>();
    }

    private void Update()
    {
        _stateMachine.UpdateStateLogic();
    }

    private void FixedUpdate()
    {
        _stateMachine.UpdateStatePhisics();
    }
}
