using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScrollAdvanced : MonoBehaviour, 
    IBeginDragHandler, 
    IDragHandler, 
    IEndDragHandler,
    IPointerDownEventer,
    IPointerUpEventer, IPointerStopableScroll
{
    public Action<PointerEventData> onStartScrolling;
    public Action<PointerEventData> onStopScrolling;
    public Action<PointerEventData> onScrolling;
    public event Action<bool> OnActiveStateChange;

    public Action onStartDrag;
    public Action onStopDrag;
    public Action onStopMovement;
    public RectTransform viewport;
    public RectTransform content;

    public bool isHorizontal;
    public bool isVertical = true;

    public enum MovementTypeEnum {
        Elastic,
        Clamped,
        Unrestricted
    }

    public MovementTypeEnum movementType;

    public bool isUseInertia = true;
    public float inertia = 0.01f;
    public float inertiaFade = 3;

    public float inertiaLowerValue = 0.1f;

    public float elasity = 50;
    public float maxElasitySence = 1;

    public float lerpSpeed = 10;

    public ScrollAdvanced parentScroll;
    public bool isParentScroll = true;

    public bool isStartDrag;

    public Vector2 inertiaVector;
    public float maxInertia = 80;

    public bool isDebug = false;



    public bool isClonedObj;
    public RectTransform clonedRect;
    public ScrollAdvanced targetScroll;
    public Vector2 targetPos;

    public bool isCanStartDrag = true;

    private bool needStop = false;
    private bool isSentStopMovement;
    private Vector2 firstPositionMouse;
    private Vector2 firstPosition;
    private Vector2 currentPosition;
    private Vector2 clampedPos;
    private new Camera camera;
    private RectTransform canvas;

    public event Action<PointerEventData> OnPointerDownInvoked;
    public event Action<PointerEventData> OnPointerUpInvoked;
    bool IPointerStopableScroll.IsNeedStop => needStop;

    private void Start() {

        camera = Camera.main;

        var lCanvas = this.transform.root.GetComponent<Canvas>();
        if (lCanvas != null && lCanvas.TryGetComponent(out RectTransform transform))
        {
            canvas = transform;
        }           
    }
    
    void Update() {

        if (isClonedObj) {
            clonedRect.anchoredPosition = content.anchoredPosition;
            return;
        }

        if (isParentScroll && needStop)
        {
            return;
        }

        if (isStartDrag) {
            MoveScroll();
        } else{
            var xPos = (content.rect.width * content.localScale.x > viewport.rect.width ?
                viewport.rect.width * (1 - content.anchorMin.x) * (1 - content.anchorMax.x)
                - content.rect.width * content.localScale.x * (1 - content.pivot.x) : 0);
            var yPos = (content.rect.height * content.localScale.x > viewport.rect.height ? 
                viewport.rect.height * (1 - content.anchorMin.y) * (1 - content.anchorMax.y)
                - content.rect.height * content.localScale.x * (1 - content.pivot.y) : 0);

            clampedPos = new Vector2(isHorizontal ?
                        Mathf.Clamp(content.anchoredPosition.x, xPos, xPos + Mathf.Clamp(content.rect.width * content.localScale.x - viewport.rect.width, 0,
                        float.MaxValue))
                        : content.anchoredPosition.x, isVertical ?
                        Mathf.Clamp(content.anchoredPosition.y, yPos, yPos + Mathf.Clamp(content.rect.height * content.localScale.x - viewport.rect.height, 0,
                        float.MaxValue))
                        : content.anchoredPosition.y);
            if (movementType == MovementTypeEnum.Elastic && !isUseInertia) {
                content.anchoredPosition = new Vector3(Mathf.Lerp(content.anchoredPosition.x, clampedPos.x, lerpSpeed * Time.deltaTime),
                    Mathf.Lerp(content.anchoredPosition.y, clampedPos.y, lerpSpeed * Time.deltaTime));
            }
            else if(movementType == MovementTypeEnum.Clamped) {
                content.anchoredPosition = clampedPos;
            }
            if (isUseInertia) {
                if (movementType == MovementTypeEnum.Unrestricted) {
                    inertiaVector = new Vector3(Mathf.Lerp(inertiaVector.x, 0, inertiaFade * Time.deltaTime),
                        Mathf.Lerp(inertiaVector.y, 0, inertiaFade * Time.deltaTime));
                    content.anchoredPosition += inertiaVector;

                    if ((Mathf.Abs(inertiaVector.x) + Mathf.Abs(inertiaVector.y)) < inertiaLowerValue && !isSentStopMovement) {
                        isSentStopMovement = true;
                        if (onStopMovement != null) {
                            onStopMovement();
                        }
                    }
                    return;
                }
                if(movementType == MovementTypeEnum.Elastic) {
                    float ratio = 1;

                    if (targetPos.x < xPos && inertiaVector.x != 0) {
                        inertiaVector.x = Mathf.Clamp(inertiaVector.x, -elasity, 0);
                    }

                    if (targetPos.x >
                        xPos + content.rect.width * content.localScale.x - viewport.rect.width && inertiaVector.x != 0) {
                        inertiaVector.x = Mathf.Clamp(inertiaVector.x, 0, elasity);
                    }

                    if (targetPos.y < yPos && inertiaVector.y != 0) {
                        inertiaVector.y = Mathf.Clamp(inertiaVector.y, -elasity, 0);
                    }

                    if (targetPos.y >
                        yPos + content.rect.height * content.localScale.x - viewport.rect.height && inertiaVector.y != 0) {
                        inertiaVector.y = Mathf.Clamp(inertiaVector.y, 0, elasity);
                    }


                    if (targetPos.x + inertiaVector.x < xPos && inertiaVector.x != 0) {
                        ratio = Mathf.Clamp01(1 - Mathf.Abs(xPos - targetPos.x) / elasity);
                    }

                    if (targetPos.x + inertiaVector.x >
                        xPos + content.rect.width * content.localScale.x - viewport.rect.width && inertiaVector.x != 0) {
                        ratio = Mathf.Clamp01(1 - Mathf.Abs(targetPos.x - (xPos + 
                            Mathf.Clamp(content.rect.width * content.localScale.x - viewport.rect.width, 0, float.MaxValue))) / elasity);
                    }

                    if (targetPos.y + inertiaVector.y < yPos && inertiaVector.y != 0) {
                        ratio = Mathf.Clamp01(1 - (Mathf.Abs(yPos - targetPos.y) / elasity));
                    }

                    if (targetPos.y + inertiaVector.y >
                        yPos + content.rect.height * content.localScale.x - viewport.rect.height && inertiaVector.y != 0) {
                        ratio = Mathf.Clamp01(1 - Mathf.Abs(targetPos.y - (yPos + 
                            Mathf.Clamp(content.rect.height * content.localScale.x - viewport.rect.height, 0, float.MaxValue))) / elasity);
                    }

                    if (isDebug) {
                        //print(ratio + " " + targetPos + " " + inertiaVector);
                    }

                    targetPos = new Vector2((targetPos.x < xPos && isHorizontal) ?
                        targetPos.x + (inertiaVector.x < 0 ? Mathf.Clamp(inertiaVector.x, -maxElasitySence, 0) * Mathf.Clamp(1 - Mathf.Abs(xPos - targetPos.x) /
                           elasity, 0, 1) : inertiaVector.x) : (targetPos.x >= xPos + elasity + Mathf.Clamp(content.rect.width * content.localScale.x
                           - viewport.rect.width, 0, float.MaxValue)
                    && isHorizontal ? targetPos.x + (inertiaVector.x > 0 ? Mathf.Clamp(inertiaVector.x, 0, maxElasitySence) * Mathf.Clamp(1 - Mathf.Abs(targetPos.x -
                        (xPos + Mathf.Clamp(content.rect.width * content.localScale.x - viewport.rect.width, 0, float.MaxValue))) /
                           elasity, 0, 1) : inertiaVector.x) : isHorizontal ? targetPos.x + inertiaVector.x : targetPos.x),
                           targetPos.y < yPos && isVertical ?
                           targetPos.y + (inertiaVector.y < 0 ? Mathf.Clamp(inertiaVector.y, -maxElasitySence, 0) * Mathf.Clamp(1 - Mathf.Abs(yPos - targetPos.y) /
                           elasity, 0, 1) : inertiaVector.y) : (targetPos.y > yPos + Mathf.Clamp(content.rect.height * content.localScale.x
                           - viewport.rect.height, 0, float.MaxValue))
                    && isVertical ? targetPos.y + (inertiaVector.y > 0 ? Mathf.Clamp(inertiaVector.y, 0, maxElasitySence) * Mathf.Clamp(1 - Mathf.Abs(targetPos.y -
                        (yPos + Mathf.Clamp(content.rect.height * content.localScale.x - viewport.rect.height, 0, float.MaxValue))) /
                           elasity, 0, 1) : inertiaVector.y) : isVertical ? targetPos.y + inertiaVector.y : targetPos.y);

                    clampedPos = new Vector2(isHorizontal ?
                        Mathf.Clamp(targetPos.x, xPos, xPos + Mathf.Clamp(content.rect.width * content.localScale.x - viewport.rect.width, 0,
                        float.MaxValue))
                        : targetPos.x, isVertical ?
                        Mathf.Clamp(targetPos.y, yPos, yPos + Mathf.Clamp(content.rect.height * content.localScale.x - viewport.rect.height, 0,
                        float.MaxValue))
                        : targetPos.y);

                    targetPos = new Vector3(Mathf.Lerp(targetPos.x, clampedPos.x, lerpSpeed * Time.deltaTime),
                        Mathf.Lerp(targetPos.y, clampedPos.y, lerpSpeed * Time.deltaTime));

                    targetPos = new Vector2(isHorizontal ?
                        Mathf.Clamp(targetPos.x, xPos - elasity, xPos + elasity + Mathf.Clamp(content.rect.width * content.localScale.x - viewport.rect.width, 0,
                        float.MaxValue))
                        : targetPos.x, isVertical ?
                        Mathf.Clamp(targetPos.y, yPos - elasity, yPos + elasity + Mathf.Clamp(content.rect.height * content.localScale.x - viewport.rect.height, 0,
                        float.MaxValue))
                        : targetPos.y);

                    inertiaVector = new Vector3(Mathf.Lerp(inertiaVector.x, 0, inertiaFade * Time.deltaTime),
                        Mathf.Lerp(inertiaVector.y, 0, inertiaFade * Time.deltaTime));
                    inertiaVector *= ratio;

                    content.anchoredPosition = Vector2.Lerp(content.anchoredPosition, targetPos, lerpSpeedContent * Time.deltaTime);

                    if ((Mathf.Abs(inertiaVector.x) + Mathf.Abs(inertiaVector.y)) < inertiaLowerValue && !isSentStopMovement) {
                        isSentStopMovement = true;
                        if (onStopMovement != null) {
                            onStopMovement();
                        }
                    }
                    return;
                }
                if(content.anchoredPosition.x + inertiaVector.x < xPos && inertiaVector.x != 0) {
                    inertiaVector.x = 0;
                    content.anchoredPosition = new Vector2(xPos, content.anchoredPosition.y);
                }
                if (content.anchoredPosition.x + inertiaVector.x >
                    xPos + content.rect.width * content.localScale.x - viewport.rect.width && inertiaVector.x != 0) {
                    inertiaVector.x = 0;
                    content.anchoredPosition = new Vector2(xPos + content.rect.width * content.localScale.x - viewport.rect.width, content.anchoredPosition.y);
                }

                if (content.anchoredPosition.y + inertiaVector.y < yPos && inertiaVector.y != 0) {
                    inertiaVector.y = 0;
                    content.anchoredPosition = new Vector2(content.anchoredPosition.x, yPos);
                }
                if (content.anchoredPosition.y + inertiaVector.y >
                    yPos + content.rect.height * content.localScale.x - viewport.rect.height && inertiaVector.y != 0) {
                    inertiaVector.y = 0;

                    content.anchoredPosition = new Vector2(content.anchoredPosition.x, 
                        yPos + content.rect.height * content.localScale.x - viewport.rect.height);
                }
                inertiaVector = new Vector3(Mathf.Lerp(inertiaVector.x, 0, inertiaFade * Time.deltaTime),
                    Mathf.Lerp(inertiaVector.y, 0, inertiaFade * Time.deltaTime));
                content.anchoredPosition += inertiaVector;

                if ((Mathf.Abs(inertiaVector.x) + Mathf.Abs(inertiaVector.y)) < inertiaLowerValue && !isSentStopMovement) {
                    isSentStopMovement = true;
                    if (onStopMovement != null) {
                        onStopMovement();
                    }
                }
            }
        }
    }

    private void OnEnable()
    {
        OnActiveStateChange?.Invoke(true);
    }

    private void OnDisable()
    {
        OnActiveStateChange?.Invoke(false);
    }


    public void OnBeginDrag(PointerEventData eventData) {

        needStop = false;

        if (isDebug) {
            //print("BeginDrag " + eventData.position);
        }

        if (!isCanStartDrag) {
            return;
        }

        if (parentScroll == null || isHorizontal && isVertical || isParentScroll) 
        {
            isStartDrag = true;
            isSentStopMovement = false;
            Vector2 mouseVector = eventData.position;
            mouseVector.x /= camera.pixelWidth;
            mouseVector.y /= camera.pixelHeight;

            mouseVector.x *= canvas.rect.width;
            mouseVector.y *= canvas.rect.height;

            currentPosition = mouseVector;
            firstPosition = content.anchoredPosition;
            targetPos = content.anchoredPosition;
            firstPositionMouse = mouseVector;

            onStartDrag?.Invoke();
            onStartScrolling?.Invoke(eventData);

            return;
        }
        if (isClonedObj) {
            isStartDrag = true;
            isSentStopMovement = false;
            targetScroll.OnBeginDrag(eventData);
            return;
        }
        if (isHorizontal) {
            if (Mathf.Abs(eventData.delta.x) > Mathf.Abs(eventData.delta.y)) {
                isStartDrag = true;
                isSentStopMovement = false;
                Vector2 mouseVector = eventData.position;
                mouseVector.x /= camera.pixelWidth;
                mouseVector.y /= camera.pixelHeight;

                mouseVector.x *= canvas.rect.width;
                mouseVector.y *= canvas.rect.height;

                currentPosition = mouseVector;
                firstPosition = content.anchoredPosition;
                targetPos = content.anchoredPosition;
                firstPositionMouse = mouseVector;

                onStartDrag?.Invoke();
                onStartScrolling?.Invoke(eventData);
            } else {
                isStartDrag = false;
                parentScroll?.OnBeginDrag(eventData);
            }
        }else if (isVertical) {
            if (Mathf.Abs(eventData.delta.y) > Mathf.Abs(eventData.delta.x)) {
                isStartDrag = true;
                isSentStopMovement = false;
                Vector2 mouseVector = eventData.position;
                mouseVector.x /= camera.pixelWidth;
                mouseVector.y /= camera.pixelHeight;

                mouseVector.x *= canvas.rect.width;
                mouseVector.y *= canvas.rect.height;

                currentPosition = mouseVector;
                firstPosition = content.anchoredPosition;
                targetPos = content.anchoredPosition;
                firstPositionMouse = mouseVector;

                onStartDrag?.Invoke();
                onStartScrolling?.Invoke(eventData);
            } else {
                parentScroll?.OnBeginDrag(eventData);
            }
        }
    }

    public void OnDrag(PointerEventData eventData) {
        if (!isParentScroll && !isStartDrag) {
            parentScroll?.OnDrag(eventData);
            onScrolling?.Invoke(eventData);
            return;
        }

        if (isDebug) {
            //print("Drag " + eventData.position);
        }
        Vector2 mouseVector = eventData.position;
        mouseVector.x /= camera.pixelWidth;
        mouseVector.y /= camera.pixelHeight;

        mouseVector.x *= canvas.rect.width;
        mouseVector.y *= canvas.rect.height;

        currentPosition = mouseVector;

        onScrolling?.Invoke(eventData);
    }

    public void OnEndDrag(PointerEventData eventData, bool target) {
        if (isDebug) {
            //print("EndDrag " + eventData.position);
        }
        isCanStartDrag = true;
        isStartDrag = false;
        var xPos = (content.rect.width * content.localScale.x > viewport.rect.width ?
            viewport.rect.width * (1 - content.anchorMin.x) * (1 - content.anchorMax.x)
            - content.rect.width * content.localScale.x * (1 - content.pivot.x) : 0);
        var yPos = (content.rect.height * content.localScale.x > viewport.rect.height ?
            viewport.rect.height * (1 - content.anchorMin.y) * (1 - content.anchorMax.y)
            - content.rect.height * content.localScale.x * (1 - content.pivot.y) : 0);

        if (movementType != MovementTypeEnum.Unrestricted) {
            if (content.anchoredPosition.x < xPos) {
                inertiaVector.x = 0;
            }
            if (content.anchoredPosition.x >
                xPos + content.rect.width * content.localScale.x - viewport.rect.width) {
                inertiaVector.x = 0;
            }

            if (content.anchoredPosition.y < yPos) {
                inertiaVector.y = 0;
            }
            if (content.anchoredPosition.y >
                yPos + content.rect.height * content.localScale.x - viewport.rect.height) {
                inertiaVector.y = 0;
            }
        }

        if (onStopDrag != null) {
            onStopDrag();
        }
        onStopScrolling?.Invoke(eventData);
    }

    public float lerpSpeedContent = 50;

    public void OnEndDrag(PointerEventData eventData) {
        if (isDebug) {
            //print("EndDrag2 " + eventData.position);
        }
        isCanStartDrag = true;

        if (isClonedObj) {
            isStartDrag = false;
            targetScroll.OnEndDrag(eventData);
            return;
        }

        if (!isStartDrag && !isParentScroll) {
            parentScroll?.OnEndDrag(eventData);
            onStopScrolling?.Invoke(eventData);
            return;
        }
        Vector2 mouseVector = eventData.position;
        mouseVector.x /= camera.pixelWidth;
        mouseVector.y /= camera.pixelHeight;

        mouseVector.x *= canvas.rect.width;
        mouseVector.y *= canvas.rect.height;

        currentPosition = mouseVector;
        //MoveScroll();

        isStartDrag = false;

        var xPos = (content.rect.width * content.localScale.x > viewport.rect.width ?
            viewport.rect.width * (1 - content.anchorMin.x) * (1 - content.anchorMax.x)
            - content.rect.width * content.localScale.x * (1 - content.pivot.x) : 0);
        var yPos = (content.rect.height * content.localScale.x > viewport.rect.height ?
            viewport.rect.height * (1 - content.anchorMin.y) * (1 - content.anchorMax.y)
            - content.rect.height * content.localScale.x * (1 - content.pivot.y) : 0);

        if (movementType != MovementTypeEnum.Unrestricted) {
            if (content.anchoredPosition.x < xPos) {
                inertiaVector.x = 0;
            }
            if (content.anchoredPosition.x >
                xPos + content.rect.width * content.localScale.x - viewport.rect.width) {
                inertiaVector.x = 0;
            }

            if (content.anchoredPosition.y < yPos) {
                inertiaVector.y = 0;
            }
            if (content.anchoredPosition.y >
                yPos + content.rect.height * content.localScale.x - viewport.rect.height) {
                inertiaVector.y = 0;
            }
        }

        if (onStopDrag != null) {
            onStopDrag();
        }
        onStopScrolling?.Invoke(eventData);
    }
   

    public void AddPosition(Vector2 position) {
        content.anchoredPosition += position;
        firstPosition = content.anchoredPosition;
    }

    public void SetPosition(Vector2 position) {
        content.anchoredPosition = position;
        targetPos = position;
        inertiaVector = new Vector2(0, 0);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isStartDrag && inertiaVector.magnitude > 0.2f)
        {
            needStop = true;
            inertiaVector = Vector2.zero;            
        }
        OnPointerDownInvoked?.Invoke(eventData);
        if (parentScroll != null) parentScroll.OnPointerDown(null);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (needStop)
        {
            needStop = false;
            targetPos = content.anchoredPosition;
        }
        OnPointerUpInvoked?.Invoke(eventData);
        if (parentScroll != null) parentScroll.OnPointerUp(null);
    }
    private void MoveScroll () {
        var mouseDelta = currentPosition - firstPositionMouse;
        firstPositionMouse = currentPosition;

        if (isUseInertia) {
            inertiaVector = new Vector2(isHorizontal ? mouseDelta.x / Time.deltaTime : 0, isVertical ? mouseDelta.y / Time.deltaTime : 0) * inertia +
                0.2f * inertiaVector;
            inertiaVector = new Vector2(Mathf.Clamp(inertiaVector.x, -maxInertia, maxInertia),
                Mathf.Clamp(inertiaVector.y, -maxInertia, maxInertia));
        }

        switch (movementType) {
            case MovementTypeEnum.Clamped:
                targetPos += new Vector2(isHorizontal ? mouseDelta.x : 0,
                    isVertical ? mouseDelta.y : 0);
                var xPos = (content.rect.width * content.localScale.x > viewport.rect.width ?
                    viewport.rect.width * (1 - content.anchorMin.x) * (1 - content.anchorMax.x)
                    - content.rect.width * content.localScale.x * (1 - content.pivot.x) : 0);
                var yPos = (content.rect.height * content.localScale.x > viewport.rect.height ?
                    viewport.rect.height * (1 - content.anchorMin.y) * (1 - content.anchorMax.y)
                    - content.rect.height * content.localScale.x * (1 - content.pivot.y) : 0);

                targetPos = new Vector2(isHorizontal ?
                    Mathf.Clamp(targetPos.x, xPos, xPos + Mathf.Clamp(content.rect.width * content.localScale.x - viewport.rect.width, 0,
                    float.MaxValue))
                    : targetPos.x, isVertical ?
                    Mathf.Clamp(targetPos.y, yPos, yPos + Mathf.Clamp(content.rect.height * content.localScale.x - viewport.rect.height, 0,
                    float.MaxValue))
                    : targetPos.y);

                content.anchoredPosition = Vector2.Lerp(content.anchoredPosition, targetPos, lerpSpeedContent * Time.deltaTime);
                break;
            case MovementTypeEnum.Elastic:
                xPos = (content.rect.width * content.localScale.x > viewport.rect.width ?
                    viewport.rect.width * (1 - content.anchorMin.x) * (1 - content.anchorMax.x)
                    - content.rect.width * content.localScale.x * (1 - content.pivot.x) : 0);
                yPos = (content.rect.height * content.localScale.x > viewport.rect.height ?
                    viewport.rect.height * (1 - content.anchorMin.y) * (1 - content.anchorMax.y)
                    - content.rect.height * content.localScale.x * (1 - content.pivot.y) : 0);

                targetPos = new Vector2((targetPos.x < xPos && isHorizontal) ?
                        firstPosition.x + (mouseDelta.x < 0 ? Mathf.Clamp(mouseDelta.x, -maxElasitySence, 0) * Mathf.Clamp(1 - Mathf.Abs(xPos - targetPos.x) /
                           elasity, 0, 1) : mouseDelta.x) : (targetPos.x >= xPos + elasity + Mathf.Clamp(content.rect.width * content.localScale.x
                           - viewport.rect.width, 0, float.MaxValue)
                    && isHorizontal ? firstPosition.x + (mouseDelta.x > 0 ? Mathf.Clamp(mouseDelta.x, 0, maxElasitySence) * Mathf.Clamp(1 - Mathf.Abs(targetPos.x -
                        (xPos + Mathf.Clamp(content.rect.width * content.localScale.x - viewport.rect.width, 0, float.MaxValue))) /
                           elasity, 0, 1) : mouseDelta.x) : isHorizontal ? firstPosition.x + mouseDelta.x : targetPos.x),


                           targetPos.y < yPos && isVertical ?
                           firstPosition.y + (mouseDelta.y < 0 ? Mathf.Clamp(mouseDelta.y, -maxElasitySence, 0) * Mathf.Clamp(1 - Mathf.Abs(yPos - targetPos.y) /
                           elasity, 0, 1) : mouseDelta.y) : (targetPos.y > yPos + Mathf.Clamp(content.rect.height * content.localScale.x
                           - viewport.rect.height, 0, float.MaxValue))
                    && isVertical ? firstPosition.y + (mouseDelta.y > 0 ? Mathf.Clamp(mouseDelta.y, 0, maxElasitySence) * Mathf.Clamp(1 - Mathf.Abs(targetPos.y -
                        (yPos + Mathf.Clamp(content.rect.height * content.localScale.x - viewport.rect.height, 0, float.MaxValue))) /
                           elasity, 0, 1) : mouseDelta.y) : isVertical ? firstPosition.y + mouseDelta.y : targetPos.y);

                targetPos = new Vector2(isHorizontal ?
                    Mathf.Clamp(targetPos.x, xPos - elasity,
                    xPos + Mathf.Clamp(content.rect.width * content.localScale.x - viewport.rect.width, 0,
                    float.MaxValue) + elasity)
                    : targetPos.x, isVertical ?
                    Mathf.Clamp(targetPos.y, yPos - elasity,
                    yPos + Mathf.Clamp(content.rect.height * content.localScale.x - viewport.rect.height, 0,
                    float.MaxValue) + elasity)
                    : targetPos.y);


                firstPosition = targetPos;

                content.anchoredPosition = Vector2.Lerp(content.anchoredPosition, targetPos, lerpSpeedContent * Time.deltaTime);
                break;
            case MovementTypeEnum.Unrestricted:
                content.anchoredPosition += new Vector2(isHorizontal ? mouseDelta.x : 0,
                    isVertical ? mouseDelta.y : 0);
                break;
        }

    }

}

public interface IPointerDownEventer : IPointerDownHandler
{
    public event Action<PointerEventData> OnPointerDownInvoked;
}

public interface IPointerUpEventer : IPointerUpHandler
{
    public event Action<PointerEventData> OnPointerUpInvoked;
}

public interface IPointerStopableScroll
{
    public bool IsNeedStop { get; }
}