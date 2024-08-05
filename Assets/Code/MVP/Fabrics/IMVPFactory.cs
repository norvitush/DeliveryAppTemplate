using System;
using UnityEngine;

public interface IMVPFactory
{
    WindowPresenter Create(WindowType windowType);
    WindowPresenter Create(SliderPanelType sliderType);
    Type GetPresenterType(WindowType windowType);
    Type GetPresenterType(SliderPanelType sliderType);
}


