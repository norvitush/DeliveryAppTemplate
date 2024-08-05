using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class SceneInstancesLinker: SceneInstancesLinkerBase
{
    [SerializeField] private RectTransform  _uiContainer;
    [SerializeField] private GameObject     _onlineMapsCameraObject;
    [SerializeField] private OnlineMaps     _onlineMaps;
    //[SerializeField, GameObjectOfType(typeof(IView))] private GameObject _IView;

    [SerializeField] private List<WindowAssociation> _windowsLinks  = new List<WindowAssociation>();
    [SerializeField] private List<SlidersAssociation> _slidersLinks = new List<SlidersAssociation>();

    public override IWindowsDirector WindowsDirector                            => _uiContainer.TryGetComponent(out IWindowsDirector windowsDirector) ? windowsDirector : null;
    public override OnlineMaps OnlineMaps                                       => _onlineMaps;
    public override GameObject MapsCameraObject                                 => _onlineMapsCameraObject;
    public override List<IView> WindowsViewsList                                       => _windowsLinks.Select(w => w.WindowObject.GetComponent<IView>()).OfType<IView>()?.ToList();
    public override List<ISlidePanelView> SlidersViewsList                       => _slidersLinks.Select(w => w.gameObject.GetComponent<ISlidePanelView>()).OfType<ISlidePanelView>()?.ToList();
    public override Dictionary<WindowType, MenuButtonType> ButtonsAssociations  => _windowsLinks.Where(a => a.ButtonType != MenuButtonType.None)?.ToDictionary(a=> a.WindowType, a => a.ButtonType);

    public override Dictionary<WindowType, IView> WindowsAssociations
    {
        get 
        {
            Dictionary<WindowType, IView> forReturn = new();
            foreach (var link in _windowsLinks)
            {
                if(link.WindowObject.TryGetComponent<IView>(out var view))
                {
                    forReturn.Add(link.WindowType, view);
                }
            }

            return forReturn;
        }
    }

    public override Dictionary<SliderPanelType, ISlidePanelView> SlidersAssociations
    {
        get
        {
            Dictionary<SliderPanelType, ISlidePanelView> forReturn = new();
            foreach (var link in _slidersLinks)
            {
                if (link.gameObject.TryGetComponent<ISlidePanelView>(out var sliderView))
                {
                    forReturn.Add(link.Type, sliderView);
                }
            }

            return forReturn;
        }
    }
}