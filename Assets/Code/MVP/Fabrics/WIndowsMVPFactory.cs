using System;
using System.Collections.Generic;
using UnityEngine;

public class WIndowsMVPFactory : IMVPFactory
{
    private readonly AppGeneralSettingsSO _settingsSO;
    private readonly Transform _uiContainer;
    private readonly SceneInstancesLinkerBase _sceneObjectsLinker;
    private readonly IModel _model;
    private readonly IWindowsDirector _windowsDirector;

    public WIndowsMVPFactory(IModel model, AppGeneralSettingsSO settingsSO, Transform uiContainer, SceneInstancesLinkerBase sceneObjectsLinker)
    {
        _sceneObjectsLinker = sceneObjectsLinker;
        _uiContainer        = uiContainer;
        _settingsSO         = settingsSO;
        _model              = model;
        _windowsDirector    = sceneObjectsLinker.WindowsDirector;
    }

    public WindowPresenter Create(WindowType wType)
    {
        var linkedWindow = (_model as IWindowsDataProvider).GetWindowAssociation(wType);

        if(linkedWindow == null)
            throw new System.NotImplementedException($"No linked window (View object) for {wType} window type. Check Scene Object Linker!");

        switch (wType)
        {
            case WindowType.Empty:
                break;
            case WindowType.SlotsMap:
                return CreateMapWindowPresenter(linkedWindow);
            case WindowType.SlotOrderDetails:
                return CreateSlotDetailsWindowPresenter(linkedWindow);
            case WindowType.Orders:
                return CreateOrdersWindowPresenter(linkedWindow);
            case WindowType.OrderDetails:
                return CreateOrdersDetailesWindowPresenter(linkedWindow);
            case WindowType.Notifications:
                return CreateNotificationsWindowPresenter(linkedWindow);
            case WindowType.GetOrderByQR:
                return CreateGetOrderByQRWindowPresenter(linkedWindow);
            case WindowType.Profile:
                return CreateProfileWindowPresenter(linkedWindow);
            case WindowType.Chat:
                return CreateChatWindowPresenter(linkedWindow);
            case WindowType.Support:
                break;
            default:
                throw new System.NotImplementedException($"Need implementation for {wType} window type");
        }


        return default;
    }

    public WindowPresenter Create(SliderPanelType sType)
    {
        var linkedWindow = (_model as IWindowsDataProvider).GetSliderPanelAssociation(sType);

        if (linkedWindow == null)
            throw new System.NotImplementedException($"No linked window (View object) for {sType} slider type. Check Scene Object Linker!");

        switch (sType)
        {
            case SliderPanelType.Empty:
                break;
            case SliderPanelType.CancelOrder:
                return CreateCancelOrderPresenter(linkedWindow);
            default:
                throw new System.NotImplementedException($"Need implementation for {sType} window type");
        }


        return default;

    }

    public Type GetPresenterType(WindowType windowType)
    {
        return windowType switch
        {
            WindowType.Empty            => throw new NotImplementedException(),
            WindowType.SlotsMap         => typeof(MapWindowPresenter),
            WindowType.SlotOrderDetails => typeof(SlotDetailsWindowPresenter),
            WindowType.Orders           => typeof(OrdersWindowPresenter),
            WindowType.OrderDetails     => typeof(OrderDetailesWindowPresenter),
            WindowType.Notifications    => typeof(NotificationWindowPresenter),
            WindowType.GetOrderByQR     => typeof(GetOrderWindowPresenter),
            WindowType.Profile          => typeof(ProfileWindowPresenter),
            WindowType.Chat             => typeof(ChatWindowPresenter),
            WindowType.Support          => throw new NotImplementedException(),
            _                           => throw new NotImplementedException(),
        };
    }

    public Type GetPresenterType(SliderPanelType sliderType)
    {
        return sliderType switch
        {
            SliderPanelType.Empty => throw new NotImplementedException(),
            SliderPanelType.CancelOrder => typeof(CancelOrderPresenter),
            _ => throw new NotImplementedException(),
        };
    }
    
    private NotificationWindowPresenter CreateNotificationsWindowPresenter(IView linkedWindow)
    {
        if (linkedWindow != null && linkedWindow is INotificationsView concreteView)
        {
            var presenter = new NotificationWindowPresenter(concreteView, (INotificationsDataProvider)_model, _windowsDirector);
            concreteView.Init(presenter);

            return presenter;
        }
        else
        {
            throw new Exception($"*Menu Links ERROR!* {linkedWindow.GetType()} doesn`t contain {nameof(IMapWindowView)} on create {nameof(MapWindowPresenter)} for MapWindow button");
        }
    }
    
    private ChatWindowPresenter CreateChatWindowPresenter(IView linkedWindow)
    {
        if (linkedWindow != null && linkedWindow is IChatWindowView concreteView)
        {
            var presenter = new ChatWindowPresenter(concreteView, (IChatDataProvider)_model, _windowsDirector);
            concreteView.Init(presenter);

            return presenter;
        }
        else
        {
            throw new Exception($"*Menu Links ERROR!* {linkedWindow.GetType()} doesn`t contain {nameof(IMapWindowView)} on create {nameof(MapWindowPresenter)} for MapWindow button");
        }
    }    
    
    private WindowPresenter CreateProfileWindowPresenter(IView linkedWindow)
    {
        if (linkedWindow != null && linkedWindow is IProfileWindowView concreteView)
        {
            var presenter = new ProfileWindowPresenter(concreteView, (IProfileDataProvider)_model, _windowsDirector);
            concreteView.Init(presenter);

            return presenter;
        }
        else
        {
            throw new Exception($"*Menu Links ERROR!* {linkedWindow.GetType()} doesn`t contain {nameof(IMapWindowView)} on create {nameof(MapWindowPresenter)} for MapWindow button");
        }
    }
    
    private OrdersWindowPresenter CreateOrdersWindowPresenter(IView linkedWindow)
    {
        if (linkedWindow != null && linkedWindow is IOrdersWindowView concreteView)
        {
            var presenter = new OrdersWindowPresenter(concreteView, (IOrdersDataProvider)_model, _windowsDirector);
            concreteView.Init(presenter);

            return presenter;
        }
        else
        {
            throw new Exception($"*Menu Links ERROR!* {linkedWindow.GetType()} doesn`t contain {nameof(IMapWindowView)} on create {nameof(MapWindowPresenter)} for MapWindow button");
        }
    }          
    
    private SlotDetailsWindowPresenter CreateSlotDetailsWindowPresenter(IView linkedWindow)
    {
        if (linkedWindow != null && linkedWindow is ISlotDetailsView concreteView)
        {
            var presenter = new SlotDetailsWindowPresenter(concreteView, (ISlotDetailsDataProvider)_model, _windowsDirector);
            concreteView.Init(presenter);

            return presenter;
        }
        else
        {
            throw new Exception($"*Menu Links ERROR!* {linkedWindow.GetType()} doesn`t contain {nameof(IMapWindowView)} on create {nameof(MapWindowPresenter)} for MapWindow button");
        }
    }       
    
    private GetOrderWindowPresenter CreateGetOrderByQRWindowPresenter(IView linkedWindow)
    {
        if (linkedWindow != null && linkedWindow is IGetOrderView concreteView)
        {
            var presenter = new GetOrderWindowPresenter(concreteView, (IOrdersDataProvider)_model, _windowsDirector);
            concreteView.Init(presenter);

            return presenter;
        }
        else
        {
            throw new Exception($"*Menu Links ERROR!* {linkedWindow.GetType()} doesn`t contain {nameof(IMapWindowView)} on create {nameof(MapWindowPresenter)} for MapWindow button");
        }
    }    
    
    private OrderDetailesWindowPresenter CreateOrdersDetailesWindowPresenter(IView linkedWindow)
    {
        if (linkedWindow != null && linkedWindow is IOrderDetailesView concreteView)
        {
            var presenter = new OrderDetailesWindowPresenter(concreteView, (IOrdersDataProvider)_model, _windowsDirector);
            concreteView.Init(presenter);

            return presenter;
        }
        else
        {
            throw new Exception($"*Menu Links ERROR!* {linkedWindow.GetType()} doesn`t contain {nameof(IOrderDetailesView)} on create {nameof(OrderDetailesWindowPresenter)} for MapWindow button");
        }
    }
    
    private MapWindowPresenter CreateMapWindowPresenter(IView linkedWindow)
    {
        
        if (linkedWindow != null && linkedWindow is IMapWindowView concreteView)
        {
            var presenter = new MapWindowPresenter(concreteView, (IMapDataProvider)_model, _sceneObjectsLinker.MapsCameraObject, _windowsDirector);
            concreteView.Init(presenter);

            return presenter;
        }
        else
        {
            throw new Exception($"*Menu Links ERROR!* {linkedWindow.GetType()} doesn`t contain {nameof(IMapWindowView)} on create {nameof(MapWindowPresenter)} for MapWindow button");
        }
    }

    private CancelOrderPresenter CreateCancelOrderPresenter(ISlidePanelView linkedWindow)
    {

        if (linkedWindow != null && linkedWindow is ICancelOrderView concreteView)
        {
            var presenter = new CancelOrderPresenter(concreteView, (IOrdersDataProvider)_model, _windowsDirector);
            concreteView.Init(presenter);

            return presenter;
        }
        else
        {
            throw new Exception($"*Menu Links ERROR!* {linkedWindow.GetType()} doesn`t contain {nameof(ICancelOrderView)} on create {nameof(CancelOrderPresenter)} for MapWindow button");
        }
    }

}
