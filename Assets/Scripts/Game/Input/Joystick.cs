using Application.Utils;
using Game.Enabler;
using UnityEngine;

namespace Game.Input
{
    public class Joystick : EnablerMonoBehaviour
    {
        public static Joystick Instance { get; private set; }

        [SerializeField] private JoystickVisuals _joystickVisuals;

        private const float ExpectedBaseWidth = 1080.0f;

        private JoystickVisuals _currentVisuals;
        private Vector3 _startingPosition;
        private float _scaleFactor;
        private float _maxMovement;
        
        private void Awake()
        {
            Instance = this;
        }

        public void Initialize()
        {
            _currentVisuals = _joystickVisuals;
            _scaleFactor = Screen.width / ExpectedBaseWidth;
            _maxMovement = _currentVisuals.RectTransform.sizeDelta.x * _scaleFactor * NumberConstants.Half;
            Vector3 startingPosition = new Vector3(Screen.width * NumberConstants.Half, _currentVisuals.RectTransform.anchoredPosition.y * _scaleFactor, NumberConstants.Zero);
            _currentVisuals.RectTransform.anchoredPosition = startingPosition;
            _startingPosition = startingPosition;
            _currentVisuals.RectTransform.transform.localScale *= _scaleFactor;
            Hide();
        }
        
        public void FingerMove(Vector2 currentTouchPosition)
        {
            if (!_isEnabled)
            {
                return;
            }
            
            Vector2 handlePosition;
            Vector2 scaledTouchPosition = currentTouchPosition / _scaleFactor;
            Vector2 scaledPosition = _currentVisuals.RectTransform.anchoredPosition;

            if (Vector2.Distance(scaledTouchPosition, scaledPosition) > _maxMovement)
            {
                handlePosition = (scaledTouchPosition - scaledPosition).normalized * _maxMovement;
            }
            else
            {
                handlePosition = scaledTouchPosition - scaledPosition;
            }

            _currentVisuals.Handle.anchoredPosition = handlePosition;
        }

        public void FingerUp()
        {
            if (!_isEnabled)
            {
                return;
            }
            
            _currentVisuals.Handle.anchoredPosition = Vector2.zero;
            _currentVisuals.RectTransform.anchoredPosition = _startingPosition;
            Hide();
        }

        public void FingerDown(Vector2 startTouchPosition)
        {
            if (!_isEnabled)
            {
                return;
            }
            
            _currentVisuals.gameObject.SetActive(true);
            _currentVisuals.RectTransform.anchoredPosition = startTouchPosition / _scaleFactor;
            Show();
        }

        public override void Enable()
        {
            base.Enable();
            Show();
        }

        public override void Disable()
        {
            base.Disable();
            Hide();
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }

        private void Show()
        {
            gameObject.SetActive(true);
        }

        public float GetMaxMovement()
        {
            return _maxMovement;
        }
    }
}