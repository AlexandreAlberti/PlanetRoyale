using System;
using System.Collections.Generic;
using Game.Equipment;
using UnityEngine;

namespace Application.Data.Equipment
{
    public class EquipmentDataManager : MonoBehaviour
    {
        private const string NumberOfEquipmentUpgrades = "NumberOfEquipmentUpgrades";
        private const string Equipment = "Equipment";
        
        [SerializeField] private EquipmentPieceData[] _equipmentPiecesPool;
        [SerializeField] private EquipmentSlotData _equipmentSlotData;
        [SerializeField] private EquipmentPieceData _bowWeaponEquipmentPiece;
        [SerializeField] private EquipmentPieceData _greatSwordWeaponEquipmentPiece;
        [SerializeField] private EquipmentPieceData _fireBallStaffWeaponEquipmentPiece;
        [SerializeField] private EquipmentPieceData _tridentWeaponEquipmentPiece;
        [SerializeField] private EquipmentPieceData _poisonBombWeaponEquipmentPiece;

        private const int MinItemsForMerge = 3;
        private const int WeaponSlotIndex = 0;
        
        private Equipment _equipment;
        public Action OnEquipmentUpdated { get; set; }
        public Action OnPieceEquipped { get; set; }
        public Action OnPieceUnEquipped { get; set; }

        public static EquipmentDataManager Instance { get; set; }
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(this);
                LoadEquipment();
            }
        }

        public void GiveInitialEquipment()
        {
            EquipmentPieceInstance equipmentPieceInstance = EquipmentPieceFactory.GenerateEquipmentPieceById(_bowWeaponEquipmentPiece.Id);
            equipmentPieceInstance.IsNew = false;
            _equipment.AddPieceToInventory(equipmentPieceInstance);
            _equipment.EquipPiece(equipmentPieceInstance);
            SaveEquipment();
        }
        
        public int GetNumberOfEquipmentUpgrades()
        {
            return PlayerPrefs.GetInt(NumberOfEquipmentUpgrades, 0);
        }

        public void IncreaseNumberOfEquipmentUpgrades()
        {
            PlayerPrefs.SetInt(NumberOfEquipmentUpgrades, GetNumberOfEquipmentUpgrades() + 1);
        }
        
        private void LoadEquipment()
        {
            string equipmentDataString = PlayerPrefs.GetString(Equipment);
            
            if (equipmentDataString == "")
            {
                _equipment = new Equipment();
                SaveEquipment();
            }
            else
            {
                _equipment = JsonUtility.FromJson<Equipment>(equipmentDataString);
            }
        }
        
        public EquipmentPieceInstance[] GetEquippedPieces()
        {
            return _equipment.GetEquippedPieceInstances();
        }

        public List<EquipmentPieceInstance> GetInventoryPieces()
        {
            return _equipment.GetInventoryPieceInstances();
        }

        public void EquipPiece(EquipmentPieceInstance equipmentPieceInstance)
        {
            _equipment.EquipPiece(equipmentPieceInstance);
            SaveEquipment();
            
            OnPieceEquipped?.Invoke();
        }

        private void SaveEquipment()
        {
            string jsonEquipment = JsonUtility.ToJson(_equipment);
            PlayerPrefs.SetString(Equipment, jsonEquipment);
            
            OnEquipmentUpdated?.Invoke();
        }

        public void UnEquipPiece(EquipmentPieceInstance equipmentPieceInstance)
        {
            EquipmentPieceData equipmentPieceData = GetEquipmentPieceData(equipmentPieceInstance.Id);
            _equipment.UnEquipSlot((int)equipmentPieceData.EquipmentPieceType);
            SaveEquipment();
            
            OnPieceUnEquipped?.Invoke();
        }
        
        public EquipmentPieceInstance GetSlotPiece(int slotIndex)
        {
            return _equipment.GetEquippedPieceInstances()[slotIndex];
        }

        public void AddPieceToInventory(EquipmentPieceInstance equipmentPieceInstance)
        {
            _equipment.AddPieceToInventory(equipmentPieceInstance);
            SaveEquipment();
        }
        
        public void AddPieceListToInventory(List<EquipmentPieceInstance> equipmentPiecesInstances)
        {
            foreach (EquipmentPieceInstance equipmentPieceInstance in equipmentPiecesInstances)
            {
                _equipment.AddPieceToInventory(equipmentPieceInstance);
            }

            SaveEquipment();
        }
        
        public void AddPieceListToInventory(EquipmentReward[] equipmentPiecesInstances)
        {
            foreach (EquipmentReward equipmentReward in equipmentPiecesInstances)
            {
                _equipment.AddPieceToInventory( new EquipmentPieceInstance(equipmentReward.Type.Id, equipmentReward.Rarity));
            }

            SaveEquipment();
        }
        
        public EquipmentPieceData GetEquipmentPieceData(int id)
        {
            foreach (EquipmentPieceData equipmentPieceData in _equipmentPiecesPool)
            {
                if (equipmentPieceData.Id == id)
                {
                    return equipmentPieceData;
                }
            }
            
            return null;
        }

        public EquipmentPieceData[] GetEquipmentPiecesPool()
        {
            return _equipmentPiecesPool;
        }

        public void Clear()
        {
            PlayerPrefs.SetString(Equipment, "");
            LoadEquipment();
        }

        public bool HasNewEquipmentPieces()
        {
            foreach (EquipmentPieceInstance equipmentPieceInstance in _equipment.GetInventoryPieceInstances())
            {
                if (equipmentPieceInstance.IsNew)
                {
                    return true;
                }
            }

            return false;
        }

        public void UnsetNewEquipmentPiece(EquipmentPieceInstance equipmentPieceInstance)
        {
            equipmentPieceInstance.IsNew = false;
            SaveEquipment();
        }

        public bool HasEmptySlots()
        {
            foreach (EquipmentPieceInstance inventoryPiece in GetInventoryPieces())
            {
                EquipmentPieceData equipmentPieceData = GetEquipmentPieceData(inventoryPiece.Id);

                if (GetEquippedPieces()[(int)equipmentPieceData.EquipmentPieceType].Id == -1)
                {
                    return true;
                }
            }

            return false;
        }
        
        public bool IsSlotEmpty(EquipmentPieceType equipmentPieceType)
        {
            return GetEquippedPieces()[(int)equipmentPieceType].Id == -1;
        }

        public bool HasAvailableMerges()
        {
            foreach (EquipmentPieceInstance inventoryPiece in _equipment.GetAllEquipmentPieces())
            {
                int equipmentPieceCopies = 0;

                foreach (EquipmentPieceInstance inventoryPieceCopy in _equipment.GetAllEquipmentPieces())
                {
                    if (inventoryPieceCopy.Id == inventoryPiece.Id
                        && inventoryPieceCopy.Rarity == inventoryPiece.Rarity)
                    {
                        equipmentPieceCopies++;

                        if (equipmentPieceCopies >= MinItemsForMerge)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public List<EquipmentPieceInstance> GetAllEquipmentPieces()
        {
            return _equipment.GetAllEquipmentPieces();
        }

        public void UpgradeEquipmentPieceRarity(EquipmentPieceInstance equipmentPieceInstance)
        {
            foreach (EquipmentPieceInstance equipmentPiece in _equipment.GetAllEquipmentPieces())
            {
                if (equipmentPieceInstance.InstanceId == equipmentPiece.InstanceId)
                {
                    equipmentPiece.UpgradeRarity();
                    SaveEquipment();
                    return;
                }
            }
        }

        public void RemoveEquipmentPiece(EquipmentPieceInstance equipmentPieceInstance)
        {
            foreach (EquipmentPieceInstance equipmentPiece in _equipment.GetAllEquipmentPieces())
            {
                if (equipmentPieceInstance.InstanceId == equipmentPiece.InstanceId)
                {
                    _equipment.RemoveEquipmentPiece(equipmentPiece);
                    SaveEquipment();
                    return;
                }
            }
        }
        
        public int GetSlotLevel(EquipmentPieceType equipmentPieceType)
        {
            return _equipment.GetSlotLevel(equipmentPieceType);
        }
        
        public void LevelUpSlot(EquipmentPieceType equipmentPieceType)
        {
            _equipment.LevelUpSlot(equipmentPieceType);
            SaveEquipment();
        }
        
        public bool IsSlotAtMaxLevel(EquipmentPieceType equipmentPieceType)
        {
            return _equipment.IsSlotAtMaxLevel(equipmentPieceType);
        }

        public EquipmentSlotData GetEquipmentSlotData()
        {
            return _equipmentSlotData;
        }

        public int GetMaxSlotLevel()
        {
            return _equipment.GetMaxSlotLevel();
        }

        public WeaponType GetEquippedWeaponType()
        {
            if (_equipment.GetEquippedPieceInstances()[WeaponSlotIndex] == null)
            {
                return WeaponType.None;
            }

            if (_equipment.GetEquippedPieceInstances()[WeaponSlotIndex].Id == _bowWeaponEquipmentPiece.Id)
            {
                return WeaponType.Bow;
            }

            if (_equipment.GetEquippedPieceInstances()[WeaponSlotIndex].Id == _greatSwordWeaponEquipmentPiece.Id)
            {
                return WeaponType.GreatSword;
            }

            if (_equipment.GetEquippedPieceInstances()[WeaponSlotIndex].Id == _fireBallStaffWeaponEquipmentPiece.Id)
            {
                return WeaponType.FireBallStaff;
            }

            if (_equipment.GetEquippedPieceInstances()[WeaponSlotIndex].Id == _tridentWeaponEquipmentPiece.Id)
            {
                return WeaponType.Trident;
            }
            if (_equipment.GetEquippedPieceInstances()[WeaponSlotIndex].Id == _poisonBombWeaponEquipmentPiece.Id)
            {
                return WeaponType.PoisonBomb;
            }
            
            return WeaponType.None;
        }
    }
}