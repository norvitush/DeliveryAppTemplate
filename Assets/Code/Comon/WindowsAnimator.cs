using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using System.Diagnostics;

public class WindowsAnimator : MonoBehaviour, IOverlayAnimator
{
    [SerializeField] private CanvasGroup _canvasGroup;

    private Sequence _tranzitionHandler;

    public event Action OnCompleteTranzit;

    public void AnimateTranzition(Action callbackOnHide)
    {

        if (_tranzitionHandler != null && _tranzitionHandler.IsPlaying()) return;

        _canvasGroup.gameObject.SetActive(true);
        StopAllCoroutines();
        _canvasGroup.alpha = 0;    
        StartCoroutine(WaitingFirstUpdate(callbackOnHide));
    }

    private IEnumerator WaitingFirstUpdate(Action callbackOnHide)
    {
        yield return null;
        _tranzitionHandler = DOTween.Sequence();
        _tranzitionHandler.OnKill(() => { _tranzitionHandler = null; });
        _canvasGroup.alpha = 1;
        _tranzitionHandler
            .Append(_canvasGroup.DOFade(1, 0.15f).SetEase(Ease.OutCubic).OnComplete(()=> { callbackOnHide?.Invoke(); }))
            .Append(_canvasGroup.DOFade(0, 0.20f).SetEase(Ease.InCubic));

        
        yield return _tranzitionHandler.WaitForStart();
        yield return _tranzitionHandler.WaitForCompletion();

        OnCompleteTranzit?.Invoke(); 
        _canvasGroup.gameObject.SetActive(false);
    }

}
