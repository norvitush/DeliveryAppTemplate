using UnityEngine;

public interface IView
{
    bool IsActive { get; }
    void SetActive(bool value);
    GameObject gameObject { get; }

    void Init(WindowPresenter presenter);
};