using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationsWindowView : MonoBehaviour, INotificationsView
{
    private NotificationWindowPresenter _presenter;
    public bool IsActive => gameObject.activeInHierarchy;
    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }

    public void Init(WindowPresenter presenter)
    {
        _presenter = (NotificationWindowPresenter)presenter;
    }
    public void OnClickBack()
    {
        _presenter.OnClickBack();
    }
}
