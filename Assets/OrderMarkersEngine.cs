using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class OrderMarkersEngine : MonoBehaviour, IPressResolverByID
{
    public event Action<int> OnPressMarker;
    private List<MarkerInstance> markers = new();

    public OnlineMapsRawImageTouchForwarder forwarder;
    public RectTransform container;
    public GameObject prefab;
    public MarkerData[] datas;

    private Canvas canvas;
    private OnlineMaps map;
    private OnlineMapsTileSetControl control;
    private bool isInited;
    [SerializeField] private int[] selected = new int[0];
    private Camera worldCamera
    {
        get
        {
            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay) return null;
            return canvas.worldCamera;
        }
    }

    private void OnEnable()
    {
        canvas = container.GetComponentInParent<Canvas>();
        
        if (isInited == false)
        {
            Activate();
        }
    }

    private void Activate()
    {
        map = OnlineMaps.instance;
        control = OnlineMapsTileSetControl.instance;

        if (control == null || map == null)
        {
            Debug.LogWarning($"{nameof(OrderMarkersEngine)}: No Control for set markers!");
            return;
        }

        FillMarkersList();
        UpdateMarkersList();
        UpdateSelected();

        if (isInited == false)
        {
            isInited = true;
            map.OnMapUpdated += UpdateMarkersList;
            OnlineMapsCameraOrbit.instance.OnCameraControl += UpdateMarkersList;
        }
    }

    public void SetSelected(int[] ids)
    {
        if (ids == null) return;
        selected = ids;

        UpdateSelected();
    }

    private void UpdateSelected()
    {
        foreach (var marker in markers)
        {
            marker.view.SetSelected(selected.Contains(marker.ID));
        }
    }

    public void FillMarkersList(MarkerData[] fillData = null)
    {
        if(fillData != null) datas = fillData;

        foreach (var marker in markers)
        {
            Destroy(marker.gameObject);
        }

        markers = new List<MarkerInstance>();

        foreach (MarkerData data in datas)
        {

            GameObject markerGameObject = Instantiate(prefab) as GameObject;
            markerGameObject.name = data.title;
            RectTransform rectTransform = markerGameObject.transform as RectTransform;
            rectTransform.SetParent(container);
            markerGameObject.transform.localScale = Vector3.one;
            MarkerInstance marker = new MarkerInstance();
            marker.data = data;
            marker.gameObject = markerGameObject;
            marker.transform = rectTransform;

            if (rectTransform.TryGetComponent<ISelectedMark>(out var view))
            {
                view.SetData(this, data.id, data.price);
                marker.view = view;
            }

            markers.Add(marker);
        }
    }

    private void UpdateMarkersList()
    {
        if (markers == null) return;

        foreach (MarkerInstance marker in markers) UpdateMarker(marker);
    }

    private void UpdateMarker(MarkerInstance marker)
    {
        

        double px = marker.data.longitude;
        double py = marker.data.latitude;

        Vector2 screenPosition = control.GetScreenPosition(px, py);
        if (forwarder != null)
        {
            if (!map.InMapView(px, py))
            {
                marker.gameObject.SetActive(false);
                return;
            }

            screenPosition = forwarder.MapToForwarderSpace(screenPosition);            
        }

        if (screenPosition.x < 0 || screenPosition.x > Screen.width ||
            screenPosition.y < 0 || screenPosition.y > Screen.height)
        {
            marker.gameObject.SetActive(false);
            return;
        }

        RectTransform markerRectTransform = marker.transform;

        if (!marker.gameObject.activeSelf) marker.gameObject.SetActive(true);

        Vector2 point;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(markerRectTransform.parent as RectTransform, screenPosition, worldCamera, out point);
        markerRectTransform.localPosition = point;
    }

    public void PressItem(int id)
    {
        OnPressMarker?.Invoke(id);
    }

    [Serializable]
    public class MarkerData
    {
        public int id;
        public string title;
        public double longitude;
        public double latitude;
        public string price;
    }

    public class MarkerInstance
    {
        public MarkerData data;
        public GameObject gameObject;
        public RectTransform transform;
        public ISelectedMark view;
        public int ID => data != null ? data.id : 0;
    }
}
