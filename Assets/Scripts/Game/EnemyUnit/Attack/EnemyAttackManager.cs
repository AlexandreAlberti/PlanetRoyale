using System.Collections.Generic;
using Game.Enabler;
using Game.EnemyUnit.Attack.Active;
using Game.EnemyUnit.Attack.Passive;
using UnityEngine;

namespace Game.EnemyUnit.Attack
{
    public class EnemyAttackManager : EnablerMonoBehaviour
    {
        private List<EnemyActiveAttack> _enemyActiveAttacks;
        private List<EnemyPassiveAttack> _enemyPassiveAttacks;

        private Vector3 _sphereCenter;
        private float _sphereRadius;

        private void Awake()
        {
            _enemyActiveAttacks = new List<EnemyActiveAttack>();
            
            foreach (EnemyActiveAttack enemyActiveAttack in GetComponents<EnemyActiveAttack>())
            {
                _enemyActiveAttacks.Add(enemyActiveAttack);
            }

            _enemyPassiveAttacks = new List<EnemyPassiveAttack>();
            
            foreach (EnemyPassiveAttack enemyPassiveAttack in GetComponents<EnemyPassiveAttack>())
            {
                _enemyPassiveAttacks.Add(enemyPassiveAttack);
            }
        }

        public void Initialize(Vector3 sphereCenter, float sphereRadius)
        {
            _sphereCenter = sphereCenter;
            _sphereRadius = sphereRadius;
            
            foreach (EnemyActiveAttack enemyAttack in _enemyActiveAttacks)
            {
                enemyAttack.Initialize();
            }
            
            foreach (EnemyPassiveAttack enemyAttack in _enemyPassiveAttacks)
            {
                enemyAttack.Initialize();

                if (enemyAttack is EnemyMinionSpawnerThroughProjectile enemyMinionSpawnerThroughProjectile)
                {
                    enemyMinionSpawnerThroughProjectile.SetSphereParameters(_sphereCenter, _sphereRadius);
                }
            }
        }

        public override void Enable()
        {
            base.Enable();
            
            foreach (EnemyActiveAttack enemyAttack in _enemyActiveAttacks)
            {
                enemyAttack.Enable();
            }
            
            foreach (EnemyPassiveAttack enemyAttack in _enemyPassiveAttacks)
            {
                enemyAttack.Enable();
            }
        }

        public override void Disable()
        {
            base.Disable();
            
            foreach (EnemyActiveAttack enemyAttack in _enemyActiveAttacks)
            {
                enemyAttack.Disable();
            }
            
            foreach (EnemyPassiveAttack enemyAttack in _enemyPassiveAttacks)
            {
                enemyAttack.Disable();
            }
        }

        public void UseActiveAttackByIndex(int attackIndex)
        {
            if (!_isEnabled)
            {
                return;
            }

            if (_enemyActiveAttacks.Count <= attackIndex)
            {
                return;
            }
            
            _enemyActiveAttacks[attackIndex].Attack(_sphereCenter, _sphereRadius);
        }

        public float GetActiveAttackDurationByIndex(int attackIndex)
        {
            if (_enemyActiveAttacks.Count <= attackIndex)
            {
                return 0.0f;
            }
            
            return _enemyActiveAttacks[attackIndex].GetAttackDuration();
        }
    }
}