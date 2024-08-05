using GoldenSoft.Map.Common;
using System;

public interface IMapDataProvider : IModel
{
    OnlineMapsMarkerBase UserMarker { get; }

    event Action OnMapMarkersDataChange;
    OrderMarkersEngine.MarkerData[] MapMarkers { get; }
    (float longitude, float lattitude) DefaultMapPoint { get; }
    int SelectedMark { get; }

    void SetUserLocation(OnlineMapsMarkerBase marker, bool withNotify = true);
    void SetSelectedMark(int id, bool withNotify = true);
}
