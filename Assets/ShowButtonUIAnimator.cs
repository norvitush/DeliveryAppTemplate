using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class ShowButtonUIAnimator : MonoBehaviour
{
    [SerializeField] private CanvasGroup _object;
    [SerializeField, Range(0, 1f)] private float _startAlpha = 1f;
    
    Tween _currentAction;
    private void Awake()
    {
        _object = GetComponent<CanvasGroup>();
        _object.alpha = _startAlpha;
    }
    public void Show(float maxAlpha01 = 1, float time =0.35f)
    {
        if (_currentAction.IsActive()) _currentAction.Kill(complete: true);
        _currentAction = _object.DOFade(maxAlpha01, time);
    }

    public void Hide(float minAlpha01 = 0, float time = 0.2f)
    {
        if (_currentAction.IsActive()) _currentAction.Kill(complete: true);
        _currentAction = _object.DOFade(minAlpha01, time);
    }
}
