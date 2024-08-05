using GoldenSoft.Map.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Android;

public class AppDataKeeper : IWindowsDataProvider, IChatDataProvider, IMapDataProvider, IProfileDataProvider, IOrdersDataProvider, ISlotDetailsDataProvider, INotificationsDataProvider
{
    public event Action OnChange;
    public event Action OnMapMarkersDataChange;

    private readonly Dictionary<Type, object> _customData = new();
    private readonly Dictionary<Type, WindowPresenter> _presenters = new();
    private readonly List<OrderMarkersEngine.MarkerData> _mapMarkers = new();

    private readonly AppGeneralSettingsSO _settings;
    private SceneInstancesLinkerBase _sceneObjectsLinker;

    public AppDataKeeper(AppGeneralSettingsSO settings) => _settings = settings;

    public object this[Type type]
    {
        get
        {
            return _customData.TryGetValue(type, out var data) ? data : default;
        }
    }

    #region IWindowsDataProvider
    private Dictionary<WindowType, MenuButtonType> _buttonAssociations = new();
    private Dictionary<WindowType, IView> _windowAssociations = new();
    private Dictionary<SliderPanelType, ISlidePanelView> _sliderAssociations = new();
    public IEnumerable<IView> AllWindowsViews { get; private set; }
    public IEnumerable<ISlidePanelView> AllSlidersViews { get; private set; }
    public IEnumerable<WindowPresenter> AllPresenters => _presenters.Values;

    public ISlidePanelView GetSliderPanelAssociation(SliderPanelType type)
    {
        return _sliderAssociations.TryGetValue(type, out var slider) ? slider : default;
    }
    public MenuButtonType GetMenuButtonByWindow(WindowType type)
    {
        return _buttonAssociations.TryGetValue(type, out MenuButtonType val) ? val : MenuButtonType.None;
    }

    public WindowType GetWindowByMenuButton(MenuButtonType type)
    {
        return _buttonAssociations.FirstOrDefault(val =>  val.Value == type).Key;
    }

    public IView GetWindowAssociation(WindowType wType)
    {
        return _windowAssociations.TryGetValue(wType, out var window) ? window : default;
    }
    public MenuButtonType CurrentMenu { get; private set; }
    
    public SceneInstancesLinkerBase SceneObjectsLinker { get => _sceneObjectsLinker; set => ObjectLinkerSetter(value); }

    private void ObjectLinkerSetter(SceneInstancesLinkerBase value)
    {
        _sceneObjectsLinker = value;

        AllWindowsViews = _sceneObjectsLinker.WindowsViewsList;
        AllSlidersViews = _sceneObjectsLinker.SlidersViewsList;

        _buttonAssociations = _sceneObjectsLinker.ButtonsAssociations;
        _windowAssociations = _sceneObjectsLinker.WindowsAssociations;
        _sliderAssociations = _sceneObjectsLinker.SlidersAssociations;

        Debug.Log($"SET AllViews cnt: {AllWindowsViews.Count()}");
        Debug.Log($"SET AllSliders cnt: {AllSlidersViews.Count()}");
        Debug.Log($"SET _buttonAssociations cnt: {_buttonAssociations.Count}");
        Debug.Log($"SET _windowAssociations cnt: {_windowAssociations.Count}");        
    }

    public void ChangeMenuSelect(MenuButtonType type, bool withNotify = true)
    {
        bool needInvoke = CurrentMenu != type;
        CurrentMenu = type;
        ValidateAndNotify(needInvoke & withNotify);
    }

    public bool TryGetPresenterFromCreated(Type presenterType, out WindowPresenter presenter)
    {
        presenter = null;
        if (presenterType == null) return false;

        return _presenters.TryGetValue(presenterType, out presenter);
    }
    public void RegisterPresenter(Type presenterType, WindowPresenter newPresenter)
    {
        if (presenterType == null) return;

        if (_presenters.TryGetValue(presenterType, out var presenter))
        {
            presenter = newPresenter;
        }
        else
        {
            _presenters.Add(presenterType, newPresenter);
        }

    }

    #endregion

    #region IChatDataProvider

    public int NewMessages { get; private set; }


    public void SetMessages(object messages, int newMessageCount, bool withNotify = true)
    {
        NewMessages = newMessageCount;
        ValidateAndNotify(withNotify);
    }

    #endregion

    #region IMapDataProvider
    public OnlineMapsMarkerBase UserMarker { get; private set; }
    public int SelectedMark { get; private set; }
    public OrderMarkersEngine.MarkerData[] MapMarkers
    {
        get => _mapMarkers.ToArray();
        private set
        {
            _mapMarkers.Clear();
            for (int i = 0; i < value.Length; i++)
            {
                _mapMarkers.Add(value[i]);
            }
        }
    }

    public (float longitude, float lattitude) DefaultMapPoint => _settings != null ? (_settings.MapDefaultStartPoint.x, _settings.MapDefaultStartPoint.y) : (0, 0);


    void IMapDataProvider.SetUserLocation(OnlineMapsMarkerBase marker, bool withNotify)
    {
        UserMarker = marker;
        ValidateAndNotify(withNotify);
    }

    public void SetSelectedMark(int id, bool withNotify = true)
    {
        if (id <= 0) return;
        SelectedMark = id;
        if (withNotify) OnMapMarkersDataChange?.Invoke();
    }

    #endregion

    #region IOrdersDataProvider
    public int SelectedOrder { get; private set; }

    public void SetSelectedOrder(int id)
    {
        SelectedOrder = id;
        ValidateAndNotify(true);
    }
    #endregion

    public void ValidateAndNotify(bool withNotify)
    {
        if (withNotify == false) return;
        //Debug.Log($"withNotify: {withNotify}");
        OnChange?.Invoke();
    }
}
