using System.Collections;
using Game.Detectors;
using Game.PlayerUnit;
using UnityEngine;

namespace Game.EnemyUnit.Attack.Active
{
    [RequireComponent(typeof(PlayerDetector))]
    [RequireComponent(typeof(EnemyVisuals))]
    public abstract class EnemyActiveAttack : EnemyAttack
    {
        [SerializeField] protected float _attackStartTime;
        [SerializeField] protected int _damage;
        [SerializeField] protected int _attackAnimationIndex;

        private PlayerDetector _playerDetector;
        protected EnemyVisuals _enemyVisuals;
        protected Coroutine _attackChoreography;

        protected virtual void Awake()
        {
            _playerDetector = GetComponent<PlayerDetector>();
            _enemyVisuals = GetComponent<EnemyVisuals>();
        }

        public override void Initialize()
        {
            _playerDetector.Initialize(false, GetComponent<Enemy>().GetCenterAnchor());
        }

        public void Attack(Vector3 sphereCenter, float sphereRadius)
        {
            if (!_isEnabled)
            {
                return;
            }
            
            Player closestPlayer = _playerDetector.GetClosestPlayer();

            if (closestPlayer)
            {
                _attackChoreography = StartCoroutine(AttackChoreography(closestPlayer, sphereCenter, sphereRadius));
            }
        }

        public abstract float GetAttackDuration();

        protected abstract IEnumerator AttackChoreography(Player closestPlayer, Vector3 sphereCenter, float sphereRadius);

        protected void CalculateFaceToPlayerParams(Vector3 closestPlayerPosition, Vector3 sphereCenter, out Vector3 directionToPlayer, out Vector3 upDirection)
        {
            Vector3 vectorToPlayer = closestPlayerPosition - transform.position;
            directionToPlayer = vectorToPlayer.normalized;
            upDirection = (transform.position - sphereCenter).normalized;
        }
    }
}