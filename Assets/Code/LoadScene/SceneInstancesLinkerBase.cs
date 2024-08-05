using System.Collections.Generic;
using UnityEngine;

public abstract class SceneInstancesLinkerBase : MonoBehaviour
{
    public abstract IWindowsDirector WindowsDirector { get; }
    public abstract OnlineMaps OnlineMaps { get; }
    public abstract GameObject MapsCameraObject { get; }
    public abstract List<IView> WindowsViewsList { get; }
    public abstract List<ISlidePanelView> SlidersViewsList { get; }
    public abstract Dictionary<WindowType, MenuButtonType> ButtonsAssociations { get; }
    public abstract Dictionary<WindowType, IView> WindowsAssociations { get; }

    public abstract Dictionary<SliderPanelType, ISlidePanelView> SlidersAssociations { get; }

}