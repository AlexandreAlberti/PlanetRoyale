using System;
using Game.Enabler;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace Game.Input
{
    public class InputManager : EnablerMonoBehaviour
    {
        [Range(0.0f, 1.0f)] [SerializeField] private float _deadZonePercent;
        
        private Vector2 _inputVector;
        private Vector2 _touchStartPosition;
        private Finger _movementFinger;
        private float _maxSquaredValidMovementInversed;
        private bool _isDragging;

        public static InputManager Instance { get; private set; }

        public Action<Vector2> OnDragVector;
        public Action OnDragStarted;
        public Action OnDragStopped;

        private void Awake()
        {
            Instance = this;
#if !UNITY_EDITOR
            EnhancedTouchSupport.Enable();
#endif
        }

        void OnEnable()
        {
#if !UNITY_EDITOR
            TouchSimulation.Enable();
#endif
        }

        private void Update()
        {
            if (!_isEnabled)
            {
                return;
            }

            if (!_isDragging)
            {
                return;
            }

            if (_inputVector.sqrMagnitude * _maxSquaredValidMovementInversed < _deadZonePercent)
            {
                OnDragVector?.Invoke(Vector2.zero);
                return;
            }

            OnDragVector?.Invoke(_inputVector.normalized);
        }

        public void Initialize()
        {
            ResetMovement();
            float maxValidMovement = Joystick.Instance.GetMaxMovement();
            _maxSquaredValidMovementInversed = 1 / (maxValidMovement * maxValidMovement);
        }

        public override void Enable()
        {
            base.Enable();
            Touch.onFingerDown += HandleFingerDown;
            Touch.onFingerUp += HandleLoseFinger;
            Touch.onFingerMove += HandleFingerMove;
        }

        public override void Disable()
        {
            base.Disable();
            ResetMovement();
            Touch.onFingerDown -= HandleFingerDown;
            Touch.onFingerUp -= HandleLoseFinger;
            Touch.onFingerMove -= HandleFingerMove;
        }

        private void HandleFingerMove(Finger movedFinger)
        {
            if (movedFinger == _movementFinger && _isEnabled)
            {
                Touch currentTouch = movedFinger.currentTouch;
                _inputVector = currentTouch.screenPosition - _touchStartPosition;
                Joystick.Instance.FingerMove(currentTouch.screenPosition);
            }
        }
        
        private void HandleLoseFinger(Finger lostFinger)
        {
            if (_movementFinger != lostFinger || !_isEnabled)
            {
                return;
            }

            OnDragStopped?.Invoke();

            _movementFinger = null;
            _inputVector = Vector2.zero;
            _isDragging = false;
            Joystick.Instance.FingerUp();
        }

        private void HandleFingerDown(Finger touchedFinger)
        {
            if (_movementFinger == null && _isEnabled)
            {
                _movementFinger = touchedFinger;
                _inputVector = Vector2.zero;
                _touchStartPosition = touchedFinger.screenPosition;
                OnDragStarted?.Invoke();
                _isDragging = true;
                Joystick.Instance.FingerDown(_touchStartPosition);
            }
        }

        private void ResetMovement()
        {
            _movementFinger = null;
            _touchStartPosition = Vector2.zero;
            _inputVector = Vector2.zero;
        }
    }
}