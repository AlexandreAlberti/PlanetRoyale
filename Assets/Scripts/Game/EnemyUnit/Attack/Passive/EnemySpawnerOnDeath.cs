using System.Collections;
using System.Collections.Generic;
using Game.BaseUnit;
using Game.Pool;
using UnityEngine;

namespace Game.EnemyUnit.Attack.Passive
{
    [RequireComponent(typeof(UnitVisuals))]
    [RequireComponent(typeof(EnemyVisuals))]
    public class EnemySpawnerOnDeath : EnemyPassiveAttack
    {
        [SerializeField] private int _quantity;
        [SerializeField] private float _spawnRadius;
        [SerializeField] private PooledEnemyConfig _enemyConfig;
        [SerializeField] private GameObject _dieVfxPrefab;
        [SerializeField] private GameObject _appearVfxPrefab;

        private Enemy _enemy;
        private UnitVisuals _unitVisuals;
        private EnemyVisuals _enemyVisuals;

        private void Awake()
        {
            _enemy = GetComponent<Enemy>();
            _unitVisuals = GetComponent<UnitVisuals>();
            _enemyVisuals = GetComponent<EnemyVisuals>();
        }

        public override void Initialize()
        {
            _unitVisuals.DisableDeathAnimation();
            ObjectPool.Instance.RegisterPrefab(_enemyConfig.EnemyPrefab.gameObject, _enemyConfig.EnemyPrewarmCount);
            ObjectPool.Instance.RegisterPrefab(_enemyConfig.EnemyRagdollPrefab.gameObject, _enemyConfig.EnemyRagdollPrewarmCount);
            _enemy.OnDead += Enemy_OnDead;
            Enable();
        }

        private void Enemy_OnDead(Enemy enemy, int heroIndex)
        {
            _enemy.OnDead -= Enemy_OnDead;

            if (!_isEnabled)
            {
                return;
            }

            for (int i = 0; i < _quantity; i++)
            {
                EnemyManager.Instance.SpawnEnemyInsideRadius(_enemyConfig.EnemyPrefab, _enemy.GetTier(), GetInstanceID(), transform.position, _spawnRadius);
            }

            _enemyVisuals.SpawnVfx(transform, _dieVfxPrefab);

            List<Enemy> spawnedEnemies = EnemyManager.Instance.GetSpawnedEnemies(GetInstanceID());

            foreach (Enemy spawnedEnemy in spawnedEnemies)
            {
                _enemyVisuals.SpawnVfx(spawnedEnemy.transform, _appearVfxPrefab);
            }
        }
    }
}