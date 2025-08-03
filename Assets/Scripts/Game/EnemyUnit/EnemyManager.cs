using System;
using System.Collections;
using System.Collections.Generic;
using Application.Utils;
using Game.Drop;
using Game.Enabler;
using Game.Match;
using Game.PlayerUnit;
using Game.Pool;
using Game.UI;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Game.EnemyUnit
{
    public class EnemyManager : EnablerMonoBehaviour
    {
        [SerializeField] private EnemyMarkSpawner _enemySpawnMarkVfxPrefab;
        [SerializeField] private EnemyRaySpawner _enemySpawnRayVfxPrefab;
        [SerializeField] private EnemyMarkSpawner _enemyBossSpawnMarkVfxPrefab;
        [SerializeField] private EnemyRaySpawner _enemyBossSpawnRayVfxPrefab;
        [SerializeField] private LayerMask _obstacleMask;
        [SerializeField] private float _timeBeforeInstantSpawn;

        private const int MaxAttempts = 1000;
        private const float MaxDistanceFromTestPoint = 0.1f;
        private static readonly Vector3 MatchTutorialBossPosition = new(-0.7f, 6.21f, 13.43f);

        private List<PooledEnemyConfig> _stage1ConfigList;
        private List<PooledEnemyConfig> _stage2ConfigList;
        private List<PooledEnemyConfig> _stage3ConfigList;
        private List<PooledEnemyConfig> _miniBossesConfigList;
        private List<PooledEnemyConfig> _bossesConfigList;
        private int _initialBatchSpawns;
        private float _spawnInterval;
        private int _stage1MaxSpawnsPerBatch;
        private int _stage1MaxEnemiesInPlanet;
        private int _stage2MaxSpawnsPerBatch;
        private int _stage2MaxEnemiesInPlanet;
        private int _stage3MaxSpawnsPerBatch;
        private int _stage3MaxEnemiesInPlanet;

        private float _spawnTimer;
        private List<Enemy> _enemyListStage1;
        private List<Enemy> _enemyListStage2;
        private List<Enemy> _enemyListStage3;
        private List<Enemy> _enemyTutorialList;
        private List<Enemy> _enemyMinionsList;
        private List<Enemy> _miniBossList;
        private List<Enemy> _bossList;
        private List<Transform> _miniBossSpawnerList;
        private List<Transform> _bossSpawnerList;
        private Vector3 _sphereCenter;
        protected float _sphereRadius;
        private Collider[] _colliderHits;
        private bool _hasSpawnedInitialBatch;
        private int _numberOfMiniBosses;
        private int _numberOfBosses;
        private Coroutine _spawnEnemyCoroutine;

        public Action<Enemy> OnEnemyDead;
        public Action<Enemy> OnMinionSpawned;
        public Action OnBossDead;

        public static EnemyManager Instance;

        private void Awake()
        {
            Instance = this;
        }

        public void Initialize(Vector3 sphereCenter, float sphereRadius)
        {
            _spawnTimer = _spawnInterval;
            _enemyListStage1 = new List<Enemy>();
            _enemyListStage2 = new List<Enemy>();
            _enemyListStage3 = new List<Enemy>();
            _enemyTutorialList = new List<Enemy>();
            _enemyMinionsList = new List<Enemy>();
            _miniBossList = new List<Enemy>();
            _bossList = new List<Enemy>();
            _miniBossSpawnerList = new List<Transform>();
            _bossSpawnerList = new List<Transform>();
            _sphereCenter = sphereCenter;
            _sphereRadius = sphereRadius;
            _colliderHits = new Collider[4];
            _hasSpawnedInitialBatch = false;

            _stage1ConfigList = MatchManager.Instance.SelectStage1List();
            _stage2ConfigList = MatchManager.Instance.SelectStage2List();
            _stage3ConfigList = MatchManager.Instance.SelectStage3List();
            _miniBossesConfigList = MatchManager.Instance.SelectMiniBossesList();
            _bossesConfigList = MatchManager.Instance.SelectBossesList();
            _initialBatchSpawns = MatchManager.Instance.InitialBatchSpawns();
            _spawnInterval = MatchManager.Instance.SpawnInterval();
            _stage1MaxSpawnsPerBatch = MatchManager.Instance.Stage1MaxSpawnsPerBatch();
            _stage1MaxEnemiesInPlanet = MatchManager.Instance.Stage1MaxEnemiesInPlanet();
            _stage2MaxSpawnsPerBatch = MatchManager.Instance.Stage2MaxSpawnsPerBatch();
            _stage2MaxEnemiesInPlanet = MatchManager.Instance.Stage2MaxEnemiesInPlanet();
            _stage3MaxSpawnsPerBatch = MatchManager.Instance.Stage3MaxSpawnsPerBatch();
            _stage3MaxEnemiesInPlanet = MatchManager.Instance.Stage3MaxEnemiesInPlanet();
            
            RegisterStagePrefabs(_stage1ConfigList);
            RegisterStagePrefabs(_stage2ConfigList);
            RegisterStagePrefabs(_stage3ConfigList);
            RegisterStagePrefabs(_miniBossesConfigList);
            RegisterStagePrefabs(_bossesConfigList);
            
            _numberOfMiniBosses = 0;
            _numberOfBosses = 0;
        }

        private void RegisterStagePrefabs(List<PooledEnemyConfig> stageConfigList)
        {
            foreach (PooledEnemyConfig stageConfigObject in stageConfigList)
            {
                ObjectPool.Instance.RegisterPrefab(stageConfigObject.EnemyPrefab.gameObject, stageConfigObject.EnemyPrewarmCount);
                ObjectPool.Instance.RegisterPrefab(stageConfigObject.EnemyRagdollPrefab.gameObject, stageConfigObject.EnemyRagdollPrewarmCount);
            }
        }

        public override void Enable()
        {
            base.Enable();
            
            foreach (Enemy enemy in _enemyListStage1)
            {
                enemy.Enable();
            }

            foreach (Enemy enemy in _enemyListStage2)
            {
                enemy.Enable();
            }

            foreach (Enemy enemy in _enemyListStage3)
            {
                enemy.Enable();
            }

            foreach (Enemy enemy in _enemyTutorialList)
            {
                enemy.Enable();
            }

            foreach (Enemy enemy in _enemyMinionsList)
            {
                enemy.Enable();
            }

            foreach (Enemy enemy in _miniBossList)
            {
                if (enemy)
                {
                    enemy.Enable();
                }
            }

            foreach (Enemy enemy in _bossList)
            {
                if (enemy)
                {
                    enemy.Enable();
                }
            }
        }

        public override void Disable()
        {
            base.Disable();

            if (_spawnEnemyCoroutine != null)
            {
                StopCoroutine(_spawnEnemyCoroutine);
            }

            foreach (Enemy enemy in _enemyListStage1)
            {
                enemy.Disable();
            }

            foreach (Enemy enemy in _enemyListStage2)
            {
                enemy.Disable();
            }

            foreach (Enemy enemy in _enemyListStage3)
            {
                enemy.Disable();
            }

            foreach (Enemy enemy in _enemyTutorialList)
            {
                enemy.Disable();
            }

            foreach (Enemy enemy in _enemyMinionsList)
            {
                enemy.Disable();
            }

            foreach (Enemy enemy in _miniBossList)
            {
                if (enemy)
                {
                    enemy.Disable();
                }
            }

            foreach (Enemy enemy in _bossList)
            {
                if (enemy)
                {
                    enemy.Disable();
                }
            }
        }

        private void Update()
        {
            if (!_isEnabled)
            {
                return;
            }

            _spawnTimer -= Time.deltaTime;
            
            if (_spawnTimer <= 0)
            {
                _spawnTimer += _spawnInterval;
                SpawnLogicSimple();
            }
        }

        private void SpawnLogicSimple()
        {
            int enemiesToSpawnStage1 = MatchManager.Instance.IsStage1Active() ? Math.Min(_stage1MaxSpawnsPerBatch, _stage1MaxEnemiesInPlanet - _enemyListStage1.Count) : 0;
            int enemiesToSpawnStage2 = MatchManager.Instance.IsStage2Active() ? Math.Min(_stage2MaxSpawnsPerBatch, _stage2MaxEnemiesInPlanet - _enemyListStage2.Count) : 0;
            int enemiesToSpawnStage3 = MatchManager.Instance.IsStage3Active() ? Math.Min(_stage3MaxSpawnsPerBatch, _stage3MaxEnemiesInPlanet - _enemyListStage3.Count) : 0;
            StartCoroutine(SpawnRound(enemiesToSpawnStage1, _stage1ConfigList, _enemyListStage1, Tier.Tier1));
            StartCoroutine(SpawnRound(enemiesToSpawnStage2, _stage2ConfigList, _enemyListStage2, Tier.Tier2));
            StartCoroutine(SpawnRound(enemiesToSpawnStage3, _stage3ConfigList, _enemyListStage3, Tier.Tier3));
        }

        private IEnumerator SpawnRound(int enemiesToSpawn, List<PooledEnemyConfig> stageConfig, List<Enemy> enemyList, Tier tier)
        {
            for (int i = 0; i < enemiesToSpawn; i++)
            {
                SpawnRandomEnemy(stageConfig, _enemySpawnMarkVfxPrefab, _enemySpawnRayVfxPrefab, tier, enemyList);
                yield return new WaitForEndOfFrame();
            }
        }

        public void SpawnMiniBoss()
        {
            SpawnRandomEnemy(_miniBossesConfigList, _enemyBossSpawnMarkVfxPrefab, _enemyBossSpawnRayVfxPrefab, Tier.None, creatorInstanceId: 0, isBoss: true);
        }

        public void SpawnBoss(bool isMatchTutorial = false)
        {
            SpawnRandomEnemy(_bossesConfigList, _enemyBossSpawnMarkVfxPrefab, _enemyBossSpawnRayVfxPrefab, Tier.None, creatorInstanceId: 0, isBoss: true, isMatchTutorial: isMatchTutorial);
        }

        public void SpawnEnemyInsideRadius(Enemy enemyPrefab, Tier enemyTier, int creatorInstanceId, Vector3 origin, float radius)
        {
            StartCoroutine(InstantSpawnEnemyChoreography(GeneratePointInSurfaceWithRadius(origin, radius, enemyPrefab.GetNavmeshAgentAreaMask()), enemyPrefab, enemyTier, creatorInstanceId: creatorInstanceId));
        }

        private IEnumerator InstantSpawnEnemyChoreography(Vector3 spawnPosition, Enemy enemyPrefab, Tier enemyTier, List<Enemy> enemyList = null, int creatorInstanceId = 0)
        {
            yield return new WaitForSeconds(_timeBeforeInstantSpawn);
            Vector3 upDirection = (spawnPosition - _sphereCenter).normalized;
            Quaternion rotation = Quaternion.LookRotation(Vector3.forward, upDirection);
            SpawnEnemy(enemyPrefab, spawnPosition, rotation, enemyTier, enemyList, creatorInstanceId, isMinion: true);
        }

        private void SpawnEnemy(Enemy enemyPrefab, Vector3 spawnPosition, Quaternion rotation, Tier tier, List<Enemy> enemyList = null, int creatorInstanceId = 0, bool isBoss = false, bool isMinion = false, bool doInitializeWithAiMovement = true)
        {
            Enemy enemyInstance = ObjectPool.Instance.GetPooledObject(enemyPrefab.gameObject, spawnPosition, rotation).GetComponent<Enemy>();
            enemyInstance.Initialize(_sphereCenter, _sphereRadius, enemyPrefab, creatorInstanceId, doInitializeWithAiMovement, isBoss, tier);
            EnemyInitialization(enemyInstance);
            enemyInstance.OnDead += Enemy_OnDead;

            if (isBoss)
            {
                enemyInstance.OnDead += Boss_OnDead;
                
                if (enemyInstance.GetComponent<Boss>().IsMiniBoss())
                {
                    _miniBossList.Add(enemyInstance);
                    _miniBossSpawnerList[^1] = null;
                }
                else
                {
                    _bossList.Add(enemyInstance);
                    _bossSpawnerList[^1] = null;
                }
                
                return;
            }

            if (isMinion)
            {
                _enemyMinionsList.Add(enemyInstance);
                OnMinionSpawned?.Invoke(enemyInstance);
                return;
            }

            enemyList?.Add(enemyInstance);
        }

        public Vector3 GeneratePointInSurfaceWithRadius(Vector3 origin, float radius, int areaMask)
        {
            for (int attempt = 0; attempt < MaxAttempts; attempt++)
            {
                Vector3 randomDirection = Random.onUnitSphere;
                Vector3 offset = randomDirection * Random.Range(NumberConstants.Half * radius, radius);
                Vector3 testPosition = origin + offset;
                Vector3 directionFromCenter = (testPosition - _sphereCenter).normalized;
                testPosition = _sphereCenter + directionFromCenter * _sphereRadius;

                if (PointInsideNavmesh(testPosition, areaMask))
                {
                    return testPosition;
                }
            }

            return GeneratePointInSurface(areaMask: areaMask);
        }

        private void SpawnRandomEnemy(List<PooledEnemyConfig> stageConfig, EnemyMarkSpawner spawnMarkVfxPrefab, EnemyRaySpawner spawnRayVfxPrefab, Tier tier, List<Enemy> enemyList = null, int creatorInstanceId = 0, bool isBoss = false, bool isMinion = false, bool isMatchTutorial = false)
        {
            int enemyIndex;

            if (isBoss && _numberOfMiniBosses < stageConfig.Count)
            {
                enemyIndex = _numberOfMiniBosses++;
            }
            else if (isBoss && _numberOfBosses < stageConfig.Count)
            {
                enemyIndex = _numberOfBosses++;
            }
            else
            {
                enemyIndex = Random.Range(0, stageConfig.Count);
            }

            EnemyRagdoll enemyRagdollPrefab = stageConfig[enemyIndex].EnemyRagdollPrefab;
            Enemy enemyPrefab = stageConfig[enemyIndex].EnemyPrefab;
            Vector3 pointInSurface;
            
            if (isMatchTutorial)
            {
                
                pointInSurface = MatchTutorialBossPosition;
            }
            else
            {
                pointInSurface = GeneratePointInSurface(areaMask: enemyPrefab.GetNavmeshAgentAreaMask());
            }

            _spawnEnemyCoroutine = StartCoroutine(SpawnEnemyChoreography(pointInSurface, enemyPrefab, enemyRagdollPrefab, spawnMarkVfxPrefab, spawnRayVfxPrefab, tier, enemyList, creatorInstanceId, isBoss, isMinion));
        }

        public virtual IEnumerator SpawnEnemyChoreography(Vector3 spawnPosition, Enemy enemyPrefab, EnemyRagdoll enemyRagdollPrefab, EnemyMarkSpawner spawnMarkVfxPrefab, EnemyRaySpawner spawnRayVfxPrefab, Tier tier, List<Enemy> enemyList = null, int creatorInstanceId = 0, bool isBoss = false, bool isMinion = false,
            bool useSpawnerVisuals = true, bool doInitializeWithAiMovement = true)
        {
            Vector3 upDirection = (spawnPosition - _sphereCenter).normalized;
            EnemyMarkSpawner spawnMarkVfxInstance = null;
            EnemyRaySpawner spawnRayVfxInstance = null;

            if (useSpawnerVisuals)
            {
                spawnMarkVfxInstance = ObjectPool.Instance.GetPooledObject(spawnMarkVfxPrefab.gameObject, Vector3.zero, Quaternion.identity).GetComponent<EnemyMarkSpawner>();
                spawnMarkVfxInstance.transform.position = spawnPosition;
                spawnMarkVfxInstance.transform.up = upDirection;
                spawnMarkVfxInstance.Initialize();

                if (isBoss)
                {
                    if (enemyPrefab.GetComponent<Boss>().IsMiniBoss())
                    {
                        _miniBossSpawnerList.Add(spawnMarkVfxInstance.transform);
                    }
                    else
                    {
                        _bossSpawnerList.Add(spawnMarkVfxInstance.transform);
                    }
                }
            }
            
            EnemyRagdoll enemyRagdollInstance = ObjectPool.Instance.GetPooledObject(enemyRagdollPrefab.gameObject, Vector3.zero, Quaternion.identity).GetComponent<EnemyRagdoll>();

            if (useSpawnerVisuals)
            {
                yield return new WaitForSeconds(spawnMarkVfxInstance.GetSpawnDuration() - enemyRagdollInstance.GetSpawnAnimationDuration());
            }

            InitializeRagdoll(enemyRagdollInstance, spawnPosition, upDirection);

            if (useSpawnerVisuals)
            {
                spawnRayVfxInstance = ObjectPool.Instance.GetPooledObject(spawnRayVfxPrefab.gameObject, Vector3.zero, Quaternion.identity).GetComponent<EnemyRaySpawner>();
                spawnRayVfxInstance.transform.position = spawnPosition;
                spawnRayVfxInstance.transform.up = upDirection;
                spawnRayVfxInstance.Initialize();
            }

            yield return new WaitForSeconds(enemyRagdollInstance.GetSpawnAnimationDuration());

            if (useSpawnerVisuals)
            {
                ObjectPool.Instance.Release(spawnMarkVfxPrefab.gameObject, spawnMarkVfxInstance.gameObject);
                ObjectPool.Instance.Release(spawnRayVfxPrefab.gameObject, spawnRayVfxInstance.gameObject);
            }

            ObjectPool.Instance.Release(enemyRagdollPrefab.gameObject, enemyRagdollInstance.gameObject);

            SpawnEnemy(enemyPrefab, spawnPosition, enemyRagdollInstance.transform.rotation, tier, enemyList, creatorInstanceId, isBoss, isMinion, doInitializeWithAiMovement);
        }
        
        protected virtual void InitializeRagdoll(EnemyRagdoll enemyRagdollInstance, Vector3 spawnPosition, Vector3 upDirection)
        {
            enemyRagdollInstance.Initialize();
            enemyRagdollInstance.transform.position = spawnPosition;
            enemyRagdollInstance.transform.up = upDirection;
            enemyRagdollInstance.PlaySpawnAnimation();
        }

        public Vector3 GeneratePointInSurface(int attempts = 0, int areaMask = NavMesh.AllAreas)
        {
            Vector3 testPosition = _sphereCenter + Random.insideUnitSphere.normalized * _sphereRadius;

            if (attempts < MaxAttempts && !PointInsideNavmesh(testPosition, areaMask))
            {
                return GeneratePointInSurface(attempts + 1, areaMask);
            }

            return testPosition;
        }

        private bool PointInsideNavmesh(Vector3 testPosition, int areaMask)
        {
            if (!NavMesh.SamplePosition(testPosition, out NavMeshHit hit, MaxDistanceFromTestPoint, areaMask))
            {
                return false;
            }

            int hitCount = Physics.OverlapSphereNonAlloc(testPosition, 1.0f, _colliderHits, _obstacleMask, QueryTriggerInteraction.Collide);
            return hitCount == 0;
        }


        private void Enemy_OnDead(Enemy enemy, int heroIndex)
        {
            enemy.OnDead -= Enemy_OnDead;
            ItemDrops(enemy);
            
            if (_enemyListStage1.Contains(enemy))
            {
                _enemyListStage1.Remove(enemy);
            }

            if (_enemyListStage2.Contains(enemy))
            {
                _enemyListStage2.Remove(enemy);
            }

            if (_enemyListStage3.Contains(enemy))
            {
                _enemyListStage3.Remove(enemy);
            }

            if (_enemyTutorialList.Contains(enemy))
            {
                _enemyTutorialList.Remove(enemy);
            }

            if (_enemyMinionsList.Contains(enemy))
            {
                _enemyMinionsList.Remove(enemy);
            }
            
            ObjectPool.Instance.Release(enemy.GetEnemyPrefab().gameObject, enemy.gameObject);
            OnEnemyDead?.Invoke(enemy);
        }

        protected virtual void ItemDrops(Enemy enemy)
        {
            SpawnHpDrop(enemy);

            StartCoroutine(DropManager.Instance.GiveXpDropPrefabs(enemy.TagXp(), enemy.transform.position, enemy.transform.rotation, enemy.transform.up, !IsBoss(enemy)));
        }

        private void SpawnHpDrop(Enemy enemy)
        {
            if (Random.Range(0.0f, 1.0f) > DropManager.Instance.GetHpDropSpawnChance())
            {
                return;
            }
            
            DropManager.Instance.SpawnHpDrop(enemy.transform.position, enemy.transform.rotation, enemy.transform.up);
        }

        private void Boss_OnDead(Enemy enemy, int heroIndex)
        {
            enemy.OnDead -= Boss_OnDead;
            int index;
            
            if (enemy.GetComponent<Boss>().IsMiniBoss())
            {
                index = _miniBossList.IndexOf(enemy);
                _miniBossList[index] = null;
            }
            else
            {
                index = _bossList.IndexOf(enemy);
                _bossList[index] = null;
            }
            
            if (index == 0)
            {
                UIManager.Instance.DisableMiniBossIndicator();
            }
            else
            {
                UIManager.Instance.DisableBossIndicator();
            }
            
            if (heroIndex != Player.GetHumanPlayerIndex())
            {
                UIManager.Instance.ShowPlayerKilledBossStatusPanel(heroIndex, enemy.GetComponent<Boss>().GetName());
            }

            Player player = PlayerManager.Instance.GetPlayer(heroIndex);
            player.ActivateBossPower();
            player.IncreaseBossKillCount();

            OnBossDead?.Invoke();
        }

        public List<Enemy> GetEnemies()
        {
            List<Enemy> enemies = new List<Enemy>();
            enemies.AddRange(_enemyListStage1);
            enemies.AddRange(_enemyListStage2);
            enemies.AddRange(_enemyListStage3);
            enemies.AddRange(_enemyTutorialList);
            enemies.AddRange(_enemyMinionsList);
            enemies.AddRange(_miniBossList);
            enemies.AddRange(_bossList);

            if (ControlsTutorialEnemySpawner.Instance)
            {
                enemies.AddRange(ControlsTutorialEnemySpawner.Instance.GetEnemiesWave1());
                enemies.AddRange(ControlsTutorialEnemySpawner.Instance.GetEnemiesWave2());
            }
            
            return enemies;
        }

        public Transform GetMiniBossTransform(int miniBossIndex)
        {
            if (_miniBossList.Count > miniBossIndex)
            {
                if (_miniBossList[miniBossIndex])
                {
                    return _miniBossList[miniBossIndex].transform;
                }
            }

            if (_miniBossSpawnerList.Count > miniBossIndex)
            {
                return _miniBossSpawnerList[miniBossIndex];
            }

            return null;
        }
        
        public Transform GetBossTransform(int bossIndex)
        {
            if (_bossList.Count > bossIndex)
            {
                if (_bossList[bossIndex])
                {
                    return _bossList[bossIndex].transform;
                }
            }

            if (_bossSpawnerList.Count > bossIndex)
            {
                return _bossSpawnerList[bossIndex];
            }

            return null;
        }

        public List<Enemy> GetSpawnedEnemies(int creatorInstanceId)
        {
            List<Enemy> spawnedEnemies = new List<Enemy>();

            foreach (Enemy enemy in _enemyListStage1)
            {
                if (enemy.GetCreatorInstanceId() == creatorInstanceId)
                {
                    spawnedEnemies.Add(enemy);
                }
            }

            foreach (Enemy enemy in _enemyListStage2)
            {
                if (enemy.GetCreatorInstanceId() == creatorInstanceId)
                {
                    spawnedEnemies.Add(enemy);
                }
            }

            foreach (Enemy enemy in _enemyListStage3)
            {
                if (enemy.GetCreatorInstanceId() == creatorInstanceId)
                {
                    spawnedEnemies.Add(enemy);
                }
            }

            foreach (Enemy enemy in _enemyTutorialList)
            {
                if (enemy.GetCreatorInstanceId() == creatorInstanceId)
                {
                    spawnedEnemies.Add(enemy);
                }
            }

            foreach (Enemy enemy in _enemyMinionsList)
            {
                if (enemy.GetCreatorInstanceId() == creatorInstanceId)
                {
                    spawnedEnemies.Add(enemy);
                }
            }

            foreach (Enemy enemy in _miniBossList)
            {
                if (enemy && enemy.GetCreatorInstanceId() == creatorInstanceId)
                {
                    spawnedEnemies.Add(enemy);
                }
            }

            foreach (Enemy enemy in _bossList)
            {
                if (enemy && enemy.GetCreatorInstanceId() == creatorInstanceId)
                {
                    spawnedEnemies.Add(enemy);
                }
            }

            return spawnedEnemies;
        }

        public bool IsBoss(Enemy enemy)
        {
            return _bossList.Contains(enemy) || _miniBossList.Contains(enemy);
        }

        public EnemyMarkSpawner GetEnemyMarkSpawner()
        {
            return _enemySpawnMarkVfxPrefab;
        }
        
        public EnemyRaySpawner GetEnemyRaySpawner()
        {
            return _enemySpawnRayVfxPrefab;
        }

        public void AddEnemiesFromTutorial(List<Enemy> enemiesFromTutorial)
        {
            _enemyTutorialList.AddRange(enemiesFromTutorial);
        }

        public void SpawnInitialBatch()
        {
            if (_hasSpawnedInitialBatch)
            {
                return;
            }

            _hasSpawnedInitialBatch = true;
            StartCoroutine(SpawnRound(_initialBatchSpawns, _stage1ConfigList, _enemyListStage1, Tier.Tier1));
        }

        public void StopSpawning()
        {
            base.Disable();
        }

        public void ForceSpawn()
        {
            SpawnLogicSimple();
        }

        protected virtual void EnemyInitialization(Enemy enemyInstance)
        {
            enemyInstance.GetUnit().ShowIceHazardHideableObjects();
        }
    }
}