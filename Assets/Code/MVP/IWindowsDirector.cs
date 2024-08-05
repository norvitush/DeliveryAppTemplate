using System;
using UnityEngine;

public interface IWindowsDirector
{
    public int OverlaySiblingIndex { get; }
    public void Init(IWindowsDataProvider dataModel, IOverlayAnimator overlayAnimator, IMVPFactory factory);

    public void OpenWindow(WindowType type, bool asRootWindow = false, Action closeCallback = null);
    public void OpenSlider(SliderPanelType type, Action closeCallback = null);
    public void GoPreviousWindow();
    public GameObject gameObject { get; }
}
