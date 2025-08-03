using Application.Data.Equipment;
using Application.Data.Hero;
using Application.Utils;
using Game.Equipment;
using Game.Talent;
using UnityEngine;

namespace Game.PlayerUnit
{
    [RequireComponent(typeof(EquipmentManager))]
    public class PlayerBaseStats : MonoBehaviour
    {
        [SerializeField] private BaseStatsData _baseStatsData;
        [SerializeField] private EquipmentManager _equipmentManager;
        
        public int GetBaseAttack()
        {
            float baseValue = _baseStatsData.BaseAttack + _equipmentManager.GetBaseAttack() + TalentManager.Instance.GetAttackTalentAddition();
            float multiplier = _equipmentManager.GetAttackMultiplier();
            
            return Mathf.CeilToInt(baseValue + baseValue * multiplier / NumberConstants.OneHundred);
        }
        
        public int GetBaseMaxHp()
        {
            float baseValue = _baseStatsData.BaseMaxHp + _equipmentManager.GetBaseHp() + TalentManager.Instance.GetMaxHpTalentAddition();
            float multiplier = _equipmentManager.GetHpMultiplier();
            
            return Mathf.CeilToInt(baseValue + baseValue * multiplier / NumberConstants.OneHundred);
        }

        public float GetBaseAttackRange()
        {
            WeaponType weaponType = EquipmentDataManager.Instance.GetEquippedWeaponType();
            
            float baseValue;

            switch (weaponType)
            {
                default:
                case WeaponType.None:
                case WeaponType.Bow:
                case WeaponType.FireBallStaff:
                case WeaponType.PoisonBomb:
                    baseValue = _baseStatsData.BaseRangedAttackRange;
                    break;
                case WeaponType.GreatSword:
                    baseValue = _baseStatsData.BaseGreatSwordAttackRange;
                    break;
                case WeaponType.Trident:
                    baseValue = _baseStatsData.BaseTridentAttackRange;
                    break;
            }
            
            float multiplier = _equipmentManager.GetAttackRangeMultiplier() + TalentManager.Instance.GetAttackRangeTalentPercent();
            
            return baseValue + baseValue * multiplier / NumberConstants.OneHundred;
        }

        public float GetBaseMovementSpeed()
        {
            float baseValue = _baseStatsData.BaseMovementSpeed;
            float multiplier = _equipmentManager.GetMovementSpeedMultiplier() + TalentManager.Instance.GetMovementSpeedTalentPercent();
            
            return baseValue + baseValue * multiplier / NumberConstants.OneHundred;
        }

        public float GetBaseLevelUpHealing()
        {
            float baseValue = _baseStatsData.BaseLevelUpHealing;
            float multiplier = _equipmentManager.GetLevelUpHealingMultiplier() + TalentManager.Instance.GetHpRecoveredOnLevelUpTalentPercent();

            return baseValue + baseValue * multiplier / NumberConstants.OneHundred;
        }

        public float GetBaseDodgeChance()
        {
            return _equipmentManager.GetDodgeChanceMultiplier();
        }

        public float GetCollectRadius()
        {
            float baseValue = _baseStatsData.BaseCollectRadius;
            float multiplier = _equipmentManager.GetCollectRadiusMultiplier() + TalentManager.Instance.GetCollectRadiusTalentMultiplier();

            return baseValue + baseValue * multiplier / NumberConstants.OneHundred;
        }
        
        public float GetBaseDamageReductionMultiplier()
        {
            float multiplier = _equipmentManager.GetDamageReductionMultiplier();
            
            return NumberConstants.One - multiplier / NumberConstants.OneHundred;
        }

        public float GetAttackSpeed()
        {
            float baseValue = _baseStatsData.BaseAttackSpeed;
            float multiplier = _equipmentManager.GetAttackSpeedMultiplier() + TalentManager.Instance.GetAttackSpeedTalentMultiplier();
            
            return baseValue - baseValue * multiplier / NumberConstants.OneHundred;
        }
    }
}