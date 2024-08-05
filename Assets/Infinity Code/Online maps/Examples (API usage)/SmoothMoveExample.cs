/*         INFINITY CODE         */
/*   https://infinity-code.com   */

using System;
using System.Collections;
using UnityEngine;

namespace InfinityCode.OnlineMapsExamples {
    /// <summary>
    /// Example of a smooth movement to current GPS location.
    /// </summary>
    [AddComponentMenu("Infinity Code/Online Maps/Examples (API Usage)/SmoothMoveExample")]
    public class SmoothMoveExample : MonoBehaviour, IMapSmoothMoveController
    {
        public event Action OnCameraIdle;
        public event Action OnCameraStartMove;
        public AnimationCurve curve;
        public float curveRatio;

        public OnlineMaps instanceMap;

        public static SmoothMoveExample instance;

        public bool canZoom = false;
        public bool canZoomCurve = false;
        public bool canMove = false;
        public float speedMove;
        public float speedZoom;

        public Vector2 targetCenter;
        public float targetZoom;
        public float startDistance;
        public float startZoom;
        private Coroutine _movingHandler;

        void Start () {
            instance = this;
            var control = OnlineMapsControlBase.instance;

            control.OnMapPress += OnMapPress;

            instanceMap = OnlineMaps.instance;
        }
        public static double Lerp (double a, double b, double t) {
            return a + (b - a) * t;
        }
        public void SetLocation(float lattitude, float longitude, int zoom)
        {
            DragAndZoomInertia.instance.isCanInertia = false;
            //
            //print(zoom);
            //print(instanceMap.floatZoom);

            targetCenter = new Vector2(longitude, lattitude);
            targetZoom = zoom - 1;

            startDistance = Vector2.Distance(instanceMap.position, targetCenter);
            if (startDistance == 0) {
                startDistance = 0.001f;
            }
            startZoom = instanceMap.floatZoom;


            OnCameraStartMove?.Invoke();

            if (_movingHandler != null) { StopCoroutine(_movingHandler); }
            _movingHandler = StartCoroutine(MovingToTarget());
        }

        private IEnumerator MovingToTarget()
        {
            canZoom = true;
            canMove = true;

            print($"Start smooth move");
            yield return null;
            DragAndZoomInertia.instance.isCanInertia = false;

            while (Vector2.Distance(instanceMap.position, targetCenter) > 0.00001f && (canMove||canZoom||canZoomCurve))
            {
                yield return null;
                if (Input.mouseScrollDelta.y != 0)
                {
                    canZoom = false;
                    canZoomCurve = false;
                    targetZoom = instanceMap.floatZoom;
                }

                if (canMove)
                {
                    double px;
                    double pxTarget = targetCenter.x;
                    double py;
                    double pyTarget = targetCenter.y;
                    instanceMap.GetPosition(out px, out py);
                    instanceMap.SetPosition(Lerp(px, pxTarget, speedMove * Time.deltaTime), Lerp(py, pyTarget, speedMove * Time.deltaTime));

                    if (Vector2.Distance(instanceMap.position, targetCenter) < 0.00001f)
                    {
                        OnCameraIdle?.Invoke();
                        canMove = false;
                    }
                }

                if (canZoom)
                {
                    instanceMap.floatZoom = startZoom + (1 - (Vector2.Distance(instanceMap.position, targetCenter) /
                        startDistance)) * (targetZoom - startZoom);
                }

                if (canZoomCurve)
                {
                    instanceMap.floatZoom = targetZoom - curve.Evaluate(Mathf.Clamp(Vector2.Distance(instanceMap.position, targetCenter) /
                        startDistance, 0f, 1f)) * curveRatio;
                }
            }

            print($"End smooth move");
            _movingHandler = null;
            canMove = canZoom = canZoomCurve = false;
        }

        private void OnMapPress () {
            canZoom = false;
            canMove = false;
            canZoomCurve = false;
            targetZoom = instanceMap.floatZoom;
            if (_movingHandler != null) { StopCoroutine(_movingHandler); _movingHandler = null; }
        }
        private void OnDisable()
        {
            _movingHandler = null;
        }
    }
}