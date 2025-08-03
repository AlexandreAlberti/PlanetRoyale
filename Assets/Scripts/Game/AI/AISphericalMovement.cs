using Application.Utils;
using Game.PlayerUnit;
using UnityEngine;

namespace Game.AI
{
    [RequireComponent(typeof(AIInputManager))]
    public class AISphericalMovement : SphericalMovement
    {
        private AIInputManager _aiInputManager;

        protected override void Awake()
        {
            base.Awake();
            _aiInputManager = GetComponent<AIInputManager>();
        }

        public void Initialize(Vector3 sphereCenter, float sphereRadius)
        {
            _sphereCenter = sphereCenter;
            _sphereRadius = sphereRadius;
            Vector3 up = transform.position - _sphereCenter;
            _forwardDirection = Vector3.ProjectOnPlane(transform.forward, up).normalized;
            _aiInputManager.Initialize();
            AIInputManager_OnDragVector(Vector2.up);
        }

        public override void Enable()
        {
            base.Enable();
            _aiInputManager.Enable();
            _aiInputManager.OnDragVector += AIInputManager_OnDragVector;
            _aiInputManager.OnDragStarted += AIInputManager_OnDragStarted;
            _aiInputManager.OnDragStopped += AIInputManager_OnDragStopped;
        }

        public override void Disable()
        {
            _aiInputManager.OnDragVector -= AIInputManager_OnDragVector;
            _aiInputManager.OnDragStarted -= AIInputManager_OnDragStarted;
            _aiInputManager.OnDragStopped -= AIInputManager_OnDragStopped;
            _aiInputManager.Disable();
            base.Disable();
        }

        private void AIInputManager_OnDragStarted()
        {
            _isMoving = true;
        }

        private void AIInputManager_OnDragStopped()
        {
            _isMoving = false;
        }

        private void AIInputManager_OnDragVector(Vector2 input)
        {
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