using System;
using UnityEngine;
public abstract class TopPanelView: MonoBehaviour
{    

    public event Action OnPressBack;
    protected virtual void Publish(MenuButtonType selected) => OnPressBack?.Invoke();

    public abstract void Show();
    public abstract void Hide();

    public abstract void SetLableValue(string title, bool hasBackArrow);

    public abstract void SetButtons(TopPanelIco[] icons);
}
