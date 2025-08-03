using System.Collections.Generic;
using Game.BaseUnit;
using Game.Damage;
using Game.EnemyUnit;
using UnityEngine;

namespace Game.PlayerUnit
{
    public class PlayerAttackTrigger : UnitAttackTrigger
    {
        private PlayerStats _playerStats;
        private List<Enemy> _damagedEnemies;
        private float _pushBackForce;
        private int _playerIndex;

        public void Initialize(int playerIndex, PlayerStats playerStats, DamageEffect damageEffect, int damage, int amountOfParticles, float pushBackForce)
        {
            base.Initialize(damageEffect, damage, amountOfParticles);
            _playerStats = playerStats;
            _playerIndex = playerIndex;
            _pushBackForce = pushBackForce;
        }

        
        public override void Enable()
        {
            base.Enable();
            _damagedEnemies = new List<Enemy>();
            gameObject.SetActive(true);
        }

        public override void Disable()
        {
            base.Disable();
            _damagedEnemies = new List<Enemy>();
            gameObject.SetActive(false);
        }

        protected override void OnTriggerStay(Collider other)
        {
            if (!_isEnabled)
            {
                return;
            }

            Enemy enemy = other.GetComponent<Enemy>();

            if (enemy && !_damagedEnemies.Contains(enemy))
            {
                enemy.Damage(_playerStats.GetAttack(), DamageType.Melee, _damageEffect, _playerIndex);
                _damagedEnemies.Add(enemy);
                
                Vector3 pushBackDirection = enemy.transform.position - transform.position;

                if (!enemy.IsBoss())
                {
                    enemy.PushBack(_pushBackForce, pushBackDirection);
                }

            }
        }
    }
}