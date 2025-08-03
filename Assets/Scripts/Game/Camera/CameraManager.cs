using System.Collections;
using Application.Utils;
using Cinemachine;
using Game.PlayerUnit;
using UnityEngine;

namespace Game.Camera
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera _camera;
        [SerializeField] private float _zoomOutTime;
        [Range(0, 15)]
        [SerializeField] private float _maxZoomOutScaleAllowed;

        public static CameraManager Instance { get; private set; }

        private CinemachineTransposer _cinemachineTransposer;
        private Vector3 _originalOffset;
        private float _originalYZMagnitude;
        private float _magnitudeConstantMultiplier;
        private float _lastScaleUsed;
        private float _fogStartCameraDistance;
        private float _fogEndCameraDistance;

        private void Awake()
        {
            Instance = this;
        }

        public void Initialize()
        {
            _cinemachineTransposer = _camera.GetCinemachineComponent<CinemachineTransposer>();
            _originalOffset = _cinemachineTransposer.m_FollowOffset;
            _originalYZMagnitude = Mathf.Sqrt(_originalOffset.y * _originalOffset.y + _originalOffset.z * _originalOffset.z);
            RecalculateMagnitudeConstantMultiplier();
            _lastScaleUsed = -1.0f;
            _fogStartCameraDistance = RenderSettings.fogStartDistance - _originalYZMagnitude;
            _fogEndCameraDistance = RenderSettings.fogEndDistance - _originalYZMagnitude;
            _camera.Follow = PlayerManager.Instance.GetHumanPlayer().transform;
            _camera.LookAt = PlayerManager.Instance.GetHumanPlayer().transform;
        }

        public void RecalculateMagnitudeConstantMultiplier()
        {
            float aspect = (float) Screen.width / Screen.height;
            float verticalFOV = _camera.m_Lens.FieldOfView;
            float horizontalFOV = 2 * Mathf.Atan(Mathf.Tan(verticalFOV * NumberConstants.Half * Mathf.Deg2Rad) * aspect) * Mathf.Rad2Deg;
            _magnitudeConstantMultiplier = PlayerManager.Instance.GetHumanPlayer().GetFactorOfPlayerRadiusToSee() / (_originalYZMagnitude * Mathf.Tan(horizontalFOV * Mathf.Deg2Rad * NumberConstants.Half));
        }

        public CinemachineVirtualCamera Camera()
        {
            return _camera;
        }

        public void ForceOffset(float radiusFromPlayer)
        {
            ApplyOffset(radiusFromPlayer * _magnitudeConstantMultiplier);
        }
        
        public void UpdateOffset(float radiusFromPlayer)
        {
            float scale = radiusFromPlayer * _magnitudeConstantMultiplier;

            if (radiusFromPlayer * PlayerManager.Instance.GetHumanPlayer().GetFactorOfPlayerRadiusToSee() > _maxZoomOutScaleAllowed)
            {
                scale = _maxZoomOutScaleAllowed * _magnitudeConstantMultiplier / PlayerManager.Instance.GetHumanPlayer().GetFactorOfPlayerRadiusToSee();
            }
            
            if (_lastScaleUsed < 0.0f)
            {
                ApplyOffset(scale);
                return;
            }

            StartCoroutine(ZoomOutProgressive(scale));
        }

        private void ApplyOffset(float scale)
        {
            _lastScaleUsed = scale;
            float offsetY = _originalOffset.y * scale;
            float offsetZ = _originalOffset.z * scale;
            _cinemachineTransposer.m_FollowOffset = new Vector3(_originalOffset.x, offsetY, offsetZ);
            float newYZMagnitude = Mathf.Sqrt(offsetY * offsetY + offsetZ * offsetZ);
            RenderSettings.fogStartDistance = _fogStartCameraDistance + newYZMagnitude;
            RenderSettings.fogEndDistance = _fogEndCameraDistance + newYZMagnitude;
        }

        private IEnumerator ZoomOutProgressive(float newScale)
        {
            float elapsed = 0f;
            float originalLastScaleUsed = _lastScaleUsed;

            while (elapsed < _zoomOutTime)
            {
                float scale = Mathf.Lerp(originalLastScaleUsed, newScale, elapsed / _zoomOutTime);
                ApplyOffset(scale);
                elapsed += Time.deltaTime;
                yield return null;
            }

            ApplyOffset(newScale);
        }
    }
}