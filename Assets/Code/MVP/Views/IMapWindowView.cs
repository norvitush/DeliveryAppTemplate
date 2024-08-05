public interface IMapWindowView : IView
{
    OnlineMapsRawImageTouchForwarder Forwarder { get; }
    OrderMarkersEngine MarkEngine { get; }
    void PrepaireMapTexture();
    void SetActiveUserLocationButton(bool val);
}
