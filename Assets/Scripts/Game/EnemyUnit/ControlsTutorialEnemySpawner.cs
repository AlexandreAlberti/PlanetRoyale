using System.Collections;
using System.Collections.Generic;
using Game.Map;
using Game.Pool;
using UnityEngine;

namespace Game.EnemyUnit
{
    public class ControlsTutorialEnemySpawner : MonoBehaviour
    {
        private List<EnemySpawnPoint> _wave1EnemyConfig;
        private List<EnemySpawnPoint> _wave2EnemyConfig;
        private List<Enemy> _wave1Enemies;
        private List<Enemy> _wave2Enemies;
        private int _remainingWaveEnemiesToSpawn;

        public static ControlsTutorialEnemySpawner Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        public void Initialize()
        {
            _wave1EnemyConfig = new List<EnemySpawnPoint>();
            _wave2EnemyConfig = new List<EnemySpawnPoint>();
            _wave1Enemies = new List<Enemy>();
            _wave2Enemies = new List<Enemy>();
            EnemySpawnPoint[] enemySpawnPoints = FindObjectsOfType<EnemySpawnPoint>();

            foreach (EnemySpawnPoint enemySpawnPoint in enemySpawnPoints)
            {
                if (enemySpawnPoint.WaveIndex() == 0)
                {
                    _wave1EnemyConfig.Add(enemySpawnPoint);
                }
                else
                {
                    _wave2EnemyConfig.Add(enemySpawnPoint);
                }

                enemySpawnPoint.gameObject.SetActive(false);

                ObjectPool.Instance.RegisterPrefab(enemySpawnPoint.EnemyPrefab().gameObject, 1);
                ObjectPool.Instance.RegisterPrefab(enemySpawnPoint.EnemyRagdollPrefab().gameObject, 1);
            }
        }

        public IEnumerator SpawnWave1()
        {
            _remainingWaveEnemiesToSpawn = _wave1EnemyConfig.Count;

            foreach (EnemySpawnPoint enemySpawnPoint in _wave1EnemyConfig)
            {
                StartCoroutine(WaitForSpawnEnemyChoreographyEndAndDecreaseRemainingEnemiesToSpawn(enemySpawnPoint.transform.position, enemySpawnPoint.EnemyPrefab(), enemySpawnPoint.EnemyRagdollPrefab(),
                    EnemyManager.Instance.GetEnemyMarkSpawner(), EnemyManager.Instance.GetEnemyRaySpawner(), _wave1Enemies, false));
            }

            while (_remainingWaveEnemiesToSpawn > 0)
            {
                yield return null;
            }
        }

        public IEnumerator SpawnWave2()
        {
            _remainingWaveEnemiesToSpawn = _wave2EnemyConfig.Count;

            foreach (EnemySpawnPoint enemySpawnPoint in _wave2EnemyConfig)
            {
                StartCoroutine(WaitForSpawnEnemyChoreographyEndAndDecreaseRemainingEnemiesToSpawn(enemySpawnPoint.transform.position, enemySpawnPoint.EnemyPrefab(), enemySpawnPoint.EnemyRagdollPrefab(),
                    EnemyManager.Instance.GetEnemyMarkSpawner(), EnemyManager.Instance.GetEnemyRaySpawner(), _wave2Enemies, true));
            }

            while (_remainingWaveEnemiesToSpawn > 0)
            {
                yield return null;
            }
        }

        private IEnumerator WaitForSpawnEnemyChoreographyEndAndDecreaseRemainingEnemiesToSpawn(Vector3 spawnPosition, Enemy enemyPrefab, EnemyRagdoll enemyRagdollPrefab, EnemyMarkSpawner spawnMarkVfxPrefab, EnemyRaySpawner spawnRayVfxPrefab, List<Enemy> enemyList, bool doInitializeWithAiMovement)
        {
            yield return StartCoroutine(EnemyManager.Instance.SpawnEnemyChoreography(spawnPosition, enemyPrefab, enemyRagdollPrefab, spawnMarkVfxPrefab, spawnRayVfxPrefab, Tier.None, enemyList: enemyList, creatorInstanceId: GetInstanceID(), doInitializeWithAiMovement: doInitializeWithAiMovement));
            _remainingWaveEnemiesToSpawn--;
        }

        public List<Enemy> GetEnemiesWave1()
        {
            return _wave1Enemies;
        }

        public List<Enemy> GetEnemiesWave2()
        {
            return _wave2Enemies;
        }

        public void RemoveEnemyFromWave1(Enemy enemy)
        {
            _wave1Enemies.Remove(enemy);
        }

        public void RemoveEnemyFromWave2(Enemy enemy)
        {
            _wave2Enemies.Remove(enemy);
        }
    }
}