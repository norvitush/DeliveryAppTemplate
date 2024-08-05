using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderDetailesView : MonoBehaviour, IOrderDetailesView
{
    private OrderDetailesWindowPresenter _presenter;
    public bool IsActive => gameObject.activeInHierarchy;

    public void SetActive(bool value) => gameObject.SetActive(value);

    public void OnClickBack()
    {
        _presenter.OnClickBack();
    }
    public void OnClickGetOrder()
    {
        _presenter.OnClickGetOrder();
    }

    public void OnClickCancelOrder()
    {
        _presenter.OnClickCancelOrder();
    }

    public void Init(WindowPresenter presenter)
    {
        _presenter = (OrderDetailesWindowPresenter)presenter;
    }
}
