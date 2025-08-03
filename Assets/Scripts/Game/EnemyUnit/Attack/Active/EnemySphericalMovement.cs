using System;
using Game.Enabler;
using UnityEngine;

namespace Game.EnemyUnit.Attack.Active
{
    public class EnemySphericalMovement : EnablerMonoBehaviour
    {
        [SerializeField] private float _moveSpeed;
        [SerializeField] private LayerMask _obstacleMask;

        private Vector3 _sphereCenter;
        private float _speedToMove;
        private float _projectileInitialMovementRadius;
        private Vector3 _forwardDirection;
        private CapsuleCollider _capsuleCollider;
        private RaycastHit[] _colliderHits;

        public Action OnCollisionFound;
        
        private void Awake()
        {
            _capsuleCollider = GetComponent<CapsuleCollider>();
        }

        public void Initialize(Vector3 direction, Vector3 sphereCenter)
        {
            transform.forward = direction;
            _sphereCenter = sphereCenter;
            _projectileInitialMovementRadius = (transform.position - _sphereCenter).magnitude;
            _forwardDirection = Vector3.ProjectOnPlane(transform.forward, (transform.position - _sphereCenter).normalized).normalized;
            _speedToMove = _moveSpeed;
            Enable();
        }

        protected void Update()
        {
            if (!_isEnabled)
            {
                return;
            }

            Vector3 upDirection = (transform.position - _sphereCenter).normalized;
            _forwardDirection = Vector3.ProjectOnPlane(_forwardDirection, upDirection).normalized;
            float amountToMove = _speedToMove * Time.deltaTime;

            float radius = _capsuleCollider.radius;
            Vector3 nextPosition = transform.position + _forwardDirection * amountToMove;

            if (Physics.CheckSphere(nextPosition, radius, _obstacleMask, QueryTriggerInteraction.Collide))
            {
                OnCollisionFound?.Invoke();
                _isEnabled = false;
                return;
            }

            transform.position = nextPosition;
            upDirection = (transform.position - _sphereCenter).normalized;
            transform.position = _sphereCenter + upDirection * _projectileInitialMovementRadius;
            transform.rotation = Quaternion.LookRotation(_forwardDirection, upDirection);
        }
    }
}