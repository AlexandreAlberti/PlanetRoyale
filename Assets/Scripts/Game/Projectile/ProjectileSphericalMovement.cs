using Game.Enabler;
using UnityEngine;

namespace Game.Projectile
{
    public class ProjectileSphericalMovement : EnablerMonoBehaviour
    {
        [SerializeField] protected Transform _shadow;
        [SerializeField] private float _moveSpeed;

        private Vector3 _sphereCenter;
        private float _sphereRadius;
        protected float _speedToMove;
        private float _projectileInitialMovementRadius;
        private Vector3 _forwardDirection;

        public virtual void Initialize(Vector3 direction, float distanceToEnemy, Vector3 sphereCenter, float sphereRadius)
        {
            transform.forward = direction;
            _sphereCenter = sphereCenter;
            _sphereRadius = sphereRadius;
            _projectileInitialMovementRadius = (transform.position - _sphereCenter).magnitude;
            _forwardDirection = Vector3.ProjectOnPlane(transform.forward, (transform.position - _sphereCenter).normalized).normalized;
            _speedToMove = _moveSpeed;
            Enable();
        }

        protected virtual void Update()
        {
            if (!_isEnabled)
            {
                return;
            }
            
            Vector3 upDirection = (transform.position - _sphereCenter).normalized;
            _forwardDirection = Vector3.ProjectOnPlane(_forwardDirection, upDirection).normalized;
            transform.position += _forwardDirection * (_speedToMove * Time.deltaTime);
            upDirection = (transform.position - _sphereCenter).normalized;
            transform.position = _sphereCenter + upDirection * _projectileInitialMovementRadius;
            transform.rotation = Quaternion.LookRotation(_forwardDirection, upDirection);
            ExtraMovementToApply(upDirection);
        }

        public void UpdateDirection(Vector3 direction)
        {
            Vector3 upDirection = (transform.position - _sphereCenter).normalized;
            _forwardDirection = Vector3.ProjectOnPlane(direction, upDirection).normalized;
        }

        protected void UpdateShadowTransform(Vector3 upDirection)
        {
            _shadow.position = _sphereCenter + upDirection * _sphereRadius;
        }

        protected virtual void ExtraMovementToApply(Vector3 upDirection)
        {
            UpdateShadowTransform(upDirection);
        }
    }
}