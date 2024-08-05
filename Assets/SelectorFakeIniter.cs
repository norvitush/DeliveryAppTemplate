using System.Collections.Generic;
using UnityEngine;

public class SelectorFakeIniter: MonoBehaviour
{
    [SerializeField] AlterMassSelector _massSelector;

    private void OnEnable()
    {
        if (_massSelector != null)
        {
            _massSelector.Init(new List<string> { "В ожидании", "В доставке", "Завершенные" }, (val) => { print($"{val}"); });
        }
    }
}
