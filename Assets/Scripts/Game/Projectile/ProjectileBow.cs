using Game.BaseObstacle;
using Game.Damage;
using Game.Detectors;
using Game.EnemyUnit;
using Game.PlayerUnit;
using UnityEngine;

namespace Game.Projectile
{
    [RequireComponent(typeof(EnemyDetectorForProjectiles))]
    [RequireComponent(typeof(ProjectileSphericalMovement))]
    public class ProjectileBow : ProjectileBase
    {
        [SerializeField] private float[] _ricochetShotJumpMultipliers;

        private EnemyDetectorForProjectiles _enemyDetectorForProjectiles;
        private int _ricochetShotJumps;

        protected override void Awake()
        {
            base.Awake();
            _enemyDetectorForProjectiles = GetComponent<EnemyDetectorForProjectiles>();
        }

        public override void Initialize(Vector3 position, Vector3 direction, float distanceToTarget, Vector3 sphereCenter, float sphereRadius, int damage, GameObject projectilePrefab, int playerIndex)
        {
            base.Initialize(position, direction, distanceToTarget, sphereCenter, sphereRadius, damage, projectilePrefab, playerIndex);

            _enemyDetectorForProjectiles.Initialize(false, transform);
            _ricochetShotJumps = 0;
        }

        protected override void OnTriggerEnter(Collider other)
        {
            if (!_isEnabled)
            {
                return;
            }

            Obstacle obstacle = other.GetComponent<Obstacle>();

            if (obstacle)
            {
                Destroy();
                return;
            }

            Enemy enemy = other.GetComponent<Enemy>();

            if (enemy)
            {
                enemy.Damage(Mathf.CeilToInt(_initialDamage * _ricochetShotJumpMultipliers[_ricochetShotJumps]), DamageType.Ranged, DamageEffect.None, _heroIndex);
                RicochetShot(enemy);
            }
        }

        private void RicochetShot(Enemy currentEnemy)
        {
            _ricochetShotJumps++;

            if (_ricochetShotJumps >= _ricochetShotJumpMultipliers.Length)
            {
                Destroy();
                return;
            }
            
            _enemyDetectorForProjectiles.FindClosestUnit(currentEnemy.GetUnit());
            
            Enemy enemy = _enemyDetectorForProjectiles.GetClosestEnemy();

            if (!enemy)
            {
                currentEnemy.Damage(Mathf.CeilToInt(_initialDamage * _ricochetShotJumpMultipliers[_ricochetShotJumps]), DamageType.Environment, DamageEffect.None, _heroIndex);
                RicochetShot(currentEnemy);
                return;
            }

            _projectileSphericalMovement.UpdateDirection(enemy.transform.position - transform.position);
        }
    }
}