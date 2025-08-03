using Application.Data.Progression;
using Game.BaseObstacle;
using Game.Damage;
using Game.PlayerUnit;
using UnityEngine;

namespace Game.Projectile
{
    [RequireComponent(typeof(ProjectileSphericalMovement))]
    public class ProjectileSnake : ProjectileBase
    {
        [SerializeField] private bool _provokesPoison;

        private float _enemyDamageMultiplier;

        public override void Initialize(Vector3 position, Vector3 direction, float distanceToTarget, Vector3 sphereCenter, float sphereRadius, int damage, GameObject projectilePrefab, int playerIndex)
        {
            base.Initialize(position, direction, distanceToTarget, sphereCenter, sphereRadius, damage, projectilePrefab, playerIndex);
            _enemyDamageMultiplier = LeaguesManager.Instance.GetCurrentLeagueData().EnemyDamageMultiplier;
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

            Player player = other.GetComponent<Player>();

            if (!player)
            {
                return;
            }
            
            player.Damage(Mathf.CeilToInt(_initialDamage * _enemyDamageMultiplier), DamageType.Ranged, _provokesPoison ? DamageEffect.Poison : DamageEffect.None);
                
            Destroy();
        }
    }
}