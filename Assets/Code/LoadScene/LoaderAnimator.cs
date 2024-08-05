using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LoaderAnimator : MonoBehaviour
{
    [SerializeField] private CanvasGroup _logo;

    public void Play(Action onEndAnimation)
    {
        Sequence sq = DOTween.Sequence();
        _logo.alpha = 0;
        sq.Append(_logo.DOFade(1, 0.8f));
        sq.Insert(0, _logo.transform.DOScale(1.05f, 2f));

        sq.OnComplete(() => { onEndAnimation?.Invoke(); });
    }
}
