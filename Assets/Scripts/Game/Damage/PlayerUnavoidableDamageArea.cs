using Game.PlayerUnit;
using UnityEngine;

namespace Game.Damage
{
    public class PlayerUnavoidableDamageArea : PlayerDamageArea
    {
        [Range(0, 1)]
        [SerializeField] private float _damageFactorForNpc;
        
        protected override void DamageTarget(Collider other)
        {
            Player player = other.GetComponent<Player>();

            if (!player)
            {
                return;
            }

            float damage = _damage * _enemyDamageMultiplier;
            
            if (player.GetPlayerIndex() != Player.GetHumanPlayerIndex())
            {
                player.UnavoidableDamage(Mathf.FloorToInt(damage * _damageFactorForNpc), _damageType, _damageEffect);
                return;
            }
            
            player.UnavoidableDamage(Mathf.FloorToInt(damage), _damageType, _damageEffect);
        }
    }
}