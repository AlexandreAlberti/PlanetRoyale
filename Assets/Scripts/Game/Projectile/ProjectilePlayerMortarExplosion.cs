using Game.Damage;
using Game.EnemyUnit;
using UnityEngine;

namespace Game.Projectile
{
    public class ProjectilePlayerMortarExplosion : ProjectileMortarExplosion
    {
        protected void OnTriggerEnter(Collider other)
        {
            if (!_isEnabled)
            {
                return;
            }

            Enemy enemy = other.GetComponent<Enemy>();

            if (enemy)
            {
                enemy.Damage(_damage, DamageType.Ranged, DamageEffect.None, _playerIndex);
            }
        }
    }
}