using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class ExpandingPanelMovement : MonoBehaviour
{
    private const float TRASHOLD_Y_CLOSE = 0.5f;
    public ScrollAdvanced scrollScript;
    public RectTransform contentRect;
    public AutoSizeLayout contentAutoSize;
    public RectTransform boxPanelRect;
    //public WebViewScroll webScroll = null;
    public Animator animator;
    RectTransform canvasRect;

    private SliderState currentState;
    public int CurrentState {
        get => (int)currentState;
        private set { currentState = (SliderState)value; }    
    }

    public float startPos = 0;
    public float upperPos = 45;
    public float middlePos = 0;
    public float middlePosRate = .5f;
    public float noCosableMinHeight = 200;

    public float targetPos;

    public float speed = 10;

    public float minPad = 10;

    public bool isCanClose;
    public bool isMiddlePosAsRate;

    public RectTransform[] scrollsContent;

    public event Action OnClosePanel;
    public event Action OnOpenPanel;
    public event Action OnStartDragPanel;
    public event Action OnEndDragPanel;

    public float ContentBottomOffset = 100f;

    public bool needIgnoreMiddlePosition = false;

    public bool IsBoxStaticSize = false;

    private void Start ()
    {
        LazyInitCanvas();
        //print(canvasRect.sizeDelta);
        RecalcRects();
        scrollScript.onStartDrag += OnDragStart;
        scrollScript.onStopDrag += OnDragOver;
    }

    private void RecalcRects()
    {
        if (isMiddlePosAsRate) middlePos = (canvasRect.sizeDelta.y * middlePosRate) - (isCanClose ? 0 : noCosableMinHeight);
        if (IsBoxStaticSize)
        {
            upperPos = canvasRect.sizeDelta.y - boxPanelRect.sizeDelta.y + 35f;
        }
        else
        {
            boxPanelRect.sizeDelta = new Vector2(boxPanelRect.sizeDelta.x, canvasRect.sizeDelta.y - upperPos);
        }
        contentAutoSize.topPad = canvasRect.sizeDelta.y - (isCanClose ? 0 : noCosableMinHeight);
    }

    bool isScrolling;
    bool wasGotAnimatorCloseCallback;
    private void LazyInitCanvas()
    {
        if(canvasRect == null)
        {
            var canvas = contentRect.root.GetComponent<Canvas>();
            if (canvas != null) canvasRect = (RectTransform)canvas.transform;
        }
    }
    private void Update () {
        
        for (int i = 0; i < scrollsContent.Length; i++) {
            scrollsContent[i].sizeDelta = new Vector2(scrollsContent[i].sizeDelta.x, contentRect.anchoredPosition.y - ContentBottomOffset + scrollsContent[i].anchoredPosition.y);
        }
        
        if (isScrolling ) {
            return;
        }

        contentRect.anchoredPosition = new Vector2(0, Mathf.Lerp(contentRect.anchoredPosition.y, targetPos, speed * Time.deltaTime));

        if (wasGotAnimatorCloseCallback  && contentRect.anchoredPosition.y < targetPos + TRASHOLD_Y_CLOSE) 
        {
            gameObject.SetActive(false);
            if (animator == null)
            {
                OnClosePanel?.Invoke();
            }
        }
    }

    public void OnDragStart () {
      //  //print("Drag start " + scrollScript.targetPos.y + " " + CurrentState + " " + targetPos);
        isScrolling = true;
        OnStartDragPanel?.Invoke();
    }

    public void OnDragOver () {
        
       // //print($"Drag over scrollScript.targetPos.y:" + scrollScript.targetPos.y + " currentState:" + CurrentState + " targetPos:" + targetPos);
        switch (currentState) {
            case SliderState.Middle:
                if (scrollScript.targetPos.y < targetPos - minPad) {
                    //if (isCanClose) {
                    //    Close();
                    //}
                     Close();
                } else if (scrollScript.targetPos.y > targetPos + minPad) {
                    OpenFull();
                }
                break;
            case SliderState.FullOpened:
                if (scrollScript.targetPos.y < targetPos - minPad) {
                    if (needIgnoreMiddlePosition)
                    
                        Close();
                    
                    else
                        CloseToMiddle();
                }
                break;
            case SliderState.Closed:
                if (scrollScript.targetPos.y > targetPos + minPad)
                {
                    Open(needIgnoreMiddlePosition: needIgnoreMiddlePosition);
                }
                break;
        }

        isScrolling = false;
        OnEndDragPanel?.Invoke();
    }

    public void CloseObject () 
    {
        //gameObject.SetActive(false);
        OnClosePanel?.Invoke();
        wasGotAnimatorCloseCallback = true;
    }

    public void Close () {

        CurrentState = 0;
        targetPos = 0;
     
        if(animator != null) {
            animator.SetTrigger("Close");
        }
        else
        {
            if(isCanClose) wasGotAnimatorCloseCallback = true;
        }
    }

    public void CloseToMiddle () {
        CurrentState = 1;
        targetPos = middlePos;
        //if (webScroll != null) {
        //    webScroll.UpdateRect();
        //}
    }

    public void OpenFull () {        
        LazyInitCanvas();
        CurrentState = 2;
        targetPos = Mathf.Clamp(contentAutoSize.topPad - upperPos, 0, contentRect.sizeDelta.y - canvasRect.sizeDelta.y);
        //if (webScroll != null) {
        //    webScroll.UpdateRect();
        //}
    }
    public void OpenMiddle()
    {
        Open();
    }
    public void Open (SliderState startState = SliderState.Middle, bool needIgnoreMiddlePosition = false) {
        //print($"Open Expanded panel...");
        LazyInitCanvas();
        if(gameObject.activeInHierarchy) RecalcRects();
        OnOpenPanel?.Invoke();
        
        this.needIgnoreMiddlePosition = needIgnoreMiddlePosition;

        for (int i = 0; i < scrollsContent.Length; i++) {
            scrollsContent[i].sizeDelta = new Vector2(scrollsContent[i].sizeDelta.x, 100);
            scrollsContent[i].GetComponent<ScrollAdvanced>().SetPosition(new Vector2(0, 0));
        }
        wasGotAnimatorCloseCallback = false;
        currentState = startState;
        contentRect.anchoredPosition = new Vector2(0, startPos);
        
        if(startState == SliderState.Middle)
            targetPos = middlePos;

        if (needIgnoreMiddlePosition || startState == SliderState.FullOpened)
            targetPos = Mathf.Clamp(canvasRect.sizeDelta.y - upperPos, 0, canvasRect.sizeDelta.y);

        //if (webScroll != null) {
        //    webScroll.UpdateRect();
        //}
    }
}
