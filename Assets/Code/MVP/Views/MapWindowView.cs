using UnityEngine;
using UnityEngine.UI;

public class MapWindowView : MonoBehaviour, IMapWindowView, IPressResolverByID
{

    [SerializeField] private RawImage _onlineMapsImage;
    [SerializeField] private Canvas _rootCanvas;
    [SerializeField] private ExpandingPanelMovement _ordersSlider;
    [SerializeField] private OrderMarkersEngine _markersEngine;
    [SerializeField] private ShowButtonUIAnimator _userLocationButton;

    private MapWindowPresenter _presenter;

    public bool IsActive { get; private set; }
    public Canvas RootCanvas {
        get
        {
            if (_rootCanvas == null) _rootCanvas = _onlineMapsImage.canvas;
            return _rootCanvas;
        }
    }

    public OnlineMapsRawImageTouchForwarder Forwarder => _onlineMapsImage.TryGetComponent(out OnlineMapsRawImageTouchForwarder forwarder) ? forwarder : null;
    public OrderMarkersEngine MarkEngine => _markersEngine;

    private void OnEnable()
    {
        _markersEngine.OnPressMarker += OnPressMarkerID;
    }

    private void OnDisable()
    {
        _markersEngine.OnPressMarker -= OnPressMarkerID;
    }

    private void OnApplicationFocus(bool focus)
    {
        if (_presenter == null) return;

        _presenter.OnFocusView(focus);

    }
    private void OnPressMarkerID(int id)
    {
        _presenter.MarkerPressed(id);
    }

    public void Init(WindowPresenter presenter)
    {
        _presenter = (MapWindowPresenter)presenter;
    }

    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
        if (value)
            Show();
        else
            Hide();

        _onlineMapsImage.gameObject.SetActive(value);
    }

    private void Show()
    {
        this.OnFirstUpdate(() => {
            print($"FirstUpdate for MapWindow");

            _presenter?.OnFirstViewUpdate();
            _presenter?.OnFocusView(true);
            _ordersSlider.Open();
        });
    }
    private void Hide()
    {
        _ordersSlider.Close();
    }

    public void PrepaireMapTexture()
    {
        var mapRect = (RectTransform)_onlineMapsImage.transform;
        mapRect.sizeDelta = Helpers.GetFittedSize(new Vector2(mapRect.rect.width, mapRect.rect.height), (RectTransform)RootCanvas.transform, Helpers.FitType.ByHeight, true);
    }

    public void OnClickToUserLocation() => _presenter?.ToUserLocation();

    public void SetActiveUserLocationButton(bool val)
    {
        _userLocationButton.gameObject.SetActive(val);

        if (val)
        {
            _userLocationButton.Show();
        }
    }

    public void PressItem()
    {
        PressItem(0);
    }

    public void PressItem(int id)
    {
        _presenter.OnPressSlot(id);
    }
}
