using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetOrderWindowView : MonoBehaviour, IGetOrderView
{
    private GetOrderWindowPresenter _presenter;
    public bool IsActive => gameObject.activeInHierarchy;

    public void Init(WindowPresenter presenter)
    {
        _presenter = (GetOrderWindowPresenter)presenter;
    }

    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }

    public void OnClickBack()
    {
        _presenter.OnClickBack();
    }
}
