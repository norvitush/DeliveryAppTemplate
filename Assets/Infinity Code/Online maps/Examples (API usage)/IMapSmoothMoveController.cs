using System;
using UnityEngine;

public interface IMapSmoothMoveController
{
    event Action OnCameraIdle;
    event Action OnCameraStartMove;

    void SetLocation(float lattitude, float longitude, int zoom);
}