using Application.Data.Equipment;
using UnityEngine;

namespace Game.Equipment
{
    public class EquipmentManager : MonoBehaviour
    {
        public void EquipPiece(EquipmentPieceInstance equipmentPieceInstance)
        {
            EquipmentDataManager.Instance.EquipPiece(equipmentPieceInstance);
        }

        public void UnEquipPiece(EquipmentPieceInstance equipmentPieceInstance)
        {
            EquipmentDataManager.Instance.UnEquipPiece(equipmentPieceInstance);
        }

        public float GetBaseHp()
        {
            int amount = 0;

            foreach (EquipmentPieceInstance equipmentPieceInstance in EquipmentDataManager.Instance.GetEquippedPieces())
            {
                if (equipmentPieceInstance.Id == -1)
                {
                    continue;
                }

                EquipmentPieceData equipmentPieceData = EquipmentDataManager.Instance.GetEquipmentPieceData(equipmentPieceInstance.Id);

                if (equipmentPieceData.EquipmentBaseStatType != EquipmentBaseStatType.Hp)
                {
                    continue;
                }
                
                int slotLevel = EquipmentDataManager.Instance.GetSlotLevel(equipmentPieceData.EquipmentPieceType);
                amount += equipmentPieceData.FlatBaseStatAmount(slotLevel);
            }

            return amount;
        }

        public int GetBaseAttack()
        {
            int amount = 0;

            foreach (EquipmentPieceInstance equipmentPieceInstance in EquipmentDataManager.Instance.GetEquippedPieces())
            {
                if (equipmentPieceInstance.Id == -1)
                {
                    continue;
                }

                EquipmentPieceData equipmentPieceData = EquipmentDataManager.Instance.GetEquipmentPieceData(equipmentPieceInstance.Id);
                
                if (equipmentPieceData.EquipmentBaseStatType != EquipmentBaseStatType.Attack)
                {
                    continue;
                }
                
                int slotLevel = EquipmentDataManager.Instance.GetSlotLevel(equipmentPieceData.EquipmentPieceType);
                amount += equipmentPieceData.FlatBaseStatAmount(slotLevel);
            }

            return amount;
        }

        public float GetHpMultiplier()
        {
            float modifier = 0.0f;

            foreach (EquipmentPieceInstance equipmentPieceInstance in EquipmentDataManager.Instance.GetEquippedPieces())
            {
                if (equipmentPieceInstance == null)
                {
                    continue;
                }

                modifier += GetPieceHpModifier(equipmentPieceInstance);
            }

            return modifier;
        }
        
        public float GetAttackMultiplier()
        {
            float multiplier = 0.0f;

            foreach (EquipmentPieceInstance equipmentPieceInstance in EquipmentDataManager.Instance.GetEquippedPieces())
            {
                if (equipmentPieceInstance == null)
                {
                    continue;
                }

                multiplier += GetPieceAttackMultiplier(equipmentPieceInstance);
            }

            return multiplier;
        }
        
        public float GetAttackRangeMultiplier()
        {
            float multiplier = 0.0f;

            foreach (EquipmentPieceInstance equipmentPieceInstance in EquipmentDataManager.Instance.GetEquippedPieces())
            {
                if (equipmentPieceInstance == null)
                {
                    continue;
                }

                multiplier += GetPieceAttackRangeMultiplier(equipmentPieceInstance);
            }

            return multiplier;
        }
        
        public float GetMovementSpeedMultiplier()
        {
            float multiplier = 0.0f;

            foreach (EquipmentPieceInstance equipmentPieceInstance in EquipmentDataManager.Instance.GetEquippedPieces())
            {
                if (equipmentPieceInstance == null)
                {
                    continue;
                }

                multiplier += GetPieceMovementSpeedMultiplier(equipmentPieceInstance);
            }

            return multiplier;
        }
        
        public float GetLevelUpHealingMultiplier()
        {
            float multiplier = 0.0f;

            foreach (EquipmentPieceInstance equipmentPieceInstance in EquipmentDataManager.Instance.GetEquippedPieces())
            {
                if (equipmentPieceInstance == null)
                {
                    continue;
                }

                multiplier += GetPieceLevelUpHealingMultiplier(equipmentPieceInstance);
            }

            return multiplier;
        }
        
        public float GetDodgeChanceMultiplier()
        {
            float multiplier = 0.0f;

            foreach (EquipmentPieceInstance equipmentPieceInstance in EquipmentDataManager.Instance.GetEquippedPieces())
            {
                if (equipmentPieceInstance == null)
                {
                    continue;
                }

                multiplier += GetPieceDodgeChanceMultiplier(equipmentPieceInstance);
            }

            return multiplier;
        }
        
        public float GetCollectRadiusMultiplier()
        {
            float multiplier = 0.0f;

            foreach (EquipmentPieceInstance equipmentPieceInstance in EquipmentDataManager.Instance.GetEquippedPieces())
            {
                if (equipmentPieceInstance == null)
                {
                    continue;
                }

                multiplier += GetPieceCollectRadiusMultiplier(equipmentPieceInstance);
            }

            return multiplier;
        }

        public float GetDamageReductionMultiplier()
        {
            float multiplier = 0.0f;

            foreach (EquipmentPieceInstance equipmentPieceInstance in EquipmentDataManager.Instance.GetEquippedPieces())
            {
                if (equipmentPieceInstance == null)
                {
                    continue;
                }

                multiplier += GetPieceDamageReductionMultiplier(equipmentPieceInstance);
            }

            return multiplier;
        }
        
        public float GetAttackSpeedMultiplier()
        {
            float multiplier = 0.0f;

            foreach (EquipmentPieceInstance equipmentPieceInstance in EquipmentDataManager.Instance.GetEquippedPieces())
            {
                if (equipmentPieceInstance == null)
                {
                    continue;
                }

                multiplier += GetPieceAttackSpeedMultiplier(equipmentPieceInstance);
            }

            return multiplier;
        }

        private float GetPieceHpModifier(EquipmentPieceInstance equipmentPieceInstance)
        {
            return GetPieceModifierByEquipmentBoostType(EquipmentBoostType.MaxHp, equipmentPieceInstance);
        }

        private float GetPieceAttackMultiplier(EquipmentPieceInstance equipmentPieceInstance)
        {
            return GetPieceModifierByEquipmentBoostType(EquipmentBoostType.Attack, equipmentPieceInstance);
        }

        private float GetPieceAttackRangeMultiplier(EquipmentPieceInstance equipmentPieceInstance)
        {
            return GetPieceModifierByEquipmentBoostType(EquipmentBoostType.AttackRange, equipmentPieceInstance);
        }

        private float GetPieceMovementSpeedMultiplier(EquipmentPieceInstance equipmentPieceInstance)
        {
            return GetPieceModifierByEquipmentBoostType(EquipmentBoostType.MovementSpeed, equipmentPieceInstance);
        }

        private float GetPieceLevelUpHealingMultiplier(EquipmentPieceInstance equipmentPieceInstance)
        {
            return GetPieceModifierByEquipmentBoostType(EquipmentBoostType.LevelUpHealing, equipmentPieceInstance);
        }

        private float GetPieceDodgeChanceMultiplier(EquipmentPieceInstance equipmentPieceInstance)
        {
            return GetPieceModifierByEquipmentBoostType(EquipmentBoostType.DodgeChance, equipmentPieceInstance);
        }

        private float GetPieceCollectRadiusMultiplier(EquipmentPieceInstance equipmentPieceInstance)
        {
            return GetPieceModifierByEquipmentBoostType(EquipmentBoostType.CollectRadius, equipmentPieceInstance);
        }

        private float GetPieceDamageReductionMultiplier(EquipmentPieceInstance equipmentPieceInstance)
        {
            return GetPieceModifierByEquipmentBoostType(EquipmentBoostType.DamageReduction, equipmentPieceInstance);
        }

        private float GetPieceAttackSpeedMultiplier(EquipmentPieceInstance equipmentPieceInstance)
        {
            return GetPieceModifierByEquipmentBoostType(EquipmentBoostType.AttackSpeed, equipmentPieceInstance);
        }

        private float GetPieceModifierByEquipmentBoostType(EquipmentBoostType equipmentBoostType, EquipmentPieceInstance equipmentPieceInstance)
        {
            float multiplier = 0.0f;

            EquipmentPieceData equipmentPieceData = EquipmentDataManager.Instance.GetEquipmentPieceData(equipmentPieceInstance.Id);

            if (!equipmentPieceData)
            {
                return multiplier;
            }

            for (int i = 0; i < equipmentPieceInstance.Rarity; i++)
            {
                if (equipmentPieceData.EquipmentTraitsPerRarity[i].EquipmentBoostType == equipmentBoostType)
                {
                    multiplier += equipmentPieceData.EquipmentTraitsPerRarity[i].EquipmentBoostAmount;
                }
            }

            return multiplier;
        }
    }
}