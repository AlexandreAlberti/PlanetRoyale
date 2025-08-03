using System;
using Game.Pool;
using Game.Vfx;
using UnityEngine;

namespace Game.EnemyUnit.Attack.Passive
{
    [RequireComponent(typeof(Enemy))]
    [RequireComponent(typeof(EnemyVisuals))]
    public class EnemyMinionSpawner : EnemyPassiveAttack
    {
        [SerializeField] private float _minionSpawnCooldown;
        [SerializeField] private int _minionQuantityPerSpawn;
        [SerializeField] private int _maxMinionQuantity;
        [SerializeField] protected float _minionSpawnRadius;
        [SerializeField] protected PooledEnemyConfig _minionEnemyConfig;
        [SerializeField] private GameObject _spawnerVfxPrefab;
        [SerializeField] private GameObject _spawnedVfxPrefab;

        private float _spawnTimer;
        private int _spawnedMinions;
        private int _creatorInstanceId;
        private Enemy _enemy;
        private EnemyVisuals _enemyVisuals;

        private void Awake()
        {
            _enemy = GetComponent<Enemy>();
            _enemyVisuals = GetComponent<EnemyVisuals>();
        }

        public override void Initialize()
        {
            _spawnedMinions = 0;
            _creatorInstanceId = GetInstanceID();
            ObjectPool.Instance.RegisterPrefab(_minionEnemyConfig.EnemyPrefab.gameObject, _minionEnemyConfig.EnemyPrewarmCount);
            ObjectPool.Instance.RegisterPrefab(_minionEnemyConfig.EnemyRagdollPrefab.gameObject, _minionEnemyConfig.EnemyRagdollPrewarmCount);
            EnemyManager.Instance.OnEnemyDead += EnemyManager_OnEnemyDead;
            EnemyManager.Instance.OnMinionSpawned += EnemyManager_OnMinionSpawned;
            _enemy.OnDead += Enemy_OnDead;
            Enable();
        }

        private void EnemyManager_OnMinionSpawned(Enemy enemy)
        {
            if (!_isEnabled)
            {
                return;
            }

            if (enemy.GetCreatorInstanceId() != _creatorInstanceId)
            {
                return;
            }
            
            enemy.SetTier(_enemy.GetTier());
            
            if (_spawnedVfxPrefab)
            {
                _enemyVisuals.SpawnVfx(enemy.transform, _spawnedVfxPrefab);
            }
        }

        protected virtual void Enemy_OnDead(Enemy enemy, int heroIndex)
        {
            EnemyManager.Instance.OnEnemyDead -= EnemyManager_OnEnemyDead;
        }

        private void EnemyManager_OnEnemyDead(Enemy enemy)
        {
            if (enemy.GetCreatorInstanceId() != _creatorInstanceId)
            {
                return;
            }

            _spawnedMinions--;
        }

        private void Update()
        {
            if (!_isEnabled)
            {
                return;
            }

            _spawnTimer += Time.deltaTime;

            if (_spawnTimer >= _minionSpawnCooldown)
            {
                _spawnTimer = 0.0f;

                if (_spawnedMinions >= _maxMinionQuantity)
                {
                    return;
                }

                int minionsToSpawn = Math.Min(_minionQuantityPerSpawn, _maxMinionQuantity - _spawnedMinions);

                for (int i = 0; i < minionsToSpawn; i++)
                {
                    _spawnedMinions++;
                    SpawnMinionLogic();
                }

                if (_spawnerVfxPrefab)
                {
                    _enemyVisuals.SpawnVfx(transform, _spawnerVfxPrefab);
                }
            }
        }

        protected virtual void SpawnMinionLogic()
        {
            if (!_isEnabled)
            {
                return;
            }

            EnemyManager.Instance.SpawnEnemyInsideRadius(_minionEnemyConfig.EnemyPrefab, _enemy.GetTier(), GetInstanceID(), transform.position, _minionSpawnRadius);
        }
    }
}