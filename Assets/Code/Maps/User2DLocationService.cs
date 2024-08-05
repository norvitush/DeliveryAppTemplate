using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.Maps
{
    public class User2DLocationService : MonoBehaviour, IDisposable
    {
        public enum UserLocationState { NotStarted, WaitingLocationServiceStart, ServiceAvalible, ServiceDisable,
            BlockedByUser
        }

        /// <summary>
        /// This event is called when changed your GPS location
        /// </summary>
        public Action<Vector2> OnLocationChanged;

        /// <summary>
        /// This event is called when the GPS is initialized (the first value is received) or location by IP is found
        /// </summary>
        public event Action OnLocationInited;
        public event Action OnLocationDisable;
        public event Action OnServiceStart;

        private const int START_SERVICE_VAL = 2;
        private const string USER_TAG = "user";

        /// <summary>
        /// Texture of 2D marker
        /// </summary>
        public Texture2D marker2DTexture;

        /// <summary>
        /// Align of the 2D marker
        /// </summary>
        public OnlineMapsAlign marker2DAlign = OnlineMapsAlign.Center;

        /// <summary>
        /// The heading in degrees relative to the geographic North Pole.<br/>
        /// <strong>Important: position not available Start, because compass is not already initialized.<br/>
        /// Use OnCompassChanged event, to determine the initialization of compass.</strong>
        /// </summary>
        public float trueHeading = 0;

        /// <summary>
        /// Current GPS coordinates.<br/>
        /// <strong>Important: position not available Start, because GPS is not already initialized.<br/>
        /// Use OnLocationInited event, to determine the initialization of GPS.</strong>
        /// </summary>
        public Vector2 position = Vector2.zero;

        /// <summary>
        /// Specifies the need for marker rotation
        /// </summary>
        public bool useCompassForMarker = false;

        /// <summary>
        /// Smooth rotation by compass. This helps to bypass the jitter.
        /// </summary>
        public bool lerpCompassValue = true;

        [SerializeField] private float compassThreshold = 8;
        [SerializeField] private float markerScale;

        private OnlineMaps           _map;
        private LocationService      _innerService;
        private OnlineMapsMarkerBase _marker;

        private UserLocationState _userLocationState;
        
        private bool _isPositionInited = false;


        public UserLocationState State 
        { 
            get => _userLocationState; 
            private set 
            {
                Debug.Log($"{_userLocationState} -> {value}");
                _userLocationState = value; 
            } 
        }
        
        public bool IsStarted => State == UserLocationState.ServiceAvalible;

        public OnlineMapsMarkerBase Marker { get => _marker;}

        private float _userMarkerScale = 1f;

        private void OnEnable()
        {                
            if (_map == null) _map = GetComponent<OnlineMaps>();
            var markerManager = GetComponent<OnlineMapsMarkerManager>();
            _innerService = Input.location;
            //if (_map != null) _map.OnChangePosition += OnChangePosition;

            bool hasUser = false;
            foreach (OnlineMapsMarkerBase marker in markerManager)
            {
                if (marker.tags != null && marker.tags.Contains(USER_TAG))
                {
                    hasUser = true;
                    break;
                }
            }

            if (hasUser == false)
            {
                Debug.Log($"NO USER MARK!");
                _marker = null;                                    
            }

            OnlineMapsMarkerManager.OnRemoveItem -= OnRemoveUser;
            OnlineMapsMarkerManager.OnRemoveItem += OnRemoveUser;

            if(marker2DTexture!= null)
            {
                var targetW = Screen.width / 10f;

                var scale = targetW / marker2DTexture.width;
            }
        }
        private void OnDisable()
        {
            OnlineMapsMarkerManager.OnRemoveItem -= OnRemoveUser;
        }

        private void Update()
        {

            if (State == UserLocationState.BlockedByUser)
            {
                if (!_innerService.isEnabledByUser) return;
                else
                    State = UserLocationState.NotStarted;
            }
            
            if (State == UserLocationState.NotStarted)
            {
                Input.compass.enabled = true;
                if(!TryStartLocationService()) return;

                State = UserLocationState.WaitingLocationServiceStart;
            }

            if (State == UserLocationState.WaitingLocationServiceStart)
            {
                if (_innerService.status == LocationServiceStatus.Running)
                    State = UserLocationState.ServiceAvalible;
                else
                    return;
            }

            if(State == UserLocationState.ServiceAvalible)
            {

                bool compassChanged = false;
                bool positionChanged = false;

                UpdateCompassFromInput(ref compassChanged);
                UpdatePositionFromInput(ref positionChanged);
                
                if (positionChanged || compassChanged) UpdateMarker();

                UpdateMarkerRotation();

                if (positionChanged)
                {
                    if (!_isPositionInited)
                    {
                        _isPositionInited = true;
                        if (OnLocationInited != null) OnLocationInited();
                    }
                    if (OnLocationChanged != null) OnLocationChanged(position);

                    _map.Redraw();
                }

                if (_innerService.status != LocationServiceStatus.Running)
                {
                    State = UserLocationState.NotStarted;
                    _isPositionInited = false;
                    OnlineMapsMarkerManager.RemoveItemsByTag(USER_TAG);
                    _marker = null;
                    _map.Redraw();
                    OnLocationDisable?.Invoke();
                }
            }

        }

        public bool TryStartLocationService()
        {
            Debug.Log($"TryStartLocationService {Input.location.isEnabledByUser}");
            if (!_innerService.isEnabledByUser)
            {
                State = UserLocationState.BlockedByUser;

//                if (requestPermissionRuntime && !isPermissionRequested)
//                {
//#if PLATFORM_ANDROID
//                    if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
//                    {
//                        isPermissionRequested = true;
//                        Permission.RequestUserPermission(Permission.FineLocation);
//                        return Permission.HasUserAuthorizedPermission(Permission.FineLocation);
//                    }
//#endif
//                }
                return false;
            }
            else
            {
                if(_innerService.status != LocationServiceStatus.Running) StartLocationService();
                return true;
            }
        }


        public void StartLocationService(float? desiredAccuracyInMeters = null, float? updateDistanceInMeters = null)
        {

            if (!desiredAccuracyInMeters.HasValue) desiredAccuracyInMeters = START_SERVICE_VAL;
            if (!updateDistanceInMeters.HasValue) updateDistanceInMeters = 2;

            Debug.Log($"StartLocationService Start({desiredAccuracyInMeters.Value}, {updateDistanceInMeters.Value})");
            Input.location.Start(desiredAccuracyInMeters.Value, updateDistanceInMeters.Value);
        }
        private void UpdatePositionFromInput(ref bool positionChanged)
        {

            LocationInfo data = Input.location.lastData;
            float longitude = data.longitude;
            float latitude = data.latitude;

            if (Math.Abs(position.x - longitude) > float.Epsilon)
            {
                position.x = longitude;
                positionChanged = true;
            }
            if (Math.Abs(position.y - latitude) > float.Epsilon)
            {
                position.y = latitude;
                positionChanged = true;
            }
        }

        private void UpdateCompassFromInput(ref bool compassChanged)
        {
            float heading = Input.compass.trueHeading;
            float offset = trueHeading - heading;

            if (offset > 180) offset -= 360;
            else if (offset < -180) offset += 360;

            if (Mathf.Abs(offset) > compassThreshold)
            {
                compassChanged = true;
                trueHeading = heading;
                //if (OnCompassChanged != null) OnCompassChanged(trueHeading / 360);
            }
        }

        private void UpdateMarkerRotation()
        {
            if (_marker == null) return;

            float value = (_marker as OnlineMapsMarker).rotationDegree;
         

            if (trueHeading - value > 180) value += 360;
            else if (trueHeading - value < -180) value -= 360;

            if (Math.Abs(trueHeading - value) >= float.Epsilon)
            {
                if (!lerpCompassValue || Mathf.Abs(trueHeading - value) < 0.003f) value = trueHeading;
                else value = Mathf.Lerp(value, trueHeading, 0.02f);

                (_marker as OnlineMapsMarker).rotationDegree = value;

                _map.Redraw();
            }
        }

        private void OnRemoveUser(OnlineMapsMarker obj)
        {

            if (obj == _marker) 
            {
                Debug.Log($"REMOVE USER MARK!!!!");
                _marker = null; 
            }
        }

        /// <summary>
        /// Returns the current GPS location or emulator location.
        /// </summary>
        /// <param name="longitude">Longitude</param>
        /// <param name="latitude">Latitude</param>
        public void GetLocation(out float longitude, out float latitude)
        {
            longitude = position.x;
            latitude = position.y;
        }

        private void UpdateMarker()
        {
            if (_marker == null)
            {
                OnlineMapsMarker m2d = OnlineMapsMarkerManager.CreateItem(position, marker2DTexture, string.Empty);
                _marker = m2d;
                if (_marker.tags == null) m2d.tags = new List<string>();
                _marker.tags.Add(USER_TAG);
                m2d.align = marker2DAlign;
                m2d.scale = marker2DTexture != null ? (Screen.width / 17f) / marker2DTexture.width : markerScale;
                if (useCompassForMarker) m2d.rotationDegree = trueHeading;
            }
            else
            {
                _marker.position = position;
            }
        }

        public void Dispose()
        {
            
        }
    }
}