using System;
using System.Collections;
using Application.Utils;
using Game.Enabler;
using UnityEngine;

namespace Game.PlayerUnit
{
    [RequireComponent(typeof(PlayerStats))]
    public abstract class SphericalMovement : EnablerMonoBehaviour
    {
        [SerializeField] private float _collisionCheckRadius;
        [SerializeField] private LayerMask _obstacleMask;
        
        protected const float MinInputMovement = 0.1f;
        private const float MinMovementDirection = 0.0001f;
        private const float PushBackDuration = 0.2f;
        private const float MinProjectedMovement = 0.01f;
        protected bool _isMoving;
        protected Vector3 _sphereCenter;
        protected float _sphereRadius;
        protected Vector3 _forwardDirection;
        private PlayerStats _playerStats;

        public Action<Vector3, Vector3, Vector3> OnMoved;

        protected virtual void Awake()
        {
            _playerStats = GetComponent<PlayerStats>();
        }
        
        public void PushBack(float pushBackForce, Vector3 pushBackDirection)
        {
            if (pushBackForce <= 0.0f || pushBackDirection == Vector3.zero)
            {
                return;
            }

            StartCoroutine(PushBackChoreography(pushBackForce, pushBackDirection));
        }

        private IEnumerator PushBackChoreography(float pushBackForce, Vector3 pushBackDirection)
        {
            _isMoving = false;

            Vector3 upDirection = (transform.position - _sphereCenter).normalized;
            Vector3 tangentDirection = Vector3.ProjectOnPlane(pushBackDirection.normalized, upDirection).normalized;

            if (tangentDirection.sqrMagnitude < MinMovementDirection)
            {
                yield break;
            }

            Vector3 totalPushback = tangentDirection * pushBackForce;

            float pushBackTimeElapsed = 0.0f;

            while (pushBackTimeElapsed < PushBackDuration)
            {
                float deltaTime = Time.deltaTime;
                pushBackTimeElapsed += deltaTime;

                Vector3 framePush = totalPushback * (deltaTime / PushBackDuration);
                MoveAroundSphere(framePush);

                Vector3 newUpDirection = (transform.position - _sphereCenter).normalized;
                RepositionIntoSphereSurface(newUpDirection);
                RotateToNewForwardDirection(newUpDirection);
                OnMoved?.Invoke(_forwardDirection.normalized, framePush.normalized, newUpDirection);

                yield return null;
            }
        }
        
        protected void RepositionIntoSphereSurface(Vector3 newUpDirection)
        {
            transform.position = _sphereCenter + newUpDirection * _sphereRadius;
        }

        protected void RotateToNewForwardDirection(Vector3 newUpDirection)
        {
            transform.rotation = Quaternion.LookRotation(_forwardDirection, newUpDirection);
        }

        public bool IsMoving()
        {
            return _isMoving;
        }

        public float GetCurrentSpeed()
        {
            return _isMoving ? GetMoveSpeed() : NumberConstants.Zero;
        }

        protected Vector3 CalculateMoveVector(Vector3 movementDirection)
        {
            CalculateForwardAndUpDirections(out Vector3 forwardDirection, out Vector3 upDirection);
            Vector3 rightDirection = Vector3.ProjectOnPlane(transform.right, upDirection).normalized;
            float moveAmount = GetMoveSpeed() * Time.deltaTime;
            return forwardDirection * (movementDirection.z * moveAmount) + rightDirection * (movementDirection.x * moveAmount);
        }

        public void CalculateForwardAndUpDirections(out Vector3 forwardDirection, out Vector3 upDirection)
        {
            upDirection = (transform.position - _sphereCenter).normalized;
            forwardDirection = Vector3.ProjectOnPlane(transform.forward, upDirection).normalized;
        }

        private float GetMoveSpeed()
        {
            return _playerStats.GetMovementSpeed();
        }

        protected void MoveAroundSphere(Vector3 movement)
        {
            if (!_isEnabled || movement.sqrMagnitude < MinMovementDirection)
            {
                return;
            }

            Vector3 currentPosition = transform.position;
            Vector3 moveDirection = movement.normalized;
            float moveDistance = movement.magnitude;

            if (Physics.SphereCast(currentPosition, _collisionCheckRadius, moveDirection, out RaycastHit hit, moveDistance, _obstacleMask))
            {
                Vector3 hitNormal = hit.normal;
                Vector3 slideDirection = Vector3.ProjectOnPlane(moveDirection, hitNormal).normalized;
                Vector3 up = (transform.position - _sphereCenter).normalized;
                Vector3 tangentSlideDirection = Vector3.ProjectOnPlane(slideDirection, up).normalized;
                Vector3 slideMovement = tangentSlideDirection * moveDistance;

                if (!Physics.SphereCast(currentPosition, _collisionCheckRadius, tangentSlideDirection, out RaycastHit slideHit, moveDistance, _obstacleMask))
                {
                    transform.position += slideMovement;
                }
                else
                {
                    float safeDistance = hit.distance - MinProjectedMovement;
                    
                    if (safeDistance > NumberConstants.Zero)
                    {
                        transform.position += moveDirection * safeDistance;
                    }
                }
            }
            else
            {
                transform.position += movement;
            }
        }
    }
}