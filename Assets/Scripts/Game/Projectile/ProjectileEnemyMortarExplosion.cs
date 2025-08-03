using Application.Data.Progression;
using Game.Damage;
using Game.EnemyUnit;
using Game.PlayerUnit;
using UnityEngine;

namespace Game.Projectile
{
    public class ProjectileEnemyMortarExplosion : ProjectileMortarExplosion
    {
        [SerializeField] private bool _generatePushBack;
        [SerializeField] private float _pushBackForce;

        private float _enemyDamageMultiplier;

        public override void Initialize(int playerIndex, int damage)
        {
            base.Initialize(playerIndex, damage);
            _enemyDamageMultiplier = LeaguesManager.Instance.GetCurrentLeagueData().EnemyDamageMultiplier;
        }

        protected void OnTriggerEnter(Collider other)
        {
            if (!_isEnabled)
            {
                return;
            }

            Player player = other.GetComponent<Player>();

            if (player)
            {
                player.Damage(Mathf.CeilToInt(_damage * _enemyDamageMultiplier), DamageType.Ranged, DamageEffect.None);

                if (_generatePushBack)
                {
                    Vector3 pushBackDirection = player.transform.position - transform.position;
                    player.PushBack(_pushBackForce, pushBackDirection);
                }
            }

            if (_generatePushBack)
            {
                Enemy enemy = other.GetComponent<Enemy>();

                if (enemy && !EnemyManager.Instance.IsBoss(enemy))
                {
                    Vector3 pushBackDirection = enemy.transform.position - transform.position;
                    enemy.PushBack(_pushBackForce, pushBackDirection);
                }
            }
        }
    }
}