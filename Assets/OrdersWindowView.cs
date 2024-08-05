using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrdersWindowView : MonoBehaviour, IOrdersWindowView, IPressResolverByID
{
    private OrdersWindowPresenter _presenter;
    
    [System.Obsolete, SerializeField] private OrderDetailesView _detailes;

    public bool IsActive => gameObject.activeInHierarchy;

    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }
    public void Init(WindowPresenter presenter)
    {
        _presenter = (OrdersWindowPresenter)presenter;
    }

    public void OnClickOrder()
    {
        PressItem(0);
    }

    public void PressItem(int id)
    {
        _presenter.OnPressOrder(id);
    }

    public void SetSelectedOrder(int id)
    {
        print($"Select {id}");
    }
}

public interface IOrdersWindowView: IView
{
    void SetSelectedOrder(int id);
}