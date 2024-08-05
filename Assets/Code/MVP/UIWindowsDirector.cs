using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIWindowsDirector : MonoBehaviour, IWindowsDirector
{
    [SerializeField] private GameObject _fastMessage;
    [SerializeField] private LowerPanelView _lowerPanel;
    
    private IWindowsDataProvider _data;
    private IOverlayAnimator     _overlayAnimator;
    private IMVPFactory          _factory;

    private HierarchyViewCounter     _hierarchyCounter;

    public bool IsInited { get; private set; }

    public int OverlaySiblingIndex => _lowerPanel.transform.GetSiblingIndex();
        
    private void OnDisable()
    {
        IsInited = false;
        _data.OnChange      -= OnCommonChangeModelData;
        _lowerPanel.OnSelectMenu -= OnSelectMenuFromView;
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _hierarchyCounter.OnPressBack();
        }
    }
    
    public void Init(IWindowsDataProvider dataModel, IOverlayAnimator overlayAnimator, IMVPFactory factory)
    {
        if (gameObject.activeInHierarchy == false) gameObject.SetActive(true);

        _data = dataModel;
        _overlayAnimator = overlayAnimator;
        _factory = factory;

        HideAllWindowsViews();
        HideAllSlidersViews();

        _hierarchyCounter = new HierarchyViewCounter(_fastMessage);

        _lowerPanel.Show();
        _lowerPanel.SetCounterLableValue((dataModel as IChatDataProvider).NewMessages);
        _lowerPanel.SetSelected(_data.CurrentMenu);

        _overlayAnimator.AnimateTranzition(() => { OpenCurrentMenuWindow(); });

        if (IsInited) return;

        IsInited = true;
        _data.OnChange += OnCommonChangeModelData;
        _lowerPanel.OnSelectMenu += OnSelectMenuFromView;

        (overlayAnimator as MonoBehaviour).transform.SetSiblingIndex(OverlaySiblingIndex);
    }
    
    public void GoPreviousWindow() => _hierarchyCounter.OnPressBack();

    public void OpenWindow(WindowType type, bool asSingle = false, Action closeCallback = null)
    {
        if (type == WindowType.Empty) return;

        Type presenterType = _factory.GetPresenterType(type);

        WindowPresenter presenter =
            GetOrCreatePresenterByType(presenterType, windowType: type);
        
        if (presenter == null) throw new Exception($"Error of Get or Create Presenter for {type} window");

        SetForClose(asSingle, closeCallback, presenter);
        presenter.SetActive(true);
    }

    public void OpenSlider(SliderPanelType type, Action closeCallback = null)
    {
        if (type == SliderPanelType.Empty) return;

        Type presenterType = _factory.GetPresenterType(type);

        WindowPresenter presenter =
        GetOrCreatePresenterByType(presenterType, sliderType: type);

        if (presenter == null) throw new Exception($"Error of Get or Create Presenter for {type} window");

        SetForClose(false, closeCallback, presenter);
        presenter.SetActive(true);
    }
    
    public void OpenCurrentMenuWindow()
    {
        WindowType wType = _data.GetWindowByMenuButton( _data.CurrentMenu );        
        OpenWindow(wType, asSingle: true);
    }

    private void OnSelectMenuFromView(MenuButtonType selected)
    {
        print($"OnSelectMenuFromView: CurrentMenu: {_data.CurrentMenu} , selected: {selected}");

        if (_data.CurrentMenu != selected) _overlayAnimator.AnimateTranzition(()=> {
            _data.ChangeMenuSelect(selected, withNotify: false);
            OpenCurrentMenuWindow();
        });
       
    }
    
    private void HideAllSlidersViews()
    {
        foreach (var view in _data.AllSlidersViews)
        {
            if (view.IsActive || view.gameObject.activeInHierarchy) { view.SetActive(false); }
            //print($"Game VIEW: {view.gameObject.name}");
        }
    }

    private void HideAllWindowsViews()
    {
        foreach (var view in _data.AllWindowsViews)
        {
            if(view.IsActive || view.gameObject.activeInHierarchy) view.SetActive(false);
            //print($"Game VIEW: {view.gameObject.name}");
        }
    }
    
    private void OnCommonChangeModelData()
    {
        print($"OnChangeModelData: CurrentMenu: {_data.CurrentMenu} , _lowerPanel.Selected: {_lowerPanel.Selected}");
        if(_data.CurrentMenu != _lowerPanel.Selected)
        {
            _lowerPanel.SetSelected(_data.CurrentMenu, needCallAction: false);
            _overlayAnimator.AnimateTranzition(()=> { OpenCurrentMenuWindow(); });
           
        }
    }


    private WindowPresenter SetForClose(bool asSingle, Action closeCallback, WindowPresenter presenter)
    {
        if (asSingle)
        {
            _hierarchyCounter.ForceDrop();

            //deactivate other presenters logic
            foreach (var handler in _data.AllPresenters)
            {
                if (handler.IsActive) handler.SetActive(false);
            }

            HideAllSlidersViews();
            HideAllWindowsViews();

        }
        else
        {
            _hierarchyCounter.AddOpened(presenter.GetView(), presenter, closeCallback);
        }


        return presenter;
    }

    private WindowPresenter GetOrCreatePresenterByType(Type presenterType, WindowType windowType = WindowType.Empty, SliderPanelType sliderType = SliderPanelType.Empty)
    {
        WindowPresenter presenter;
        if (!_data.TryGetPresenterFromCreated(presenterType, out presenter))
        {
            print($"Create {presenterType}");

            if (windowType != WindowType.Empty)
                presenter = _factory.Create(windowType);

            if (sliderType != SliderPanelType.Empty)
                presenter = _factory.Create(sliderType);

            if (presenter != null)
            {
                _data.RegisterPresenter(presenterType, presenter);
            }
        }

        return presenter;
    }

}

