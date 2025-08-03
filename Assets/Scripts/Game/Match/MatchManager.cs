using System;
using System.Collections.Generic;
using System.Linq;
using Application.Utils;
using Game.EnemyUnit;
using Game.UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Match
{
    public class MatchManager : MonoBehaviour
    {
        [Header("Match Game Config")] 
        [SerializeField] private int _totalGameTimeStamp;
        [SerializeField] private int _miniBossArrivalTime;
        [SerializeField] private int _bossArrivalTime;
        [SerializeField] private int _stage1DifferentEnemiesSelection;
        [SerializeField] private int _stage2DifferentEnemiesSelection;
        [SerializeField] private int _stage3DifferentEnemiesSelection;
        [Range(0,1)]
        [SerializeField] private int _miniBossDifferentEnemiesSelection;
        [Range(0,1)]
        [SerializeField] private int _bossDifferentEnemiesSelection;

        [Header("Match Enemies Config")]
        [SerializeField] private int _initialBatchSpawns;
        [SerializeField] private float _spawnInterval;
        [SerializeField] private List<PooledEnemyConfig> _stage1ConfigList;
        [SerializeField] private int _stage1StartTime;
        [SerializeField] private int _stage1EndTime;
        [SerializeField] private int _stage1MaxEnemiesInPlanet;
        [SerializeField] private int _stage1MaxSpawnsPerBatch;
        [SerializeField] private List<PooledEnemyConfig> _stage2ConfigList;
        [SerializeField] private int _stage2StartTime;
        [SerializeField] private int _stage2EndTime;
        [SerializeField] private int _stage2MaxEnemiesInPlanet;
        [SerializeField] private int _stage2MaxSpawnsPerBatch;
        [SerializeField] private List<PooledEnemyConfig> _stage3ConfigList;
        [SerializeField] private int _stage3StartTime;
        [SerializeField] private int _stage3EndTime;
        [SerializeField] private int _stage3MaxEnemiesInPlanet;
        [SerializeField] private int _stage3MaxSpawnsPerBatch;
        [SerializeField] private List<PooledEnemyConfig> _miniBossesConfigList;
        [SerializeField] private List<PooledEnemyConfig> _bossesConfigList;

        [Header("Boss Power Config")]
        [SerializeField] private float _bossPowerInvulnerabilityDuration;
        [SerializeField] private float _bossPowerDuration;
        [Range(1.0f, 2.0f)][SerializeField] private float _bossPowerXpGainMultiplier;
        [Range(1.0f, 2.0f)][SerializeField] private float _bossPowerAttackMultiplier;
        [Range(1.0f, 2.0f)][SerializeField] private float _bossPowerMaxHpMultiplier;
        [Range(1.0f, 2.0f)][SerializeField] private float _bossPowerAttackRangeMultiplier;
        [Range(1.0f, 2.0f)][SerializeField] private float _bossPowerMovementSpeedMultiplier;
        
        public static MatchManager Instance;

        private CountDown _matchCountDown;
        private int _secondsPassed;
        private bool _isStage1Active;
        private bool _isStage2Active;
        private bool _isStage3Active;
        private bool _isStage2TransitionDone;
        private bool _isStage3TransitionDone;

        public Action OnCountDownEnded;
        
        private void Awake()
        {
            Instance = this;
        }

        public void Initialize()
        {
            _secondsPassed = 0;
            _matchCountDown = gameObject.AddComponent<CountDown>();
            _matchCountDown.SetTotalTime(_totalGameTimeStamp);
            _matchCountDown.OnSecondPassed += MatchCountDown_OnSecondPassed;
            _matchCountDown.OnCountDownEnded += MatchCountDown_OnCountDownEnded;
            UIManager.Instance.UpdateCountDownPanel(_matchCountDown.GetRemainingTime());
            _isStage2TransitionDone = false;
            _isStage3TransitionDone = false;
        }

        private void MatchCountDown_OnCountDownEnded()
        {
            OnCountDownEnded?.Invoke();
        }
        
        private void MatchCountDown_OnSecondPassed()
        {
            UIManager.Instance.UpdateCountDownPanel(_matchCountDown.GetRemainingTime());
            _secondsPassed++;

            if (_secondsPassed == _miniBossArrivalTime - Mathf.FloorToInt(UIManager.Instance.BossWarningOverlayDuration()))
            {
                UIManager.Instance.ShowBossWarningOverlay();
            }

            if (_secondsPassed == _bossArrivalTime - Mathf.FloorToInt(UIManager.Instance.BossWarningOverlayDuration()))
            {
                UIManager.Instance.ShowBossWarningOverlay();
            }

            if (_secondsPassed == _miniBossArrivalTime)
            {
                EnemyManager.Instance.SpawnMiniBoss();
                UIManager.Instance.EnableMiniBossIndicator();
            }

            if (_secondsPassed == _bossArrivalTime)
            {
                EnemyManager.Instance.SpawnBoss();
                UIManager.Instance.EnableBossIndicator();
            }
            
            _isStage1Active = _secondsPassed >= _stage1StartTime && _secondsPassed <= _stage1EndTime;
            _isStage2Active = _secondsPassed >= _stage2StartTime && _secondsPassed <= _stage2EndTime;
            _isStage3Active = _secondsPassed >= _stage3StartTime && _secondsPassed <= _stage3EndTime;

            if (_isStage2Active && !_isStage2TransitionDone)
            {
                _isStage2TransitionDone = true;
                EnvironmentManager.Instance.TransitionToStage2();
            }
            
            if (_isStage3Active && !_isStage3TransitionDone)
            {
                _isStage3TransitionDone = true;
                EnvironmentManager.Instance.TransitionToStage3();
            }
        }


        public void EndGame()
        {
            _matchCountDown.StopCounting();
        }

        public void StartGame()
        {
            _matchCountDown.StartCounting();
        }
        
        public void ForceEndGameByTimer()
        {
            _matchCountDown.SetTotalTime(1);
            MatchCountDown_OnSecondPassed();
        }

        public int GetSecondsPassed()
        {
            return _secondsPassed;
        }

        public List<PooledEnemyConfig> SelectStage1List()
        {
            return SelectRandomElementsFromEnemiesConfig(_stage1ConfigList, _stage1DifferentEnemiesSelection);
        }
        
        public List<PooledEnemyConfig> SelectStage2List()
        {
            return SelectRandomElementsFromEnemiesConfig(_stage2ConfigList, _stage2DifferentEnemiesSelection);
        }
        
        public List<PooledEnemyConfig> SelectStage3List()
        {
            return SelectRandomElementsFromEnemiesConfig(_stage3ConfigList, _stage3DifferentEnemiesSelection);
        }

        public List<PooledEnemyConfig> SelectBossesList()
        {
            return SelectRandomElementsFromEnemiesConfig(_bossesConfigList, _miniBossDifferentEnemiesSelection);
        }

        public List<PooledEnemyConfig> SelectMiniBossesList()
        {
            return SelectRandomElementsFromEnemiesConfig(_miniBossesConfigList, _bossDifferentEnemiesSelection);
        }

        private List<PooledEnemyConfig> SelectRandomElementsFromEnemiesConfig(List<PooledEnemyConfig> stageConfigList, int stageDifferentEnemiesSelection)
        {
            List<int> indexes = new List<int>();

            for (int i = 0; i < stageConfigList.Count; i++)
            {
                indexes.Add(i);
            }

            List<PooledEnemyConfig> result = new List<PooledEnemyConfig>();

            while (result.Count < stageDifferentEnemiesSelection)
            {
                int index = Random.Range(0, indexes.Count);
                result.Add(stageConfigList[indexes.ElementAt(index)]);

                if (indexes.Count > 1)
                {
                    indexes.RemoveAt(index);
                }
            }

            return result;
        }
        
        public int InitialBatchSpawns()
        {
            return _initialBatchSpawns;
        }

        public float SpawnInterval()
        {
            return _spawnInterval;
        }

        public bool IsStage1Active()
        {
            return _isStage1Active;
        }
        
        public bool IsStage2Active()
        {
            return _isStage2Active;
        }
        
        public bool IsStage3Active()
        {
            return _isStage3Active;
        }

        public int Stage1MaxSpawnsPerBatch()
        {
            return _stage1MaxSpawnsPerBatch;
        }
        
        public int Stage2MaxSpawnsPerBatch()
        {
            return _stage2MaxSpawnsPerBatch;
        }
        
        public int Stage3MaxSpawnsPerBatch()
        {
            return _stage3MaxSpawnsPerBatch;
        }

        public int Stage1MaxEnemiesInPlanet()
        {
            return _stage1MaxEnemiesInPlanet;
        }
        
        public int Stage2MaxEnemiesInPlanet()
        {
            return _stage2MaxEnemiesInPlanet;
        }
        
        public int Stage3MaxEnemiesInPlanet()
        {
            return _stage3MaxEnemiesInPlanet;
        }

        public float GetBossPowerInvulnerabilityDuration()
        {
            return _bossPowerInvulnerabilityDuration;
        }

        public float GetBossPowerDuration()
        {
            return _bossPowerDuration;
        }

        public float GetBossPowerXpGainMultiplier()
        {
            return _bossPowerXpGainMultiplier;
        }

        public float GetBossPowerAttackMultiplier()
        {
            return _bossPowerAttackMultiplier;
        }

        public float GetBossPowerMaxHpMultiplier()
        {
            return _bossPowerMaxHpMultiplier;
        }

        public float GetBossPowerAttackRangeMultiplier()
        {
            return _bossPowerAttackRangeMultiplier;
        }

        public float GetBossPowerMovementSpeedMultiplier()
        {
            return _bossPowerMovementSpeedMultiplier;
        }

        public void ForceStage2()
        {
            _secondsPassed = _stage2StartTime;
        }
    }
}