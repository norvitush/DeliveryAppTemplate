using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


[RequireComponent(typeof(LoaderAnimator))]
public class PreloadBootstrap : MonoBehaviour
{
    [SerializeField] private string _mainSceneName;
    [SerializeField] private Camera _loadSceneCamera;
    [SerializeField] private AppGeneralSettingsSO _settingsSO;
    string pluginName = "com.goldensoft.unity.ColorSettings";

    private LoaderAnimator _loaderAnimator;    

    private void Awake()
    {
        _loaderAnimator = GetComponent<LoaderAnimator>();
    }

    private void Start()
    {

        Application.targetFrameRate = 120;
        Screen.fullScreen = false;
        try
        {
#if !UNITY_EDITOR
        var javaObject = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
        new AndroidJavaObject(pluginName, javaObject);
#endif
        }
        catch (Exception ex)
        {
            //prevent android ui thread exception (TO DO: recompile native plugin)
            Debug.Log(ex);
        }


        this.OnFirstUpdate(()=> {

            _loaderAnimator.Play(onEndAnimation: () => LoadMainScene());

            //Data loading
            var appData = new AppDataKeeper(_settingsSO);

            ServiceLocator.Instance.Register<IModel>(() => appData);
        });
    }

    private void LoadMainScene()
    {
        Scene current = SceneManager.GetActiveScene();
        Debug.Log(Input.location.status);
     
        SceneManager.LoadSceneAsync(_mainSceneName, LoadSceneMode.Additive).completed += operation => {
            Destroy(gameObject);
            Destroy(_loadSceneCamera.gameObject);
            //SceneManager.UnloadSceneAsync(current);
        };
    }
}
