using System;
using System.Collections;
using System.Collections.Generic;
using Application.Data;
using Application.Sound;
using CandyCoded.HapticFeedback;
using Cinemachine;
using DamageNumbersPro;
using Game.BaseUnit;
using Game.Camera;
using Game.Damage;
using Game.Detectors;
using Game.Enabler;
using Game.Match;
using Game.Skills;
using Game.Talent;
using Game.UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.PlayerUnit
{
    [RequireComponent(typeof(Unit))]
    [RequireComponent(typeof(PlayerVisuals))]
    [RequireComponent(typeof(EnemyDetectorForPlayers))]
    [RequireComponent(typeof(PlayerLevel))]
    [RequireComponent(typeof(PlayerStats))]
    public abstract class Player : EnablerMonoBehaviour
    {
        [SerializeField] private PlayerLevelData _playerLevelData;
        [SerializeField] private PlayerMagnet _playerMagnet;
        [SerializeField] private PlayerIceHazardFogFieldOfView _playerIceHazardFogFieldOfView;
        [SerializeField] private DamageNumber _xpParticlePrefab;
        [SerializeField] private DamageNumber _goldParticlePrefab;
        [SerializeField] private Transform _xpParticleSpawnPoint;
        [SerializeField] private PlayerBehavior _playerBehavior;
        [SerializeField] private bool _detectionCircleUpdateEveryFrame;
        [SerializeField] private Sprite _profileSprite;
        [SerializeField] private string _profileName;
        [SerializeField] private PlayerLevelIndicator _playerLevelIndicator;
        [SerializeField] private CinemachineImpulseSource _cinemachineImpulseSource;
        [SerializeField] private WeaponConfigData _weaponConfigData;

        private const int HumanPlayerIndex = 0;

        private int _playerIndex;
        private Unit _unit;
        private PlayerVisuals _playerVisuals;
        private PlayerLevel _playerLevel;
        private PlayerStats _playerStats;
        private EnemyDetectorForPlayers _enemyDetectorForPlayers;
        private Coroutine _activateBossPowerCoroutine;
        private Coroutine _deactivateBossPowerCoroutine;
        private int _kills;
        private int _bossKills;
        private bool _isBossPowerActive;
        private bool _isBossPowerInfinite;
        private float _factorOfPlayerRadiusToSee;
        private int _collectedGold;
        private bool _isSkillSelectionAllowed;
        private bool _isXpEffectsAllowed;

        public Action<Player> OnDead;
        public Action<PlayerLevel.OnLevelUpEventArgs> OnLevelUp { get; internal set; }
        public Action<PlayerLevel.OnXpGainedEventArgs, int> OnXpGained { get; internal set; }
        public Action<int> OnGoldGained { get; internal set; }

        protected virtual void Awake()
        {
            _unit = GetComponent<Unit>();
            _playerVisuals = GetComponent<PlayerVisuals>();
            _playerLevel = GetComponent<PlayerLevel>();
            _playerStats = GetComponent<PlayerStats>();
            _enemyDetectorForPlayers = GetComponent<EnemyDetectorForPlayers>();
        }

        public override void Enable()
        {
            base.Enable();
            _unit.Enable();
            _playerBehavior.Enable();
        }

        public override void Disable()
        {
            base.Disable();
            _unit.Disable();
            _playerBehavior.Disable();
            _playerVisuals.SetSpeed(0);
        }

        private void Update()
        {
            if (!_isEnabled)
            {
                return;
            }

            _playerVisuals.SetSpeed(_playerBehavior.GetCurrentSpeed());

            if (_detectionCircleUpdateEveryFrame)
            {
                _playerVisuals.IncrementAttackRange(_playerStats.GetAttackRange());
            }
        }

        private void Unit_OnDead(Unit unit, int heroIndex)
        {
            SoundManager.Instance.PlayPlayerDeathSound();
            gameObject.SetActive(false);
            Disable();
            _playerBehavior.Disable();
            OnDead?.Invoke(this);
        }

        public virtual void Initialize(int playerIndex, Vector3 sphereCenter, float sphereRadius, string playerName)
        {
            if (playerName.Length > 0)
            {
                _profileName = playerName;
            }
            
            _playerIndex = playerIndex;
            _factorOfPlayerRadiusToSee = _weaponConfigData.FactorOfPlayerRadiusToSee;
            _playerStats.Initialize(_weaponConfigData);
            DeactivateBossPower();
            _unit.Initialize(_playerStats.GetMaxHp());
            _unit.OnDead += Unit_OnDead;
            _unit.OnDamaged += Unit_OnDamaged;
            _playerBehavior.Initialize(sphereCenter, sphereRadius);
            _playerBehavior.OnMoved += PlayerBehavior_OnMoved;
            _playerVisuals.Initialize(sphereRadius, _enemyDetectorForPlayers.GetDetectionDistance(), _playerIndex, _profileName);
            _playerMagnet.Initialize(_playerIndex, _playerStats.GetCollectRadiusModifier());
            _playerLevel.Initialize(_playerLevelData);
            _playerLevel.OnXpGained += PlayerLevel_OnXpGained;
            _playerLevel.OnLevelUp += PlayerLevel_OnLevelUp;
            _playerLevelIndicator.UpdateLevel(0);
            _playerIceHazardFogFieldOfView.Disable();
            StartCoroutine(InitializeXpParticlesPool());

            if (TalentManager.Instance.GetStartingSkillsTalent())
            {
                _playerBehavior.ManageLevelUp();
            }

            _kills = 0;
            _bossKills = 0;
            _collectedGold = 0;
            _isSkillSelectionAllowed = true;
            _isXpEffectsAllowed = true;
        }

        public void GiveInitialSkills(int freeSkillsAmount)
        {
            for (int i = 0; i < freeSkillsAmount; i++)
            {
                _playerBehavior.ManageLevelUp();
            }
        }
        
        public void DisableSkillSelection()
        {
            _isSkillSelectionAllowed = false;
        }
        
        public void DisableXpEffects()
        {
            _isXpEffectsAllowed = false;
        }
        
        private void Unit_OnDamaged(int amount, int currentHealth, int maxHealth, DamageType damageType, int damagerIndex)
        {
            if (_playerIndex != HumanPlayerIndex)
            {
                return;
            }

            if (amount == 0)
            {
                return;
            }

            _cinemachineImpulseSource.GenerateImpulse();
            HapticFeedback.LightFeedback();
        }

        public float GetEnemyDetectionDistance()
        {
            return _enemyDetectorForPlayers.GetDetectionDistance();
        }

        private IEnumerator InitializeXpParticlesPool()
        {
            yield return new WaitForEndOfFrame();
            _xpParticlePrefab.PrewarmPool();
            _goldParticlePrefab.PrewarmPool();
        }

        private void PlayerLevel_OnLevelUp(PlayerLevel.OnLevelUpEventArgs args)
        {
            SoundManager.Instance.PlayPlayerLevelUpSound();
            _playerVisuals.LevelUp();
            _unit.HealPercentage(_playerStats.GetLevelUpHealingModifier());
            _playerLevelIndicator.UpdateLevel(args.CurrentLevel);
            
            if (_isSkillSelectionAllowed)
            {
                _playerBehavior.ManageLevelUp();
            }

            if (_isXpEffectsAllowed)
            {
                OnLevelUp?.Invoke(args);
            }
        }

        private void PlayerLevel_OnXpGained(PlayerLevel.OnXpGainedEventArgs args)
        {
            if (!_isXpEffectsAllowed)
            {
                return;
            }

            SoundManager.Instance.PlayPlayerPickXpSound();
            OnXpGained?.Invoke(args, _playerIndex);
        }

        protected void FaceToEnemyDirection(Vector3 directionToEnemy)
        {
            _playerBehavior.CalculateForwardAndUpDirections(out Vector3 forwardDirection, out Vector3 upDirection);
            _playerVisuals.PlayAttackAnimation(forwardDirection, directionToEnemy, upDirection);
        }

        public void CalculateForwardAndUpDirections(out Vector3 forwardDirection, out Vector3 upDirection)
        {
            _playerBehavior.CalculateForwardAndUpDirections(out forwardDirection, out upDirection);
        }

        private void PlayerBehavior_OnMoved(Vector3 forwardDirection, Vector3 movementDirection, Vector3 upDirection)
        {
            _playerVisuals.UpdateMoveDirection(forwardDirection, movementDirection, upDirection);
        }

        public void Damage(int damage, DamageType damageType, DamageEffect damageEffect, int amountOfParticles = 1)
        {
            if (!_isEnabled)
            {
                return;
            }
            
            int dodge = Random.Range(0, 100);
            
            if (dodge < GetDodgeChance())
            {
                _unit.SpawnDodgeTextParticle();
                return;
            }

            SoundManager.Instance.PlayPlayerHitSound();
            _unit.Damage(GetReducedDamage(damage), damageType, damageEffect, amountOfParticles: amountOfParticles);
        }

        public void UnavoidableDamage(int damage, DamageType damageType, DamageEffect damageEffect, int amountOfParticles = 1)
        {
            if (!_isEnabled)
            {
                return;
            }

            SoundManager.Instance.PlayPlayerHitSound();
            _unit.Damage(damage, damageType, damageEffect, amountOfParticles: amountOfParticles);
        }

        public void HealPercentage(float healAmountPercentage)
        {
            _unit.HealPercentage(healAmountPercentage);
        }

        private int GetReducedDamage(int damage)
        {
            return Math.Max(1, Mathf.FloorToInt(damage * _playerStats.GetDamageReductionModifier() - TalentManager.Instance.GetDamageReceivedTalentReduction()));
        }

        private int GetDodgeChance()
        {
            return _playerStats.GetDodgeChance();
        }

        public void GainXp(int xpAmount)
        {
            float gainedXp = GetXpIncrease(xpAmount);
            _playerLevel.GainXp(gainedXp);
            DamageNumber damageNumber = _xpParticlePrefab.Spawn(_xpParticleSpawnPoint.position, Mathf.FloorToInt(gainedXp));
            damageNumber.cameraOverride = CameraManager.Instance.Camera().transform;
        }

        private float GetXpIncrease(int xpAmount)
        {
            return xpAmount * _playerStats.GetXpGainedModifier();
        }

        public void GainGold(int goldAmount)
        {
            _collectedGold += goldAmount;
            OnGoldGained?.Invoke(_collectedGold);
            DamageNumber damageNumber = _goldParticlePrefab.Spawn(_xpParticleSpawnPoint.position, Mathf.FloorToInt(goldAmount));
            damageNumber.cameraOverride = CameraManager.Instance.Camera().transform;
        }

        public int GetCollectedGoldAmount()
        {
            return _collectedGold;
        }
        
        public int GetPlayerIndex()
        {
            return _playerIndex;
        }

        public Transform GetCenterAnchor()
        {
            return _unit.GetCenterAnchor();
        }

        public Sprite GetSprite()
        {
            return _profileSprite;
        }

        public string GetName()
        {
            return _profileName;
        }

        public static int GetHumanPlayerIndex()
        {
            return HumanPlayerIndex;
        }

        public bool IsAlive()
        {
            return _unit.IsAlive();
        }

        public Sprite GetIcon()
        {
            return _profileSprite;
        }

        public float GetFactorOfPlayerRadiusToSee()
        {
            return _factorOfPlayerRadiusToSee;
        }

        public void PushBack(float pushBackForce, Vector3 pushBackDirection)
        {
            _playerBehavior.PushBack(pushBackForce, pushBackDirection);
        }

        public void ActivateBossPower(bool isBossPowerInfinite = false)
        {
            if (_isBossPowerActive)
            {
                return;
            }

            _isBossPowerActive = true;
            _isBossPowerInfinite = isBossPowerInfinite;
            
            if (_activateBossPowerCoroutine != null)
            {
                StopCoroutine(_activateBossPowerCoroutine);
            }

            if (_deactivateBossPowerCoroutine != null)
            {
                StopCoroutine(_deactivateBossPowerCoroutine);
            }

            _activateBossPowerCoroutine = StartCoroutine(ActivateBossPowerChoreography());
        }

        private IEnumerator ActivateBossPowerChoreography()
        {
            _unit.MakeInvincible();
            Disable();
            _playerVisuals.ActivateBossPower();

            yield return new WaitForSeconds(MatchManager.Instance.GetBossPowerInvulnerabilityDuration());

            _unit.RemoveInvincible();
            Enable();
            _playerStats.ActivateBossPower();

            if (_isBossPowerInfinite)
            {
                yield break;
            }
            
            _deactivateBossPowerCoroutine = StartCoroutine(WaitAndDeactivateBossPower());
        }

        private IEnumerator WaitAndDeactivateBossPower()
        {
            yield return new WaitForSeconds(MatchManager.Instance.GetBossPowerDuration());
            DeactivateBossPower();
        }

        private void DeactivateBossPower()
        {
            _playerStats.DeactivateBossPower();
            _playerVisuals.DeactivateBossPower();
            _isBossPowerActive = false;
        }

        public float GetTotalXp()
        {
            return _playerLevel.GetTotalXp();
        }

        public Unit GetUnit()
        {
            return _unit;
        }

        public int CurrentHealth()
        {
            return _unit.CurrentHealth();
        }

        public void IncreaseKillCount()
        {
            _kills++;
        }

        public void IncreaseBossKillCount()
        {
            _bossKills++;
        }

        public int Kills()
        {
            return _kills;
        }

        public int BossKills()
        {
            return _bossKills;
        }

        public virtual void AttackOnlyEnable()
        {
        }

        public virtual void AttackOnlyDisable()
        {
        }

        public void MakePlayerNonKillable()
        {
            _unit.MakePlayerNonKillable();
        }
        
        public void EnableDirectionalArrow(Vector3 targetPosition)
        {
            _playerVisuals.EnableDirectionalArrow(targetPosition);
        }

        public void DisableDirectionalArrow()
        {
            _playerVisuals.DisableDirectionalArrow();
        }

        public void SetNextSkillsToChoose(List<PlayerSkillData> preselectedSkills)
        {
            _playerBehavior.SetPreselectedSkills(preselectedSkills);
        }

        public void MagnetColliderDisable()
        {
            _playerMagnet.DisableCollider();

            foreach (CapsuleCollider capsuleCollider in GetComponents<CapsuleCollider>())
            {
                capsuleCollider.enabled = false;
            }
        }
        
        public void MagnetColliderEnable()
        {
            _playerMagnet.EnableCollider();
            
            foreach (CapsuleCollider capsuleCollider in GetComponents<CapsuleCollider>())
            {
                capsuleCollider.enabled = true;
            }
        }

        public float GetAttackRange()
        {
            return _playerStats.GetAttackRange();
        }
        
        public void SetAttackRange(float newRange)
        {
            _playerStats.ForceAttackRange(newRange);
        }
        
        public void SetFactorOfPlayerRadiusToSee(float newFactorOfPlayerRadiusToSee)
        {
            _factorOfPlayerRadiusToSee = newFactorOfPlayerRadiusToSee;
        }

        public void InitializeIceHazardFogFieldOfView(float fogFieldOfViewRadius)
        {
            _playerIceHazardFogFieldOfView.Initialize(fogFieldOfViewRadius);
        }
    }
}