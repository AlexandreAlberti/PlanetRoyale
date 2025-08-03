using Game.Damage;
using UnityEngine;

namespace Game.EnemyUnit.Attack.Passive
{
    public class EnemyBossAura : EnemyPassiveAttack
    {
        [SerializeField] private PlayerDamageArea _damageArea;
        [SerializeField] private int _damage;
        
        public override void Initialize()
        {
            _damageArea.Initialize(_damage);
        }

    }
}