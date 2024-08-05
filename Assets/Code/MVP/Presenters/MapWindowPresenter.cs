using Assets.Code.Maps;
using InfinityCode.OnlineMapsExamples;
using System;
using UnityEngine;

using static UnityEngine.Debug;



public class MapWindowPresenter : WindowPresenter
{
    public enum MapMoveState { AnimateMove, CapturedMove, ReleasedMove, Idle}

    private const float INERTIA_TRESHOLD = 0.003f;
    private OnlineMaps _mapsHandler;
    private GameObject _mapsCameraObject;
    private IMapSmoothMoveController _smoothMover;
    private DragAndZoomInertia _dragAndZoomInertiaHandler;
    private User2DLocationService _locationService;
    private bool _isFocused;
    private IMapWindowView View => (IMapWindowView)_view;
    private IMapDataProvider Model => (IMapDataProvider)_model;
    public MapMoveState State 
    { 
        get => _mapMoveState; 
        set 
        { 
            //Debug.Log($"{_state}->{value}"); 
            _mapMoveState = value;} 
    }

    private Action _onFirstViewUpdate;
    private MapMoveState _mapMoveState;

    private bool _viewUserLocationActivated = false;

    public MapWindowPresenter(IMapWindowView view, IMapDataProvider model, GameObject mapsCameraObject, IWindowsDirector windowsDirector) : base(view, model, windowsDirector)
    {
        _mapsCameraObject = mapsCameraObject;
        _mapsHandler      = view.Forwarder.map;

        _dragAndZoomInertiaHandler = _mapsHandler.GetComponent<DragAndZoomInertia>();
        _smoothMover               = _mapsHandler.GetComponent<IMapSmoothMoveController>();
        _locationService           = _mapsHandler.GetComponent<User2DLocationService>();

        if (_dragAndZoomInertiaHandler == null || _smoothMover == null) throw new Exception($" Hasn`t required dependense in {nameof(OnlineMaps)}");

        _view.SetActive(false);      
    }
    protected override void Init()
    {             
        SetMapsFormRestApi();

        View.SetActive(false);
        View.PrepaireMapTexture();
    }

    protected override void OnEnable()
    {

        if (_mapsHandler == null || _mapsHandler.control == null) return;

        

        _mapsHandler.OnChangePosition += OnChangePos;
        _mapsHandler.control.OnMapPress += OnCaptureMap;
        _mapsHandler.control.OnMapRelease += OnReleaseMap;
        _smoothMover.OnCameraIdle += OnAnimateMoveEnd;
        _locationService.OnLocationChanged += OnLocationChanged;        
        _locationService.OnLocationInited += SaveUserMarkerToModel;        
        _locationService.OnLocationDisable += SaveUserMarkerToModel;        
        _model.OnChange += OnChangeModelData;
        //_locationService.OnCompassChanged += OnCompassChanged;

        _mapsCameraObject.SetActive(true);


        State = MapMoveState.Idle;

        _onFirstViewUpdate = () => { ContinueAfterFirstUppdate(); };
      
    }


    protected override void OnDisable()
    {
        if (_mapsHandler == null || _mapsHandler.control == null) return;

        _mapsHandler.OnChangePosition -= OnChangePos;
        _mapsHandler.control.OnMapPress -= OnCaptureMap;
        _mapsHandler.control.OnMapRelease -= OnReleaseMap;
        _smoothMover.OnCameraIdle -= OnAnimateMoveEnd;
        _locationService.OnLocationChanged -= OnLocationChanged;
        _locationService.OnLocationInited -= SaveUserMarkerToModel;
        _locationService.OnLocationDisable -= SaveUserMarkerToModel;
        _model.OnChange -= OnChangeModelData;

        _mapsCameraObject.SetActive(false);
        View.SetActive(false);
        DragAndZoomInertia.instance.isCanInertia = false;

        State = MapMoveState.Idle;
    }


    private void ContinueAfterFirstUppdate()
    {
        View.MarkEngine.SetSelected(new int[] { Model.SelectedMark });
        Log($"Continue after first update| location service run: {_locationService.IsStarted}");

        // to default position
        (float longitude, float latitude) = Model.DefaultMapPoint;
        Log($"to default position: {longitude}, {latitude}");
        MoveCamera(latitude, longitude, 18);

        if(_locationService.IsStarted) Model.SetUserLocation(_locationService.Marker);
    }

    private void OnLocationChanged(Vector2 location)
    {
        SaveUserMarkerToModel();
    }

    private void SaveUserMarkerToModel()
    {
        Model.SetUserLocation(_locationService.Marker);
    }

    private void SetMapsFormRestApi()
    {
        //Model.RequestMapProviderInfo()   //after update in model - change provider in OnChange event
        _mapsHandler.mapType = "mapbox.map";
        _mapsHandler.activeType.LoadSettings("07User ID21018almazman06Map ID228225clsowc5su004n01pk42ey8ntr12Access Token293290pk.eyJ1IjoiYWxtYXptYW4iLCJhIjoiY2xzb2ozMDUyMGZjcjJsbHJ2azJqanMzdiJ9.waqHp_WCJ3kTMtaHloFgiQ");

        //Log($"SetMapsFormRestApi: {_mapsHandler.mapType} {_mapsHandler.activeType}");
    }

    public void MarkerPressed(int id)
    {
        int selected = Model.SelectedMark;

        if (selected == id) return;

        Model.SetSelectedMark(id);
        View.MarkEngine.SetSelected(new int[] { id });
    }

    public void OnFocusView(bool focus)
    {
        _isFocused = focus;
        if (focus)
        {
            Input.location.Start(1, 1);
        }
        else
        {
            Input.location.Stop();
        }
    }

    public void ToUserLocation()
    {
        Log($"ToUserLocation*| location service run: {_locationService.IsStarted}");


        if (_locationService.IsStarted == false) return;

        //to user position
        _locationService.GetLocation(out float longitude, out float latitude);
        if (longitude != 0)
        {
            //Model.SetUserLocation(new Location(longitude, latitude), withNotify: false);

            Log($"User location {longitude}:{latitude}");
            AnimateCamera(latitude, longitude, 18);
        }
       
    }

    private void OnChangeModelData()
    {
        bool isActivated = (Model.UserMarker != null && _locationService.IsStarted);
        Log($"OnChangeModelData model.location data:{Model.UserMarker != null} running location service: {_locationService.IsStarted} _viewUserLocationActivated:{_viewUserLocationActivated}");
        if (isActivated != _viewUserLocationActivated)
        {
            _viewUserLocationActivated = isActivated;
            View.SetActiveUserLocationButton(isActivated);

            if (!isActivated) Model.SetUserLocation(null, withNotify: false);
        }
    }
    private void OnChangePos()
    {
        //_mapsHandler.GetPosition(out var lng, out var lat);

        if (State == MapMoveState.Idle || State == MapMoveState.CapturedMove || State == MapMoveState.AnimateMove) return;

        var inertia = _dragAndZoomInertiaHandler.inertiaVector;
        if (Mathf.Abs(inertia.x) + Mathf.Abs(inertia.y) < INERTIA_TRESHOLD)
        {
            State = MapMoveState.Idle;
        }

    }
    private void OnAnimateMoveEnd()
    {
        State = MapMoveState.Idle;

        _mapsHandler.GetPosition(out var lng, out var lat);

        //Log($"_mapsHandler.position:{ _mapsHandler.position}");
        //_mapsHandler.SetPosition(lng, lat, false);
        //Log($"On Idle Cam {lng}, {lat} t:{ _mapsHandler.position}");
        //Log($"On Idle Cam _mapsHandler.position: {_mapsHandler.position}");

        _mapsHandler.SetPositionAndZoom(lng, lat, _mapsHandler.floatZoom);
        
    }

    private void OnReleaseMap()
    {
        State = MapMoveState.ReleasedMove;
        //Log($"OnReleaseMap {_dragAndZoomInertiaHandler.isCanInertia}");
    }

    private void OnCaptureMap()
    {
        State = MapMoveState.CapturedMove;
        //Log($"OnCaptureMap {_dragAndZoomInertiaHandler.isCanInertia}");
    }


    public void MoveCamera(float lattitude, float longitude, int zoom, bool isUser = false)
    {
        _dragAndZoomInertiaHandler.isCanInertia = false;
        OnlineMaps.instance.SetPositionAndZoom(longitude, lattitude, zoom);
        //print(lattitude + " " + longitude);
        //if (isUser)
        //{
        //    //lattitude = lattitude - 0.001f;
        //}
        //OnlineMaps.instance.SetPositionAndZoom(longitude, lattitude, zoom);
    }

    public void AnimateCamera(float lattitude, float longitude, int zoom, bool isUser = false)
    {        
        State = MapMoveState.AnimateMove;
        _dragAndZoomInertiaHandler.isCanInertia = false;
        Log(lattitude + " " + longitude);
        _smoothMover.SetLocation(lattitude, longitude, zoom);
    }

    public override void LogicUpdate()
    {
        
    }

    public override void OnFirstViewUpdate() => _onFirstViewUpdate?.Invoke();

    public void OnPressSlot(int id)
    {
        View.SetActive(false);
        _windowsDirector.OpenWindow(WindowType.SlotOrderDetails, asRootWindow: false, OnBackFromDetails);
    }

    public void OnBackFromDetails()
    {
        View.SetActive(true);

    }

}
