using System;
using System.Collections.Generic;

public interface IWindowsDataProvider : IModel
{ 
    MenuButtonType CurrentMenu { get; }
    SceneInstancesLinkerBase SceneObjectsLinker { get; set; }
    IEnumerable<IView> AllWindowsViews { get; }
    IEnumerable<ISlidePanelView> AllSlidersViews { get; }
    IEnumerable<WindowPresenter> AllPresenters { get; }

    void ChangeMenuSelect(MenuButtonType type, bool withNotify = true);
    bool TryGetPresenterFromCreated(Type type, out WindowPresenter presenter);
    void RegisterPresenter(Type presenterType, WindowPresenter newPresenter);
    //WindowType GetButtonAssociation(MenuButtonType button);
    IView GetWindowAssociation(WindowType wType);
    ISlidePanelView GetSliderPanelAssociation(SliderPanelType type);
    MenuButtonType GetMenuButtonByWindow(WindowType type);
    WindowType GetWindowByMenuButton(MenuButtonType adresses);
}
