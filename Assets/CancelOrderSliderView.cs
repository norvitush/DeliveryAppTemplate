using System;
using System.Collections;
using UnityEngine;

public class CancelOrderSliderView : MonoBehaviour, ICancelOrderView
{
    [SerializeField] ExpandingPanelMovement _panel;

    private CancelOrderPresenter _presenter;

    public SliderState CurrentState => (SliderState)_panel.CurrentState;
    
    public bool IsActive => gameObject.activeInHierarchy;    

    public void Init(WindowPresenter presenter)
    {
        _presenter = (CancelOrderPresenter)presenter;        
    }

    private void OnEnable()
    {
        _panel.OnClosePanel += OnDragClose;        
    }
    private void OnDisable()
    {        
        _panel.OnClosePanel -= OnDragClose;
        _presenter.OnViewDisable();
    }

    public void Open(SliderState startState = SliderState.Middle, bool needIgnoreMiddlePosition = false)
    {
        if (CurrentState != SliderState.Closed) return;
        if (gameObject.activeSelf == false) gameObject.SetActive(true);
        _panel.Open(startState, _panel.needIgnoreMiddlePosition);
    }
    
    public void Close()
    {
        if (CurrentState != SliderState.Closed)
        {
            _panel.Close();
        }
    }

    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }
        
    private void OnDragClose()
    {
        gameObject.SetActive(false);
    }
}
