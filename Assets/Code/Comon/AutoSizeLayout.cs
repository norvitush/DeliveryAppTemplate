using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class AutoSizeLayout : MonoBehaviour
{
    private const string NON_LAYOUT = "NotInLayout";
    public bool isLoopUpdate; //Is need to update layout in void Update

    public bool dontTouchChildren = false; //Is need to move the children

    public enum States
    {
        VerticalTop,
        VerticalCenter,
        VerticalBottom,
        HorizontalLeft,
        HorizontalCenter,
        HorizontalRight
    }

    public States typeLayout; //Type of the layout

    public bool isResizeSelf = true; //Is need to resize self

    public bool isUseAdditionalPadding = false; //Is need to use additional padding. For example for Vertical type,
                                                //the additional will be top pad and bottom pad and vice versa

    public float topPad; //Top padding
    public float bottomPad; //Bottom padding
    public float leftPad; //Left padding
    public float rightPad; //Right padding

    public float spacing; //Spacing between objects

    public int repeatFrames = 2; //How many frames it should update layout after first update

    public bool isHaveMinSizeX; //Is need to set min size of layout
    public bool isHaveMinSizeY;
    public Vector2 minSize; //Min size of layout
    public bool isHaveMaxSizeX; //Is need to set max size of layout
    public bool isHaveMaxSizeY;
    public Vector2 maxSize; //Max size of layout

    public float sizeTotal; // Total size of the object

    public RectTransform minSizeTargetRect; //If set then min size of layout will be the same as the size of target rect

    public bool isInverted; // By default - if you set a tag NotInLayout to a child object - it wont be calculated
                            // But if isInverted sets to true - it will calculate only children with the tag NotInLayout
    private RectTransform selfRect
    {
        get
        {
            if (_selfRect == null)
                _selfRect = (RectTransform)transform;
            return _selfRect;
        }
    }
    private RectTransform _selfRect;
    private Vector2 anch05_0 = new Vector2(0.5f, 0);
    private Vector2 anch05_1 = new Vector2(0.5f, 1);
    private Vector2 anch0_05 = new Vector2(0, 0.5f);
    private Vector2 anch1_05 = new Vector2(1f, 0.5f);

    Coroutine updateRoutine;

    private void Awake()
    {
        _selfRect = (RectTransform)transform;
    }

    void Update()
    {
        if (isLoopUpdate)
        {
            UpdateLayout(false);
        }
    }

    /// <summary>
    /// Update layout method
    /// </summary>
    /// <param name="isRepeat">Is need to repeat update "repeatFrames" frames</param>
    /// <param name="isRecursive">Is need to launch this method in each child component</param>
    public void UpdateLayout(bool isRepeat = true, bool isRecursive = false)
    {
        UpdateAllRect(isRecursive);
        if (isRepeat)
        {
            if (updateRoutine != null)
            {
                StopCoroutine(updateRoutine);
            }
            if (gameObject.activeInHierarchy)
            {
                updateRoutine = StartCoroutine(UpdateRepeate(isRecursive));
            }
        }
    }

    private bool IsNeedIgnore(Transform childTransform)
    {
        // return childTransform.CompareTag(NON_LAYOUT);
        if(childTransform.TryGetComponent<LayoutElement>(out var layoutElement))
        {
            return layoutElement.ignoreLayout;
        }

        return false;
    }

    void UpdateAllRect(bool isRecursive)
    {
        if (minSizeTargetRect != null)
        {
            minSize = minSizeTargetRect.rect.size;
        }

        bool isResize = false;
        RectTransform rect;
        int childCount = transform.childCount;
        Transform childTransform;
        

        switch (typeLayout)
        {
            case States.VerticalTop:
                sizeTotal = topPad;

                for (int i = 0; i < childCount; i++)
                {
                    childTransform = transform.GetChild(i);
                    if ((isInverted ? IsNeedIgnore(childTransform) : !IsNeedIgnore(childTransform))
                        && childTransform.gameObject.activeSelf)
                    {
                        rect = (RectTransform)childTransform;

                        if (isRecursive)
                        {
                            if (rect.TryGetComponent(out AutoSizeLayout autoSizeLayout))
                            {
                                autoSizeLayout.UpdateLayout(isRecursive: true);
                            }
                        }
                        if (!dontTouchChildren && !(rect.TryGetComponent(out LayoutElement layoutElement) && layoutElement.ignoreLayout))
                        {
                            rect.anchorMax = anch05_1;
                            rect.anchorMin = anch05_1;
                            rect.pivot = new Vector2(rect.pivot.x, 1);
                            rect.anchoredPosition = new Vector2((isUseAdditionalPadding ? leftPad - rightPad : rect.anchoredPosition.x),
                                -rect.sizeDelta.y * rect.localScale.y * (1 - rect.pivot.y) - sizeTotal);
                        }
                        sizeTotal += rect.sizeDelta.y * rect.localScale.y + spacing;
                        isResize = true;
                    }
                }
                if (isResize)
                {
                    sizeTotal -= spacing;
                }
                sizeTotal += bottomPad;
                if (isResizeSelf)
                {
                    selfRect.sizeDelta = new Vector2(selfRect.sizeDelta.x,
                        Mathf.Clamp(sizeTotal, isHaveMinSizeY ? minSize.y : float.MinValue, isHaveMaxSizeY ? maxSize.y : float.MaxValue));
                }
                break;
            case States.VerticalBottom:
                sizeTotal = bottomPad;
                for (int i = 0; i < childCount; i++)
                {
                    childTransform = transform.GetChild(i);
                    if ((isInverted ? IsNeedIgnore(childTransform) : !IsNeedIgnore(childTransform))
                        && childTransform.gameObject.activeSelf)
                    {
                        rect = (RectTransform)childTransform;
                        if (isRecursive)
                        {
                            if (rect.TryGetComponent(out AutoSizeLayout autoSizeLayout))
                            {
                                autoSizeLayout.UpdateLayout(isRecursive: true);
                            }
                        }
                        if (!dontTouchChildren && !(rect.TryGetComponent(out LayoutElement layoutElement) && layoutElement.ignoreLayout))
                        {
                            rect.anchorMax = anch05_0;
                            rect.anchorMin = anch05_0;
                            rect.pivot = new Vector2(rect.pivot.x, 0);
                            rect.anchoredPosition = new Vector2((isUseAdditionalPadding ? leftPad - rightPad : rect.anchoredPosition.x),
                                rect.sizeDelta.y * rect.localScale.y * (rect.pivot.y) + sizeTotal);
                        }
                        sizeTotal += rect.sizeDelta.y * rect.localScale.y + spacing;
                        isResize = true;
                    }
                }
                if (isResize)
                {
                    sizeTotal -= spacing;
                }
                sizeTotal += topPad;
                if (isResizeSelf)
                {
                    selfRect.sizeDelta = new Vector2(selfRect.sizeDelta.x,
                        Mathf.Clamp(sizeTotal, isHaveMinSizeY ? minSize.y : float.MinValue, isHaveMaxSizeY ? maxSize.y : float.MaxValue));
                }
                break;
            case States.HorizontalLeft:
                sizeTotal = leftPad;
                for (int i = 0; i < childCount; i++)
                {
                    childTransform = transform.GetChild(i);

                    if ((isInverted ? IsNeedIgnore(childTransform) : !IsNeedIgnore(childTransform))
                        && childTransform.gameObject.activeSelf)
                    {
                        rect = (RectTransform)childTransform;
                        if (isRecursive)
                        {
                            if (rect.TryGetComponent(out AutoSizeLayout autoSizeLayout))
                            {
                                autoSizeLayout.UpdateLayout(isRecursive: true);
                            }
                        }
                        if (!dontTouchChildren && !(rect.TryGetComponent(out LayoutElement layoutElement) && layoutElement.ignoreLayout))
                        {
                            rect.anchorMax = anch0_05;
                            rect.anchorMin = anch0_05;
                            rect.pivot = new Vector2(0, rect.pivot.y);
                            rect.anchoredPosition = new Vector2(rect.sizeDelta.x * rect.localScale.x * (rect.pivot.x) + sizeTotal,
                                (isUseAdditionalPadding ? bottomPad - topPad : rect.anchoredPosition.y));
                        }
                        sizeTotal += rect.sizeDelta.x * rect.localScale.x + spacing;
                        isResize = true;
                    }
                }
                if (isResize)
                {
                    sizeTotal -= spacing;
                }
                sizeTotal += rightPad;
                if (isResizeSelf)
                {
                    selfRect.sizeDelta = new Vector2(Mathf.Clamp(sizeTotal, isHaveMinSizeX ? minSize.x : float.MinValue,
                        isHaveMaxSizeX ? maxSize.x : float.MaxValue), selfRect.sizeDelta.y);
                }
                break;
            case States.HorizontalRight:
                sizeTotal = rightPad;
                for (int i = 0; i < childCount; i++)
                {
                    childTransform = transform.GetChild(i);

                    if ((isInverted ? IsNeedIgnore(childTransform) : !IsNeedIgnore(childTransform))
                        && childTransform.gameObject.activeSelf)
                    {
                        rect = (RectTransform)childTransform;
                        if (isRecursive)
                        {
                            if (rect.TryGetComponent(out AutoSizeLayout autoSizeLayout))
                            {
                                autoSizeLayout.UpdateLayout(isRecursive: true);
                            }
                        }
                        if (!dontTouchChildren && !(rect.TryGetComponent(out LayoutElement layoutElement) && layoutElement.ignoreLayout))
                        {
                            rect.anchorMax = anch1_05;
                            rect.anchorMin = anch1_05;
                            rect.pivot = new Vector2(1, rect.pivot.y);
                            rect.anchoredPosition = new Vector2(-rect.sizeDelta.x * rect.localScale.x * (1 - rect.pivot.x) - sizeTotal,
                                (isUseAdditionalPadding ? bottomPad - topPad : rect.anchoredPosition.y));
                        }
                        sizeTotal += rect.sizeDelta.x * rect.localScale.x + spacing;
                        isResize = true;
                    }
                }
                if (isResize)
                {
                    sizeTotal -= spacing;
                }
                sizeTotal += leftPad;
                if (isResizeSelf)
                {
                    selfRect.sizeDelta = new Vector2(Mathf.Clamp(sizeTotal, isHaveMinSizeX ? minSize.x : float.MinValue,
                        isHaveMaxSizeX ? maxSize.x : float.MaxValue), selfRect.sizeDelta.y);
                }
                break;
            case States.HorizontalCenter:
                float startSize = -leftPad;
                sizeTotal = leftPad;
                for (int i = 0; i < childCount; i++)
                {
                    childTransform = transform.GetChild(i);

                    if ((isInverted ? IsNeedIgnore(childTransform) : !IsNeedIgnore(childTransform))
                        && transform.GetChild(i).gameObject.activeSelf)
                    {
                        rect = (RectTransform)childTransform;
                        if (isRecursive)
                        {
                            if (rect.TryGetComponent(out AutoSizeLayout autoSizeLayout))
                            {
                                autoSizeLayout.UpdateLayout(isRecursive: true);
                            }
                        }
                        if (!dontTouchChildren && !(rect.TryGetComponent(out LayoutElement layoutElement) && layoutElement.ignoreLayout))
                        {
                            rect.anchorMax = anch0_05;
                            rect.anchorMin = anch0_05;
                            rect.pivot = new Vector2(0.5f, rect.pivot.y);
                        }
                        startSize += rect.sizeDelta.x * rect.localScale.x + spacing;
                    }
                }

                startSize -= spacing;
                startSize += rightPad;
                //var rectSelf = GetComponent<RectTransform>().rect;
                sizeTotal = selfRect.rect.width / 2 - startSize / 2;
                for (int i = 0; i < childCount; i++)
                {
                    childTransform = transform.GetChild(i);

                    if ((isInverted ? IsNeedIgnore(childTransform) : !IsNeedIgnore(childTransform))
                        && childTransform.gameObject.activeSelf)
                    {
                        rect = (RectTransform)childTransform;
                        if (!dontTouchChildren && !(rect.TryGetComponent(out LayoutElement layoutElement) && layoutElement.ignoreLayout))
                        {
                            rect.anchoredPosition = new Vector2(rect.sizeDelta.x * rect.localScale.x * (1 - rect.pivot.x) + sizeTotal,
                                 (isUseAdditionalPadding ? bottomPad - topPad : rect.anchoredPosition.y));
                        }
                        sizeTotal += rect.sizeDelta.x * rect.localScale.x + spacing;
                        isResize = true;
                    }
                }
                if (isResize)
                {
                    sizeTotal -= spacing;
                }
                sizeTotal += rightPad;
                if (isResizeSelf)
                {
                    selfRect.sizeDelta = new Vector2(Mathf.Clamp(sizeTotal, isHaveMinSizeX ? minSize.x : float.MinValue,
                        isHaveMaxSizeX ? maxSize.x : float.MaxValue), selfRect.sizeDelta.y);
                }
                break;
            case States.VerticalCenter:
                float startSizeVertical = -topPad;
                sizeTotal = topPad;
                for (int i = 0; i < childCount; i++)
                {
                    childTransform = transform.GetChild(i);

                    if ((isInverted ? IsNeedIgnore(childTransform) : !IsNeedIgnore(childTransform))
                        && childTransform.gameObject.activeSelf)
                    {
                        rect = (RectTransform)childTransform;
                        if (isRecursive)
                        {
                            if (rect.TryGetComponent(out AutoSizeLayout autoSizeLayout))
                            {
                                autoSizeLayout.UpdateLayout(isRecursive: true);
                            }
                        }
                        if (!dontTouchChildren && !(rect.TryGetComponent(out LayoutElement layoutElement) && layoutElement.ignoreLayout))
                        {
                            rect.anchorMax = anch05_1;
                            rect.anchorMin = anch05_1;
                            rect.pivot = new Vector2(rect.pivot.x, 0.5f);
                        }
                        startSizeVertical += rect.sizeDelta.y * rect.localScale.y + spacing;
                    }
                }
                startSizeVertical -= spacing;
                startSizeVertical += bottomPad;
                //var rectSelfVertical = GetComponent<RectTransform>().rect;
                sizeTotal = selfRect.rect.height / 2 - startSizeVertical / 2;
                for (int i = 0; i < childCount; i++)
                {
                    childTransform = transform.GetChild(i);

                    if ((isInverted ? IsNeedIgnore(childTransform) : !IsNeedIgnore(childTransform))
                        && childTransform.gameObject.activeSelf)
                    {
                        rect = (RectTransform)childTransform;
                        if (!dontTouchChildren && !(rect.TryGetComponent(out LayoutElement layoutElement) && layoutElement.ignoreLayout))
                        {
                            rect.anchoredPosition = new Vector2((isUseAdditionalPadding ? leftPad - rightPad : rect.anchoredPosition.x),
                                -rect.sizeDelta.y * rect.localScale.y * (1 - rect.pivot.y) - sizeTotal);
                        }
                        sizeTotal += rect.sizeDelta.y * rect.localScale.y + spacing;
                        isResize = true;
                    }
                }
                if (isResize)
                {
                    sizeTotal -= spacing;
                }
                sizeTotal += bottomPad;
                if (isResizeSelf)
                {
                    selfRect.sizeDelta = new Vector2(selfRect.sizeDelta.x,
                        Mathf.Clamp(sizeTotal, isHaveMinSizeY ? minSize.y : float.MinValue, isHaveMaxSizeY ? maxSize.y : float.MaxValue));
                }
                break;
        }
    }

    private IEnumerator UpdateRepeate(bool isRecursive)
    {
        for (int i = 0; i < repeatFrames; i++)
        {
            yield return new WaitForEndOfFrame();
            UpdateAllRect(isRecursive);
        }
    }
}
