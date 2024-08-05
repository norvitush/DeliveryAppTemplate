using System.Collections;
using UnityEngine;

public class SlotDetailsView : MonoBehaviour, ISlotDetailsView
{
    private SlotDetailsWindowPresenter _presenter;
    public bool IsActive => gameObject.activeInHierarchy;

    public void Init(WindowPresenter presenter)
    {
        _presenter = (SlotDetailsWindowPresenter)presenter;
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
