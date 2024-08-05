using System;
using UnityEngine;

public class OrdersWindowPresenter : WindowPresenter
{    
    private IOrdersWindowView View => (IOrdersWindowView)_view;
    private IOrdersDataProvider Model => (IOrdersDataProvider)_model;

    public OrdersWindowPresenter(IOrdersWindowView view, IOrdersDataProvider model, IWindowsDirector windowsDirector) : base(view, model, windowsDirector) { }

    protected override void Init()
    {
        
    }

    protected override void OnDisable()
    {
        Model.OnChange -= Model_OnChange;

    }

    protected override void OnEnable()
    {
        View.SetSelectedOrder(Model.SelectedOrder);
        Model.OnChange += Model_OnChange;
    }

    private void Model_OnChange()
    {
        View.SetSelectedOrder(Model.SelectedOrder);
    }

    internal void OnPressOrder(int id)
    {
        Model.SetSelectedOrder(id);
        View.SetSelectedOrder(id);
        SetActive(false);
        _windowsDirector.OpenWindow(WindowType.OrderDetails, asRootWindow: false, OnBackFromDetailes);
    }

    public void OnBackFromDetailes()
    {
        SetActive(true);
    }
}
