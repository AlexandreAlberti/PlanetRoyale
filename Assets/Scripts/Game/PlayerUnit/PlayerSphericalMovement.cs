using Application.Utils;
using Game.Input;
using UnityEngine;

namespace Game.PlayerUnit
{
    [RequireComponent(typeof(PlayerStats))]
    public class PlayerSphericalMovement : SphericalMovement
    {
        public void Initialize(Vector3 sphereCenter, float sphereRadius)
        {
            _sphereCenter = sphereCenter;
            _sphereRadius = sphereRadius;
            Vector3 up = transform.position - _sphereCenter;
            _forwardDirection = Vector3.ProjectOnPlane(transform.forward, up).normalized;
            Enable();
        }

        public override void Enable()
        {
            base.Enable();
            InputManager.Instance.OnDragVector += InputManager_OnDragVector;
            InputManager.Instance.OnDragStarted += InputManager_OnDragStarted;
            InputManager.Instance.OnDragStopped += InputManager_OnDragStopped;
        }

        public override void Disable()
        {
            InputManager.Instance.OnDragVector -= InputManager_OnDragVector;
            InputManager.Instance.OnDragStarted -= InputManager_OnDragStarted;
            InputManager.Instance.OnDragStopped -= InputManager_OnDragStopped;
            base.Disable();
        }

        private void InputManager_OnDragStarted()
        {
            _isMoving = true;
        }

        private void InputManager_OnDragStopped()
        {
            _isMoving = false;
        }

        private void InputManager_OnDragVector(Vector2 input)
        {
            if (!_isEnabled)
            {
                return;
            }

            _isMoving = input != Vector2.zero;

            if (!_isMoving)
            {
                return;
            }

            Vector3 upDirection = (transform.position - _sphereCenter).normalized;
            _forwardDirection = Vector3.ProjectOnPlane(transform.forward, upDirection).normalized;

            Vector3 movementDirection = new Vector3(input.x, NumberConstants.Zero, input.y);

            if (Mathf.Abs(movementDirection.x) > MinInputMovement || Mathf.Abs(movementDirection.z) > MinInputMovement)
            {
                Vector3 movementVector = CalculateMoveVector(movementDirection);
                MoveAroundSphere(movementVector);
                Vector3 newUpDirection = (transform.position - _sphereCenter).normalized;
                RepositionIntoSphereSurface(newUpDirection);
                RotateToNewForwardDirection(newUpDirection);
                OnMoved?.Invoke(_forwardDirection.normalized, movementVector.normalized, newUpDirection);
            }
        }
    }
}