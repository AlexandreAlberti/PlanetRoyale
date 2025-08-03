using System;
using System.Linq;
using Application.Utils;
using Game.BaseUnit;
using Game.Match;
using Game.Skills;
using UnityEngine;

namespace Game.PlayerUnit
{
    [RequireComponent(typeof(Unit))]
    [RequireComponent(typeof(PlayerLevel))]
    [RequireComponent(typeof(PlayerBaseStats))]
    [RequireComponent(typeof(PlayerSkillTracker))]
    [RequireComponent(typeof(UnitStatusEffectSlowdown))]
    public class PlayerStats : MonoBehaviour
    {
        [SerializeField] private PlayerBehavior _playerBehavior;
        [SerializeField] private float _baseMovementSpeed;
        [SerializeField] private float _baseAttackRange;
        [SerializeField][Range(0.0f, 1.0f)] private float _levelUpMaxHpIncreasePercentage;
        [SerializeField][Range(0.0f, 1.0f)] private float _levelUpMovementSpeedIncreasePercentage;

        private const float BossPowerDisabledValue = 1.0f;
        
        private Unit _unit;
        private PlayerLevel _playerLevel;
        private PlayerBaseStats _playerBaseStats;
        private PlayerSkillTracker _playerSkillTracker;
        private UnitStatusEffectSlowdown _unitStatusEffectSlowdown;
        [SerializeField] private float _currentAttack;
        [SerializeField] private float _currentMaxHp;
        [SerializeField] private float _currentMovementSpeed;
        [SerializeField] private float _currentAttackRange;
        [SerializeField] private float _bossPowerXpGainedMultiplier;
        [SerializeField] private float _bossPowerAttackMultiplier;
        [SerializeField] private float _bossPowerMaxHpMultiplier;
        [SerializeField] private float _bossPowerAttackRangeMultiplier;
        [SerializeField] private float _bossPowerMovementSpeedMultiplier;
        private float _levelUpAttackRangeIncreasePercentage;
        private float _levelUpAttackIncreasePercentage;
        
        public Action OnAttackRangeUpdated;

        private void Awake()
        {
            _unit = GetComponent<Unit>();
            _playerLevel = GetComponent<PlayerLevel>();
            _playerBaseStats = GetComponent<PlayerBaseStats>();
            _playerSkillTracker = GetComponent<PlayerSkillTracker>();
            _unitStatusEffectSlowdown = GetComponent<UnitStatusEffectSlowdown>();
            DeactivateBossPower();
        }

        public void Initialize(WeaponConfigData weaponConfigData)
        {
            _currentAttack = _playerBaseStats.GetBaseAttack() * weaponConfigData.WeaponDamageMultiplier;
            _currentMaxHp = _playerBaseStats.GetBaseMaxHp();
            _currentAttackRange = _playerBaseStats.GetBaseAttackRange();
            _currentMovementSpeed = _playerBaseStats.GetBaseMovementSpeed();
            _levelUpAttackRangeIncreasePercentage = weaponConfigData.LevelUpAttackRangeIncreasePercentage;
            _levelUpAttackIncreasePercentage = weaponConfigData.LevelUpAttackDamageIncreasePercentage;
            _playerLevel.OnLevelUp += PlayerLevel_OnLevelUp;
            _playerSkillTracker.OnSkillGained += PlayerSkillTracker_OnSkillGained;
        }

        private void PlayerLevel_OnLevelUp(PlayerLevel.OnLevelUpEventArgs args)
        {
            _currentAttack += _currentAttack * _levelUpAttackIncreasePercentage;
            _currentMaxHp += _currentMaxHp * _levelUpMaxHpIncreasePercentage;
            _currentMovementSpeed += _currentMovementSpeed * _levelUpMovementSpeedIncreasePercentage;
            _currentAttackRange += _currentAttackRange * _levelUpAttackRangeIncreasePercentage;
            _unit.IncreaseMaxHp(GetMaxHp());
            _playerBehavior.UpdateMaxDetectionDistance(_currentAttackRange);
            OnAttackRangeUpdated?.Invoke();
        }

        private void PlayerSkillTracker_OnSkillGained(PlayerSkillData playerSkillData)
        {
            switch (playerSkillData.PlayerSkillType)
            {
                case PlayerSkillType.Attack when playerSkillData.IsFlatModifier:
                    _currentAttack += playerSkillData.ModifierAmountPerLevel[_playerSkillTracker.GetSkillLevel(PlayerSkillType.Attack) - 1];
                    break;
                case PlayerSkillType.Attack:
                    _currentAttack *= playerSkillData.ModifierAmountPerLevel[_playerSkillTracker.GetSkillLevel(PlayerSkillType.Attack) - 1];
                    break;
                case PlayerSkillType.Health when playerSkillData.IsFlatModifier:
                    _currentMaxHp += playerSkillData.ModifierAmountPerLevel[_playerSkillTracker.GetSkillLevel(PlayerSkillType.Health) - 1];
                    _unit.IncreaseMaxHp(GetMaxHp());
                    break;
                case PlayerSkillType.Health:
                    _currentMaxHp *= playerSkillData.ModifierAmountPerLevel[_playerSkillTracker.GetSkillLevel(PlayerSkillType.Health) - 1];
                    _unit.IncreaseMaxHp(GetMaxHp());
                    break;
                case PlayerSkillType.MovementSpeed when playerSkillData.IsFlatModifier:
                    _currentMovementSpeed += playerSkillData.ModifierAmountPerLevel[_playerSkillTracker.GetSkillLevel(PlayerSkillType.MovementSpeed) - 1];
                    break;
                case PlayerSkillType.MovementSpeed:
                    _currentMovementSpeed *= playerSkillData.ModifierAmountPerLevel[_playerSkillTracker.GetSkillLevel(PlayerSkillType.MovementSpeed) - 1];
                    break;
                case PlayerSkillType.AttackRange when playerSkillData.IsFlatModifier:
                    _currentAttackRange += playerSkillData.ModifierAmountPerLevel[_playerSkillTracker.GetSkillLevel(PlayerSkillType.AttackRange) - 1];
                    _playerBehavior.UpdateMaxDetectionDistance(_currentAttackRange);
                    OnAttackRangeUpdated?.Invoke();
                    break;
                case PlayerSkillType.AttackRange:
                    _currentAttackRange *= playerSkillData.ModifierAmountPerLevel[_playerSkillTracker.GetSkillLevel(PlayerSkillType.AttackRange) - 1];
                    _playerBehavior.UpdateMaxDetectionDistance(_currentAttackRange);
                    OnAttackRangeUpdated?.Invoke();
                    break;
            }
        }
        
        public int GetAttack()
        {
            return Mathf.FloorToInt(_currentAttack * _bossPowerAttackMultiplier);
        }
        
        public int GetMaxHp()
        {
            return Mathf.FloorToInt(_currentMaxHp * _bossPowerMaxHpMultiplier);
        }
        
        public int GetMovementSpeed()
        {
            return Mathf.FloorToInt(_currentMovementSpeed  * _unitStatusEffectSlowdown.GetMovementSpeedReduction() * _bossPowerMovementSpeedMultiplier);
        }

        public float GetAttackRange()
        {
            return _currentAttackRange * _bossPowerAttackRangeMultiplier;
        }
        
        public float GetAttackSpeedModifier()
        {
            return GetStatsModifier(PlayerSkillType.AttackSpeed) * _playerBaseStats.GetAttackSpeed();
        }

        public float GetDamageReductionModifier()
        {
            return GetStatsModifier(PlayerSkillType.DamageReduction) * _playerBaseStats.GetBaseDamageReductionMultiplier();
        }
        
        public int GetDodgeChance()
        {
            return Mathf.FloorToInt(GetStatsModifier(PlayerSkillType.DodgeChance) + _playerBaseStats.GetBaseDodgeChance());
        }

        public float GetXpGainedModifier()
        {
            return GetStatsModifier(PlayerSkillType.XpGained) * _bossPowerXpGainedMultiplier;
        }
        
        private float GetStatsModifier(PlayerSkillType type)
        {
            PlayerSkillData skillData = SkillManager.Instance.GetSkillDataFromType(type);
            
            if (skillData == null || skillData.ModifierAmountPerLevel.Length == 0)
            {
                return 1.0f;
            }
            
            int skillLevel = _playerSkillTracker.GetSkillLevel(type);
            return skillLevel >= skillData.ModifierAmountPerLevel.Length ? skillData.ModifierAmountPerLevel.Last() : skillData.ModifierAmountPerLevel[skillLevel];
        }

        public void ActivateBossPower()
        {
            _bossPowerXpGainedMultiplier = MatchManager.Instance.GetBossPowerXpGainMultiplier();
            _bossPowerAttackMultiplier = MatchManager.Instance.GetBossPowerAttackMultiplier();
            _bossPowerMaxHpMultiplier = MatchManager.Instance.GetBossPowerMaxHpMultiplier();
            _bossPowerAttackRangeMultiplier = MatchManager.Instance.GetBossPowerAttackRangeMultiplier();
            _bossPowerMovementSpeedMultiplier = MatchManager.Instance.GetBossPowerMovementSpeedMultiplier();
        }

        public void DeactivateBossPower()
        {
            _bossPowerXpGainedMultiplier = BossPowerDisabledValue;
            _bossPowerAttackMultiplier = BossPowerDisabledValue;
            _bossPowerMaxHpMultiplier = BossPowerDisabledValue;
            _bossPowerAttackRangeMultiplier = BossPowerDisabledValue;
            _bossPowerMovementSpeedMultiplier = BossPowerDisabledValue;
        }

        public float GetLevelUpHealingModifier()
        {
            return _playerBaseStats.GetBaseLevelUpHealing() / NumberConstants.OneHundred;
        }

        public float GetCollectRadiusModifier()
        {
            return _playerBaseStats.GetCollectRadius();
        }

        public void ForceAttackRange(float newRange)
        {
            _currentAttackRange = newRange;
            _playerBehavior.UpdateMaxDetectionDistance(newRange);
        }
    }
}