using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfileWindowView : MonoBehaviour, IProfileWindowView
{
    private ProfileWindowPresenter _presenter;
    public bool IsActive => gameObject.activeInHierarchy;

    public void SetActive(bool value) => gameObject.SetActive(value);

    public void Init(WindowPresenter presenter) => _presenter = (ProfileWindowPresenter)presenter;

    public void OnClickNotifications()
    {
        _presenter.OpenNotifications();
    }
}
