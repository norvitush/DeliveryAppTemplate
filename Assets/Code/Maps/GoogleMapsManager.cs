using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using InfinityCode;
using InfinityCode.OnlineMapsExamples;

public class GoogleMapsManager : MonoBehaviour
{
    static GoogleMapsManager Instance = null;

    public static GoogleMapsManager instance {
        get {
            if (Instance == null) {
                Instance = FindObjectOfType<GoogleMapsManager>();
            }
            return Instance;
        }
    }

    Action<int> onClickMarker;
    Action onCameraIdle;
    Action onCameraStart;

    public GameObject onlineMapsObject;
    public RectTransform onlineMapsImageRect;
    public RectTransform canvasRect;
    public SmoothMoveExample smoothMove;

    OnlineMapsMarker userMarker;
    bool canCheckIdle;
    int steps = 0;

    public Texture2D userMarkerTexture;
    public Texture2D[] markersTexture;

    public List<MarkerResized> markersList = new List<MarkerResized>();

    public void InvokeIdleCamera () {
        //print("Idle");
        if (onCameraIdle != null) {
            onCameraIdle();
        }
    }

    public void InvokeStartCamera () {
        //print("Start");
        canCheckIdle = false;
        if (onCameraStart != null) {
            onCameraStart();
        }
    }

    Vector2 prevPosition;

    private OnlineMaps maps;
    private void Update () {
        if (canCheckIdle) {
            var inertia = DragAndZoomInertia.instance.inertiaVector;
            if (Mathf.Abs(inertia.x) + Mathf.Abs(inertia.y) < 0.002f) {
                steps++;
            } else {
                steps = 0;
            }
            if(steps > 1) {
                double lng;
                double lat;
                OnlineMaps.instance.GetPosition(out lng, out lat);
                Vector2 currentPosition = new Vector2((float) lat, (float) lng);
                if (prevPosition != currentPosition) {
                    InvokeIdleCamera();
                    prevPosition = currentPosition;
                }
                canCheckIdle = false;
            }
        }
    }

    private void Awake () {
        markersList.Add(new MarkerResized(userMarkerTexture, new Vector2(77, 78)));
        for(int i = 0; i < markersTexture.Length; i++) {
            markersList.Add(new MarkerResized(markersTexture[i], new Vector2(119, 187)));
        }

        Vector2 rectParent = new Vector2(canvasRect.rect.width, canvasRect.rect.height);
        Vector2 rectTexture = new Vector2(onlineMapsImageRect.sizeDelta.x, onlineMapsImageRect.sizeDelta.y);
        if (rectParent.x / rectTexture.x > rectParent.y / rectTexture.y) {
            onlineMapsImageRect.sizeDelta = new Vector2(rectTexture.x * (rectParent.x / rectTexture.x) + 5,
                rectTexture.y * (rectParent.x / rectTexture.x) + 5);
        } else {
            onlineMapsImageRect.sizeDelta = new Vector2(rectTexture.x * (rectParent.y / rectTexture.y) + 5,
                rectTexture.y * (rectParent.y / rectTexture.y) + 5);
        }

        onlineMapsObject.SetActive(true);
        onlineMapsObject.SetActive(false);
        maps = onlineMapsObject.GetComponentInChildren<OnlineMaps>();

       
    }
    private void OnEnable()
    {
        if (OnlineMapsControlBase.instance == null) return;

        OnlineMapsControlBase.instance.OnMapPress += InvokeStartCamera;
        OnlineMapsControlBase.instance.OnMapRelease += OnMapRelease;
    }
    private void OnDisable()
    {
        if (OnlineMapsControlBase.instance == null) return;

        OnlineMapsControlBase.instance.OnMapPress -= InvokeStartCamera;
        OnlineMapsControlBase.instance.OnMapRelease -= OnMapRelease;
    }

    private void OnMapRelease()
    {
        canCheckIdle = true;
        steps = 0;
    }

    private void Start()
    {
        SetMapsFormRestApi();
    }

    private void SetMapsFormRestApi()
    {
        if (maps == null) return;

        maps.mapType = "mapbox.map";
        maps.activeType.LoadSettings("07User ID21018almazman06Map ID228225clsowc5su004n01pk42ey8ntr12Access Token293290pk.eyJ1IjoiYWxtYXptYW4iLCJhIjoiY2xzb2ozMDUyMGZjcjJsbHJ2azJqanMzdiJ9.waqHp_WCJ3kTMtaHloFgiQ");

        //Constaints.apiConnector.GetMapData(
        //    onSuccess: (dataMaps) =>
        //    {
        //        maps.mapType = dataMaps.map_type;
        //        maps.activeType.LoadSettings(dataMaps.type_settings);
        //    },
        //    onError: (message) =>
        //    {
        //        ShowMessage.instance.Show(message);
        //    });

    }

    public void OpenMap (Action onShowCallback) {
        onlineMapsObject.SetActive(true);
        onlineMapsImageRect.gameObject.SetActive(true);
        OnlineMapsMarkerManager.RemoveAllItems();
        OnlineMapsDrawingElementManager.RemoveAllItems();
        onShowCallback.Invoke();
        InvokeIdleCamera();
    }

    public void CloseMap () {
        onlineMapsObject.SetActive(false);
        onlineMapsImageRect.gameObject.SetActive(false);
        DragAndZoomInertia.instance.isCanInertia = false;
    }

    public void Clear () {
        OnlineMapsMarkerManager.RemoveAllItems();
        OnlineMapsDrawingElementManager.RemoveAllItems();
        userMarker = null;
    }

    public void MoveCamera (float lattitude, float longitude, int zoom, bool isUser = false) {
        DragAndZoomInertia.instance.isCanInertia = false;
        print(lattitude + " " + longitude);
        if (isUser) {
            //lattitude = lattitude - 0.001f;
        }
        OnlineMaps.instance.SetPositionAndZoom(longitude, lattitude, zoom);
    }

    public void AnimateCamera (float lattitude, float longitude, int zoom, bool isUser = false) {
        DragAndZoomInertia.instance.isCanInertia = false;
        print(lattitude + " " + longitude);
        if (isUser) {
            lattitude = lattitude - 0.001f;
        }
        smoothMove.SetLocation(zoom, zoom, zoom);
    }

    public bool AddUserMarker (float lattitude, float longitude, float rotation, string title = "user") {
        if (userMarker == null) {
            userMarker = OnlineMapsMarkerManager.CreateItem(longitude, lattitude, markersList[0].originalTexture);
            userMarker.rotation = rotation;
            userMarker.scale = 0.7f * markersList[0].scaleOriginal;
            userMarker.align = OnlineMapsAlign.Center;
            userMarker.customFields.Add("title", title);
            return false;
        }
        return true;
    }
    public void SetUserMarkerPositionAndRotation (Vector2 position, float rotation) {
        //print(rotation); 
        userMarker.SetPosition(position.y, position.x);
        userMarker.rotation = rotation / 360f;
    }

    public void AddMarker(float lattitude, float longitude, int iconID, float rotation, string title) {
        //print(iconID + " " + markersList.Count);
        var marker = OnlineMapsMarkerManager.CreateItem(longitude, lattitude, markersList[iconID + 1].originalTexture);
        marker.scale = 0.7f * markersList[0].scaleOriginal;
        marker.rotation = rotation;
        marker.align = OnlineMapsAlign.Bottom;
        marker.customFields.Add("title", title);
        marker.OnClick += MarkerClicked;
    }

    void MarkerClicked(OnlineMapsMarkerBase marker) {
        if (onClickMarker != null) {
            onClickMarker(int.Parse(marker.customFields["title"] as string));
        }
    }

    public void AddPolygon (List<Vector2> coordinates, Color borderColor, Color fillColor, float borderSize) {
        List<Vector2> coordinatesVector = new List<Vector2>();
        for(int i = 0; i < coordinates.Count; i++) {
            coordinatesVector.Add(new Vector2((float) coordinates[i].y, (float) coordinates[i].x));
        }
        var newPolygon = new OnlineMapsDrawingPoly(coordinatesVector, new Vector4(0, 0, 0, 0), borderSize, new Vector4(0, 0, 0, 0));
        newPolygon.checkMapBoundaries = false;
        OnlineMapsDrawingElementManager.AddItem(newPolygon);
    }

    public Vector2 GetGeoFromScreenCoordinates (Vector2 screenCoords) {
        screenCoords /= new Vector2(Screen.width, Screen.height);
        screenCoords *= new Vector2(1024, 1024);
        var location = OnlineMapsControlBase.instance.GetCoords(new Vector2(1024 + screenCoords.x, 1024 + screenCoords.y));
        Vector2 coord = new Vector2(location.y, location.x);
        //print(coord);
        return coord;
    }

    public void SetOnClickMarkerListener (Action<int> onClickMarker) {
        this.onClickMarker = onClickMarker;
    }

    public void SetOnCameraIdleListener (Action onCameraIdle) {
        this.onCameraIdle = onCameraIdle;
    }

    public void SetOnCameraStartListener (Action onCameraStart) {
        this.onCameraStart = onCameraStart;
    }

}
