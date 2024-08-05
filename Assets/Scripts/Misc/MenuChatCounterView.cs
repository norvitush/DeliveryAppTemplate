using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class CounterView : MonoBehaviour, ICounterView
{
    public bool IsActive { get; protected set; }

    public abstract void SetActive(bool value);

    public abstract void SetCounter(int count, bool visible);    
}

public class MenuChatCounterView : CounterView
{
    [SerializeField] private TextMeshProUGUI _lable;

    public override void SetActive(bool value)
    {
        if (!value)
        {
            gameObject.SetActive(false);
            IsActive = false;
            return;
        }

        gameObject.SetActive(true);
        IsActive = true;
    }

    public override void SetCounter(int count, bool visible)
    {
        SetActive(visible);

        _lable.text = count > 99 ? "+99" : count.ToString();
    }
}
