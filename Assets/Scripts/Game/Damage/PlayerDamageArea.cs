using Application.Data.Progression;
using Game.PlayerUnit;
using UnityEngine;

namespace Game.Damage
{
    public class PlayerDamageArea : DamageArea
    {
        protected float _enemyDamageMultiplier;

        public override void Initialize(int damage)
        {
            base.Initialize(damage);
            _enemyDamageMultiplier = LeaguesManager.Instance.GetCurrentLeagueData().EnemyDamageMultiplier;
        }
        
        protected override void DamageTarget(Collider other)
        {
            Player player = other.GetComponent<Player>();

            if (player)
            {
                player.Damage(Mathf.FloorToInt(_damage * _enemyDamageMultiplier), _damageType, _damageEffect);
            }   
        }
    }
}