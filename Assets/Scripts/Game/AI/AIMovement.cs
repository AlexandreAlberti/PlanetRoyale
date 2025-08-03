using System.Collections;
using Game.Enabler;
using Opsive.BehaviorDesigner.Runtime;
using UnityEngine;
using UnityEngine.AI;

namespace Game.AI
{
    [RequireComponent(typeof(BehaviorTree))]
    public class AIMovement : EnablerMonoBehaviour
    {
        [SerializeField] private NavMeshAgent _navMeshAgent;
        
        private const float MinSpeedToAvoidFloatingPointIssues = 0.1f;
        private const int MinAvoidance = 0;
        private const int MaxAvoidance = 99;
        private const float PushBackDuration = 0.2f;
        
        private Vector3 _sphereCenter;
        private float _sphereRadius;
        private BehaviorTree _behaviorTree;
        private Coroutine _coroutineTravelMeshLink;
        
        private void Awake()
        {
            _behaviorTree = GetComponent<BehaviorTree>();
            _behaviorTree.enabled = false;
            _navMeshAgent.enabled = false;
            _navMeshAgent.avoidancePriority = Random.Range(MinAvoidance, MaxAvoidance);
        }

        public void Initialize(Vector3 sphereCenter, float sphereRadius)
        {
            _sphereCenter = sphereCenter;
            _sphereRadius = sphereRadius;
            _navMeshAgent.updateRotation = false;
        }
        
        private void Update()
        {
            if (!_isEnabled || !_behaviorTree.enabled || !_navMeshAgent.enabled || !_navMeshAgent.isOnNavMesh || _navMeshAgent.isStopped)
            {
                return;
            }
            
            if (IsMoving())
            {
                Vector3 upDirection = GetUpDirection();
                transform.rotation = Quaternion.LookRotation(GetForwardDirection(), upDirection);
            }
            
            if (_navMeshAgent.isOnOffMeshLink && _coroutineTravelMeshLink == null)
            {
                _coroutineTravelMeshLink = StartCoroutine(SmoothTraverseOffMeshLink());
            }
        }

        private IEnumerator SmoothTraverseOffMeshLink()
        {
            OffMeshLinkData linkData = _navMeshAgent.currentOffMeshLinkData;
            Vector3 startPosition = _navMeshAgent.transform.position;
            Vector3 endPosition = linkData.endPos + Vector3.up * _navMeshAgent.baseOffset;

            float speed = _navMeshAgent.speed;
            float distance = (endPosition - startPosition).magnitude;
            float duration = distance / speed; // Adjust for speed/smoothness
            float elapsed = 0f;

            while (elapsed < duration)
            {
                _navMeshAgent.transform.position = Vector3.Lerp(startPosition, endPosition, elapsed / duration);
                elapsed += Time.deltaTime;
                transform.rotation = Quaternion.LookRotation((endPosition - startPosition).normalized, GetUpDirection());
                yield return null;
            }

            _navMeshAgent.transform.position = endPosition;
            _coroutineTravelMeshLink = null;
            
            if (_navMeshAgent.enabled && _navMeshAgent.isOnNavMesh && !_navMeshAgent.isStopped)
            {
                _navMeshAgent.CompleteOffMeshLink();
            }
        }

        public override void Enable()
        {
            base.Enable();
            _behaviorTree.enabled = true;
            _navMeshAgent.enabled = true;
        }

        public override void Disable()
        {
            base.Disable();
            _behaviorTree.enabled = false;
            _navMeshAgent.enabled = false;
            
            if (_coroutinePushBack != null)
            {
                StopCoroutine(_coroutinePushBack);
            }
        }

        public float GetCurrentSpeed()
        {
            return _navMeshAgent.velocity.magnitude;
        }
        
        private Vector3 GetForwardDirection()
        {
            return _navMeshAgent.velocity.normalized;
        }

        public Vector3 GetUpDirection()
        {
            return (transform.position - _sphereCenter).normalized;
        }

        private bool IsMoving()
        {
            return _navMeshAgent.velocity.magnitude > MinSpeedToAvoidFloatingPointIssues;
        }

        public int GetNavmeshAgentAreaMask()
        {
            return _navMeshAgent.areaMask;
        }

        private Coroutine _coroutinePushBack;

        public void PushBack(float pushBackForce, Vector3 pushBackDirection)
        {
            if (_coroutinePushBack != null)
            {
                StopCoroutine(_coroutinePushBack);
            }

            _coroutinePushBack = StartCoroutine(PushBackChoreography(pushBackForce, pushBackDirection.normalized));
        }

        private IEnumerator PushBackChoreography(float force, Vector3 direction)
        {
            _navMeshAgent.isStopped = true;

            float pushBackTimeElapsed = 0.0f;
            Vector3 startPosition = transform.position;
            Vector3 targetPosition = startPosition + direction * force;

            while (pushBackTimeElapsed < PushBackDuration)
            {
                float t = pushBackTimeElapsed / PushBackDuration;
                transform.position = Vector3.Lerp(startPosition, targetPosition, t);
                pushBackTimeElapsed += Time.deltaTime;
                yield return null;
            }

            transform.position = targetPosition;

            if (_isEnabled)
            {
                _navMeshAgent.Warp(transform.position);
                _navMeshAgent.isStopped = false;
            }

            _coroutinePushBack = null;
        }
    }
}