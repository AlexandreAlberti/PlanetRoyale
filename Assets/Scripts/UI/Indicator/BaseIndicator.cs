using Application.Utils;
using DG.Tweening;
using Game.PlayerUnit;
using TMPro;
using UnityEngine;

namespace UI.Indicator
{
    public abstract class BaseIndicator : MonoBehaviour
    {
        private const float UpdateTargetIndicatorTimerMax = 0.05f;

        [SerializeField] private GameObject _indicator;
        [SerializeField] private float _targetOffset;
        [SerializeField] private float _borderOffsetLeft;
        [SerializeField] private float _borderOffsetRight;
        [SerializeField] private float _borderOffsetUp;
        [SerializeField] private float _borderOffsetDown;
        [SerializeField] private RectTransform _directionArrow;
        [SerializeField] private float _directionArrowRadius;
        [SerializeField] private Animator _animator;
        [SerializeField] private TextMeshProUGUI _distanceValueText;

        private const float MovementDuration = 1.0f;
        private const string Meter = "m";

        private bool _isEnabled;
        private bool _isFirstActivation;
        private Vector3 _targetPosition;
        private Vector3 _sphereCenter;
        private float _sphereRadius;
        private Player _player;
        private float _updateTargetIndicatorTimer;
        private float _borderOffsetLeftScaled;
        private float _borderOffsetRightScaled;
        private float _borderOffsetUpScaled;
        private float _borderOffsetDownScaled;
        private float _directionArrowRadiusScaled;

        private void Awake()
        {
            _isEnabled = false;
            _isFirstActivation = true;
            _indicator.SetActive(false);
        }

        public void Initialize(Vector3 sphereCenter, float sphereRadius)
        {
            _sphereCenter = sphereCenter;
            _sphereRadius = sphereRadius;
            Canvas canvas = _directionArrow.GetComponentInParent<Canvas>();
            float scaleFactor = canvas.scaleFactor;
            _borderOffsetLeftScaled = _borderOffsetLeft * scaleFactor;
            _borderOffsetRightScaled = _borderOffsetRight * scaleFactor;
            _borderOffsetUpScaled = _borderOffsetUp * scaleFactor;
            _borderOffsetDownScaled = _borderOffsetDown * scaleFactor;
            _directionArrowRadiusScaled = _directionArrowRadius * scaleFactor;
        }

        private void Update()
        {
            if (!_isEnabled)
            {
                return;
            }

            Transform target = Target();

            if (!target)
            {
                return;
            }

            _updateTargetIndicatorTimer += Time.deltaTime;

            if (_updateTargetIndicatorTimer < UpdateTargetIndicatorTimerMax)
            {
                return;
            }

            _updateTargetIndicatorTimer -= UpdateTargetIndicatorTimerMax;

            Vector3 targetScenePosition = target.position;
            Vector3 playerScenePosition = _player.transform.position;
            _distanceValueText.text = $"{Mathf.FloorToInt(playerScenePosition.DistanceInSphereSurface(targetScenePosition, _sphereRadius))} {Meter}";
            Camera mainCamera = Camera.main;

            if (!mainCamera)
            {
                return;
            }

            _targetPosition = mainCamera.WorldToScreenPoint(targetScenePosition);
            HandleDirectionArrow(mainCamera);

            if (_isFirstActivation)
            {
                return;
            }

            bool isTargetOffScreen = IsTargetOffScreen(_targetPosition);
            bool isTargetOnVisibleHemisphere = IsTargetOnVisibleHemisphere(mainCamera.transform.position, targetScenePosition);

            _indicator.SetActive(isTargetOffScreen || !isTargetOnVisibleHemisphere);

            if (!isTargetOffScreen)
            {
                return;
            }

            transform.position = FindValidIndicatorPosition(_targetPosition);
        }

        private void HandleDirectionArrow(Camera mainCamera)
        {
            Vector3 playerPosition = mainCamera.WorldToScreenPoint(_player.transform.position);
            Vector3 direction = _targetPosition - playerPosition;
            _directionArrow.rotation = Quaternion.LookRotation(Vector3.forward, direction.normalized) * Quaternion.Euler(0, 0, 90);
            _directionArrow.transform.position = transform.position + direction.normalized * (_directionArrowRadiusScaled);
        }

        private bool IsTargetOffScreen(Vector3 targetPosition)
        {
            return targetPosition.x + _targetOffset <= _borderOffsetLeftScaled ||
                   targetPosition.x - _targetOffset >= Screen.width - _borderOffsetRightScaled ||
                   targetPosition.y + _targetOffset <= _borderOffsetDownScaled ||
                   targetPosition.y - _targetOffset >= Screen.height - _borderOffsetUpScaled;
        }

        private bool IsTargetOnVisibleHemisphere(Vector3 cameraPosition, Vector3 targetPosition)
        {
            Vector3 planetToCamera = (cameraPosition - _sphereCenter).normalized;
            Vector3 planetToTarget = (targetPosition - _sphereCenter).normalized;
            return Vector3.Dot(planetToCamera, planetToTarget) > 0.0f;
        }

        private Vector3 FindValidIndicatorPosition(Vector3 targetPosition)
        {
            Vector3 validIndicatorPosition = targetPosition;

            if (validIndicatorPosition.x <= _borderOffsetLeftScaled)
            {
                validIndicatorPosition.x = _borderOffsetLeftScaled;
            }

            if (validIndicatorPosition.x >= Screen.width - _borderOffsetRightScaled)
            {
                validIndicatorPosition.x = Screen.width - _borderOffsetRightScaled;
            }

            if (validIndicatorPosition.y <= _borderOffsetDownScaled)
            {
                validIndicatorPosition.y = _borderOffsetDownScaled;
            }

            if (validIndicatorPosition.y >= Screen.height - _borderOffsetUpScaled)
            {
                validIndicatorPosition.y = Screen.height - _borderOffsetUpScaled;
            }

            return validIndicatorPosition;
        }

        public void Enable()
        {
            _isEnabled = true;
            _player = PlayerManager.Instance.GetHumanPlayer();
            _isFirstActivation = true;
            _updateTargetIndicatorTimer = 0.0f;
            GoToInitialTargetPosition();
        }

        private void GoToInitialTargetPosition()
        {
            Transform target = Target();

            if (!target)
            {
                return;
            }

            _indicator.SetActive(true);
            Camera mainCamera = Camera.main;

            if (!mainCamera)
            {
                return;
            }
            
            transform.position = mainCamera.WorldToScreenPoint(_player.transform.position + new Vector3(0.0f, 0.0f, 1.0f));
            _targetPosition = mainCamera.WorldToScreenPoint(target.position);
            transform.DOMove(FindValidIndicatorPosition(_targetPosition), MovementDuration).SetEase(Ease.OutQuad).onComplete = OnDestinationReached;
        }

        private void OnDestinationReached()
        {
            _isFirstActivation = false;
        }

        public void Disable()
        {
            _isEnabled = false;
            _indicator.SetActive(false);
        }

        public void DisableActions()
        {
            _animator.enabled = false;
        }

        public void EnableActions()
        {
            _animator.enabled = true;
        }

        protected abstract Transform Target();
    }
}