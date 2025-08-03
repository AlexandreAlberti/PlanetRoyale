using System;
using System.Collections.Generic;
using Game.Equipment;

namespace Application.Data.Equipment
{
    [Serializable]
    public class Equipment
    {
        private const int NumberOfSlots = 6;
        private const int MaxSlotLevel = 100;
        
        public EquipmentPieceInstance[] _equippedPieceInstances = new EquipmentPieceInstance[NumberOfSlots];
        public List<EquipmentPieceInstance> _inventoryPieceInstances = new();
        public int[] _slotLevels = new int[NumberOfSlots];

        public EquipmentPieceInstance[] GetEquippedPieceInstances()
        {
            return _equippedPieceInstances;
        }

        public List<EquipmentPieceInstance> GetInventoryPieceInstances()
        {
            return _inventoryPieceInstances;
        }

        public List<EquipmentPieceInstance> GetAllEquipmentPieces()
        {
            List<EquipmentPieceInstance> allEquipmentPieces = new List<EquipmentPieceInstance>();

            foreach (EquipmentPieceInstance equipmentPieceInstance in _equippedPieceInstances)
            {
                if (equipmentPieceInstance.Id != -1)
                {
                    allEquipmentPieces.Add(equipmentPieceInstance);
                }
            }
            
            allEquipmentPieces.AddRange(_inventoryPieceInstances);
            
            return allEquipmentPieces;
        }

        public void SetEquippedPieceInstances(EquipmentPieceInstance[] equipmentPieceInstances)
        {
            _equippedPieceInstances = equipmentPieceInstances;
        }

        public void SetInventoryPieceInstances(List<EquipmentPieceInstance> inventoryPieceInstances)
        {
            _inventoryPieceInstances = inventoryPieceInstances;
        }

        public void EquipPiece(EquipmentPieceInstance equipmentPieceInstance)
        {
            equipmentPieceInstance.Equip();
            EquipmentPieceData equipmentPieceData =
                EquipmentDataManager.Instance.GetEquipmentPieceData(equipmentPieceInstance.Id);

            if (!equipmentPieceData)
            {
                return;
            }

            if (_equippedPieceInstances[(int)equipmentPieceData.EquipmentPieceType].Id != -1)
            {
                UnEquipSlot((int)equipmentPieceData.EquipmentPieceType);
            }

            _equippedPieceInstances[(int)equipmentPieceData.EquipmentPieceType] = equipmentPieceInstance;
            _inventoryPieceInstances.Remove(equipmentPieceInstance);
        }

        public void UnEquipSlot(int slotIndex)
        {
            _equippedPieceInstances[slotIndex].UnEquip();
            AddPieceToInventory(_equippedPieceInstances[slotIndex]);
            _equippedPieceInstances[slotIndex] = new EquipmentPieceInstance();
        }

        public void UnEquipPiece(EquipmentPieceInstance equipmentPieceInstance)
        {
            for (int i = 0; i < _equippedPieceInstances.Length; i++)
            {
                if (_equippedPieceInstances[i] == equipmentPieceInstance)
                {
                    UnEquipSlot(i);
                }
            }
        }

        public void AddPieceToInventory(EquipmentPieceInstance equipmentPieceInstance)
        {
            _inventoryPieceInstances.Add(equipmentPieceInstance);
        }

        public void RemoveEquipmentPiece(EquipmentPieceInstance equipmentPieceInstance)
        {
            if (equipmentPieceInstance.IsEquipped)
            {
                for (int i = 0; i < _equippedPieceInstances.Length; i++)
                {
                    if (_equippedPieceInstances[i] == equipmentPieceInstance)
                    {
                        UnEquipSlot(i);
                    }
                }
            }
            
            _inventoryPieceInstances.Remove(equipmentPieceInstance);
        }

        public void LevelUpSlot(EquipmentPieceType equipmentPieceType)
        {
            _slotLevels[(int) equipmentPieceType]++;
        }

        public bool IsSlotAtMaxLevel(EquipmentPieceType equipmentPieceType)
        {
            return _slotLevels[(int) equipmentPieceType] >= MaxSlotLevel;
        }

        public int GetSlotLevel(EquipmentPieceType equipmentPieceType)
        {
            return _slotLevels[(int) equipmentPieceType];
        }

        public int GetMaxSlotLevel()
        {
            return MaxSlotLevel;
        }
    }
}