using Game.EnemyUnit;
using UnityEngine;

namespace Game.Damage
{
    public class EnemyDamageArea : DamageArea
    {
        private int _playerIndex;
        
        public void Initialize(int playerIndex, int damage)
        {
            base.Initialize(damage);
            _playerIndex = playerIndex;
        }
        
        protected override void DamageTarget(Collider other)
        {
            Enemy enemy = other.GetComponent<Enemy>();

            if (enemy)
            {
                enemy.Damage(_damage, _damageType, _damageEffect, _playerIndex);
            }   
        }
    }
}