using System;
using UnityEngine;

public abstract class LowerPanelView : MonoBehaviour, IView
{
    protected WindowPresenter _presenter;
    public abstract MenuButtonType Selected { get; }

    public abstract bool IsActive { get; }

    public event Action<MenuButtonType> OnSelectMenu;

    protected virtual void Publish(MenuButtonType selected) => OnSelectMenu?.Invoke(selected);

    public abstract void Show();
    public abstract void Hide();

    public abstract void SetCounterLableValue(int addedElements = 0);

    public abstract void SetSelected(string menuButton);

    public abstract void SetSelected(MenuButtonType button, bool needCallAction = true);

    public abstract void SetActive(bool value);

    public void Init(WindowPresenter presenter)
    {
        _presenter = presenter;
    }
}
