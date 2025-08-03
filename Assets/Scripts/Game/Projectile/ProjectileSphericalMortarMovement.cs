using System;
using UnityEngine;

namespace Game.Projectile
{
    public class ProjectileSphericalMortarMovement : ProjectileSphericalMovement
    {
        [SerializeField] private AnimationCurve _animationCurve;
        [SerializeField] private float _mortarHeight;
        [SerializeField] private float _mortarDuration;
        [Range(0, 1)]
        [SerializeField] private float _mortarShadowScaleReductionAtArcMax;
        

        public Action<ProjectileSphericalMortarMovement> OnMortarEndedMovement;

        private float _currentMortarTime;
        private float _distanceToEnemy;
        private Vector3 _initialMortarShadowScale;

        private void Awake()
        {
            _initialMortarShadowScale = _shadow.localScale;
        }

        public override void Initialize(Vector3 direction, float distanceToEnemy, Vector3 sphereCenter, float sphereRadius)
        {
            base.Initialize(direction, distanceToEnemy, sphereCenter, sphereRadius);
            _currentMortarTime = 0.0f;
            _distanceToEnemy = distanceToEnemy;
            _speedToMove = _distanceToEnemy / _mortarDuration;
        }

        protected override void ExtraMovementToApply(Vector3 upDirection)
        {
            if (!_isEnabled)
            {
                return;
            }

            _currentMortarTime += Time.deltaTime;
            _currentMortarTime = Mathf.Min(_currentMortarTime, _mortarDuration);
            float animationCurvePoint = _animationCurve.Evaluate(_currentMortarTime / _mortarDuration);
            Vector3 moveUp = upDirection * (_mortarHeight * animationCurvePoint);
            transform.position += moveUp;
            _shadow.localScale = _initialMortarShadowScale - _initialMortarShadowScale * (_mortarShadowScaleReductionAtArcMax * animationCurvePoint);
            UpdateShadowTransform(upDirection);
            
            if (_currentMortarTime >= _mortarDuration)
            {
                Disable();
                OnMortarEndedMovement?.Invoke(this);
            }
        }
    }
}