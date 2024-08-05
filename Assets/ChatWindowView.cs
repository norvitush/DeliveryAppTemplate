using UnityEngine;

public class ChatWindowView : MonoBehaviour, IChatWindowView
{
    private ChatWindowPresenter _presenter;
    public bool IsActive => gameObject.activeInHierarchy;

    public void SetActive(bool value) => gameObject.SetActive(value);

    public void Init(WindowPresenter presenter) => _presenter = (ChatWindowPresenter)presenter;
}
