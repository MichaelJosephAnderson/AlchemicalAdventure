using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPath : MonoBehaviour
{
    [System.Serializable]
    public class PathAction : ISerializationCallbackReceiver
    {
        public Vector3 movement;
        public float speedMultiplier = 1f;

        public PathAction()
        {
            movement = Vector3.zero;
            speedMultiplier = 1f;
        }

        public void OnBeforeSerialize()
        {
            if (Mathf.Approximately(speedMultiplier, 0f))
            {
                speedMultiplier = 1f;
            }
        }

        public void OnAfterDeserialize()
        {
            if (Mathf.Approximately(speedMultiplier, 0f))
            {
                speedMultiplier = 1f;
            }
        }
    }

    public enum LoopMode
    {
        OneShot,
        OneShotDes,
        Loop,
        PingPong,
        PingPongDelay
    }

    [SerializeField] private Transform _object;
    [SerializeField] private bool _playOnStart = true;
    [SerializeField] private LoopMode _loopMode = LoopMode.PingPong;
    [SerializeField] private float _speed = 1f;
    [SerializeField] private float _smoothing = 0.1f;
    [SerializeField] private bool _alignRotation = true;
    [SerializeField] private bool _isLog = false;
    [SerializeField] private bool _reverse;
    [SerializeField] private float delayTime = 0f;
    [SerializeField] private List<PathAction> _pathActions;

    private bool _movementActive;
    private float _timer;
    private float _previousTimer;
    private float _length;
    private Vector3 _targetPosition;
    private Vector3 _positionVelocity;
    private float _targetRotation;
    private float _currentRoataion;
    private float _rotationVelocity;
    private bool _loopConnects;
    private float delayTimer;

    public float speed => _speed;

    private void Start()
    {
        if (_loopMode == LoopMode.PingPongDelay)
        {
            delayTimer = delayTime;
        }

        if (_playOnStart) 
        {
            _movementActive = true;
        }
    }

    private void Update()
    {
        UpdateLength();
        UpdateLoopConnects();
        UpdateTimer();
        UpdateObjectPosition();

        if (_alignRotation) 
        {
            UpdateObjectRotation();
        }
    }

    private Vector3 GetPositionAtTime(float time) 
    {
        float currentTime = 0f;
        Vector3 position = transform.position;

        foreach (PathAction pathAction in _pathActions) 
        {
            float newTime = currentTime + pathAction.movement.magnitude;

            if (newTime >= time)
            {
                float normalizedTime = Map(time, currentTime, newTime, 0, 1);
                Vector3 newPosition = position + transform.TransformDirection(pathAction.movement);
                position = Vector3.Lerp(position, newPosition, normalizedTime);
                break;
            }
            else 
            {
                position += transform.TransformDirection(pathAction.movement);
                currentTime = newTime;
            }
        }

        return position;
    }

    public static float Map(float x, float inMin, float inMax, float outMin, float outMax)
    {
        return (x - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
    }

    private float GetRotationAtTime(float time, bool reverse) 
    {
        float currentTime = 0f;

        foreach (PathAction pathAction in _pathActions) 
        {
            float newTime = currentTime + pathAction.movement.magnitude;

            if (newTime >= time)
            {
                if (reverse)
                {
                    return Quaternion.LookRotation(transform.TransformDirection(pathAction.movement), Vector3.up).eulerAngles.y + 180;
                }
                else
                {
                    return Quaternion.LookRotation(transform.TransformDirection(pathAction.movement), Vector3.up).eulerAngles.y;
                }
            }
            else 
            {
                currentTime = newTime;
            }
        }

        return transform.eulerAngles.y;
    }

    private float GetSpeedMultiplierAtTime(float time) 
    {
        float currentTime = 0f;

        foreach (PathAction pathAction in _pathActions) 
        {
            float newTime = currentTime + pathAction.movement.magnitude;

            if (newTime >= time)
            {
                return pathAction.speedMultiplier;
            }
            else 
            {
                currentTime = newTime;
            }
        }

        return 1f;
    }

    private void UpdateTimer() 
    {
        if (!_movementActive) 
        {
            return;
        }
        
        _previousTimer = _timer;

        if (_reverse)
        {
            _timer = Mathf.MoveTowards(_timer, -1f, Time.deltaTime * GetSpeedMultiplierAtTime(_timer) * _speed);
        }
        else 
        {
            _timer = Mathf.MoveTowards(_timer, _length + 1f, Time.deltaTime * GetSpeedMultiplierAtTime(_timer) * _speed);
        }

        if (_timer > _length || _timer < 0f) 
        {
            if (_loopMode == LoopMode.Loop)
            {
                _timer -= _length;
            }
            else if (_loopMode == LoopMode.PingPong) 
            {
                if (_reverse)
                {
                    _timer = Mathf.Abs(_timer);
                    _reverse = false;
                }
                else 
                {
                    _timer = _length - (_timer - _length);
                    _reverse = true;
                }
            }
            else if (_loopMode == LoopMode.PingPongDelay)
            {
                if (delayTimer <= 0)
                {
                    delayTimer = delayTime;
                    if (_reverse)
                    {
                        _timer = Mathf.Abs(_timer);
                        _reverse = false;
                    }
                    else 
                    {
                        _timer = _length - (_timer - _length);
                        _reverse = true;
                    }
                }
                else
                {
                    delayTimer -= Time.deltaTime;
                }
            }
            else if (_loopMode == LoopMode.OneShotDes)
            {
                Destroy(gameObject);
            }
        }
    }

    private void UpdateLength() 
    {
        _length = 0f;

        foreach (PathAction pathAction in _pathActions) 
        {
            _length += pathAction.movement.magnitude;
        }
    }

    private void UpdateLoopConnects() 
    {
        if (Vector3.Distance(GetPositionAtTime(0f), GetPositionAtTime(_length)) < 0.01f)
        {
            _loopConnects = true;
        }
        else 
        {
            _loopConnects = false;
        }
    }

    private void UpdateObjectPosition() 
    {
        _targetPosition = GetPositionAtTime(_timer);

        bool useSmoothing = true;

        if (_smoothing < 0.0001f) 
        {
            useSmoothing = false;
        }

        if (_loopMode == LoopMode.Loop && !_loopConnects) 
        {
            if (_timer < _previousTimer && !_reverse) 
            {
                useSmoothing = false;
            }

            if (_timer > _previousTimer && _reverse) 
            {
                useSmoothing = false;
            }
        }

        if (useSmoothing)
        {
            _object.position = Vector3.SmoothDamp(_object.position, _targetPosition, ref _positionVelocity, _smoothing);
        }
        else 
        {
            _object.position = _targetPosition;
            _positionVelocity = Vector3.zero;
        }
    }

    private void UpdateObjectRotation() 
    {
        _targetRotation = GetRotationAtTime(_timer, _reverse);

        if (_smoothing >= 0.0001f)
        {
            _currentRoataion = Mathf.SmoothDampAngle(_currentRoataion, _targetRotation, ref _rotationVelocity, _smoothing);
        }
        else 
        {
            _currentRoataion = _targetRotation;
            _rotationVelocity = 0f;
        }

        //Change back to (0, ~~~, 0) This just worked for a specific project
        if (_isLog)
        {
            _object.rotation = Quaternion.Euler(0, _currentRoataion + 90, 0);
        }
        else
        {
            _object.rotation = Quaternion.Euler(90, _currentRoataion, 180);   
        }
    }

    public void StartMovement() 
    {
        _movementActive = true;
    }

    public void StopMovement() 
    {
        _movementActive = false;
    }

    public void SetSpeed(float newSpeed) 
    {
        _speed = newSpeed;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, 0.1f);

        Vector3 position = transform.position;

        if (_pathActions != null) 
        {
            foreach (PathAction pathAction in _pathActions) 
            {
                Vector3 newPosition = position + transform.TransformDirection(pathAction.movement);

                Gizmos.DrawLine(position, newPosition);
                Gizmos.DrawWireSphere(newPosition, 0.1f);

                position = newPosition;
            }
        }
    }
}
