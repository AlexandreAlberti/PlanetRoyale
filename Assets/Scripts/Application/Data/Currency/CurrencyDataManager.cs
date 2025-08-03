using System;
using Application.Data.Equipment;
using UnityEngine;

namespace Application.Data.Currency
{
    public class CurrencyDataManager : MonoBehaviour
    {
        private const string Gold = "Gold";
        private const string Gems = "Gems";
        private const string SilverKeys = "SilverKeys";
        private const string GoldenKeys = "GoldenKeys";
        private const string Trophies = "Trophies";
        private const string WeaponMaterials = "WeaponMaterials";
        private const string LocketMaterials = "LocketMaterials";
        private const string RingMaterials = "RingMaterials";
        private const string HelmMaterials = "HelmMaterials";
        private const string ArmorMaterials = "ArmorMaterials";
        private const string BootsMaterials = "BootsMaterials";

        public static CurrencyDataManager Instance { get; private set; }

        public Action<int> OnGoldUpdated;
        public Action<int> OnGemsUpdated;
        public Action<int> OnSilverKeysUpdated;
        public Action<int> OnGoldenKeysUpdated;
        public Action OnMaterialsUpdated;
        public Action<int> OnTrophiesUpdated;

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
            }
        }

        public void GainGold(int amount)
        {
            int currentGoldAmount = PlayerPrefs.GetInt(Gold);
            PlayerPrefs.SetInt(Gold, currentGoldAmount + amount);

            OnGoldUpdated?.Invoke(currentGoldAmount + amount);
        }

        public void RemoveGold(int amount)
        {
            int remainingGold = PlayerPrefs.GetInt(Gold) - amount;

            if (remainingGold < 0)
            {
                remainingGold = 0;
            }

            PlayerPrefs.SetInt(Gold, remainingGold);

            OnGoldUpdated?.Invoke(remainingGold);
        }

        public int CurrentGold()
        {
            return PlayerPrefs.GetInt(Gold);
        }

        public void GainGems(int amount)
        {
            int currentGemsAmount = PlayerPrefs.GetInt(Gems);
            PlayerPrefs.SetInt(Gems, currentGemsAmount + amount);

            OnGemsUpdated?.Invoke(currentGemsAmount + amount);
        }

        public void RemoveGems(int amount)
        {
            int remainingGems = PlayerPrefs.GetInt(Gems) - amount;

            if (remainingGems < 0)
            {
                remainingGems = 0;
            }

            PlayerPrefs.SetInt(Gems, remainingGems);

            OnGemsUpdated?.Invoke(remainingGems);
        }

        public int CurrentGems()
        {
            return PlayerPrefs.GetInt(Gems);
        }

        public void GainSilverKeys(int amount)
        {
            int currentSilverKeysAmount = PlayerPrefs.GetInt(SilverKeys);
            PlayerPrefs.SetInt(SilverKeys, currentSilverKeysAmount + amount);

            OnSilverKeysUpdated?.Invoke(currentSilverKeysAmount + amount);
        }

        public void RemoveSilverKeys(int amount)
        {
            int remainingSilverKeys = PlayerPrefs.GetInt(SilverKeys) - amount;

            if (remainingSilverKeys < 0)
            {
                remainingSilverKeys = 0;
            }

            PlayerPrefs.SetInt(SilverKeys, remainingSilverKeys);

            OnSilverKeysUpdated?.Invoke(remainingSilverKeys);
        }

        public int CurrentSilverKeys()
        {
            return PlayerPrefs.GetInt(SilverKeys);
        }

        public void GainGoldenKeys(int amount)
        {
            int currentGoldenKeysAmount = PlayerPrefs.GetInt(GoldenKeys);
            PlayerPrefs.SetInt(GoldenKeys, currentGoldenKeysAmount + amount);

            OnGoldenKeysUpdated?.Invoke(currentGoldenKeysAmount + amount);
        }

        public void RemoveGoldenKeys(int amount)
        {
            int remainingGoldenKeys = PlayerPrefs.GetInt(GoldenKeys) - amount;

            if (remainingGoldenKeys < 0)
            {
                remainingGoldenKeys = 0;
            }

            PlayerPrefs.SetInt(GoldenKeys, remainingGoldenKeys);

            OnGoldenKeysUpdated?.Invoke(remainingGoldenKeys);
        }

        public int CurrentGoldenKeys()
        {
            return PlayerPrefs.GetInt(GoldenKeys);
        }


        public void UpdateTrophies(int amount)
        {
            int newTrophiesAmount = PlayerPrefs.GetInt(Trophies) + amount;

            if (newTrophiesAmount < 0)
            {
                newTrophiesAmount = 0;
            }

            PlayerPrefs.SetInt(Trophies, newTrophiesAmount);
            OnTrophiesUpdated?.Invoke(newTrophiesAmount);
        }
        
        public void ForceTrophies(int amount)
        {
            PlayerPrefs.SetInt(Trophies, amount);
        }

        public int CurrentTrophies()
        {
            return PlayerPrefs.GetInt(Trophies);
        }

        public void GainMaterials(EquipmentPieceType equipmentPieceType, int amount)
        {
            int currentAmount;

            switch (equipmentPieceType)
            {
                case EquipmentPieceType.Weapon:
                    currentAmount = PlayerPrefs.GetInt(WeaponMaterials);
                    PlayerPrefs.SetInt(WeaponMaterials, currentAmount + amount);
                    break;
                case EquipmentPieceType.Ring:
                    currentAmount = PlayerPrefs.GetInt(RingMaterials);
                    PlayerPrefs.SetInt(RingMaterials, currentAmount + amount);
                    break;
                case EquipmentPieceType.Locket:
                    currentAmount = PlayerPrefs.GetInt(LocketMaterials);
                    PlayerPrefs.SetInt(LocketMaterials, currentAmount + amount);
                    break;
                case EquipmentPieceType.Helm:
                    currentAmount = PlayerPrefs.GetInt(HelmMaterials);
                    PlayerPrefs.SetInt(HelmMaterials, currentAmount + amount);
                    break;
                case EquipmentPieceType.Armor:
                    currentAmount = PlayerPrefs.GetInt(ArmorMaterials);
                    PlayerPrefs.SetInt(ArmorMaterials, currentAmount + amount);
                    break;
                case EquipmentPieceType.Boots:
                    currentAmount = PlayerPrefs.GetInt(BootsMaterials);
                    PlayerPrefs.SetInt(BootsMaterials, currentAmount + amount);
                    break;
            }

            OnMaterialsUpdated?.Invoke();
        }

        public void RemoveMaterials(EquipmentPieceType equipmentPieceType, int amount)
        {
            switch (equipmentPieceType)
            {
                case EquipmentPieceType.Weapon:
                    RemoveMaterial(WeaponMaterials, amount);
                    break;
                case EquipmentPieceType.Ring:
                    RemoveMaterial(RingMaterials, amount);
                    break;
                case EquipmentPieceType.Locket:
                    RemoveMaterial(LocketMaterials, amount);
                    break;
                case EquipmentPieceType.Helm:
                    RemoveMaterial(HelmMaterials, amount);
                    break;
                case EquipmentPieceType.Armor:
                    RemoveMaterial(ArmorMaterials, amount);
                    break;
                case EquipmentPieceType.Boots:
                    RemoveMaterial(BootsMaterials, amount);
                    break;
            }

            OnMaterialsUpdated?.Invoke();
        }

        private static void RemoveMaterial(string key, int amount)
        {
            int remainingAmount = PlayerPrefs.GetInt(key) - amount;

            if (remainingAmount < 0)
            {
                remainingAmount = 0;
            }

            PlayerPrefs.SetInt(key, remainingAmount);
        }

        public int CurrentMaterials(EquipmentPieceType equipmentPieceType)
        {
            switch (equipmentPieceType)
            {
                case EquipmentPieceType.Weapon:
                    return PlayerPrefs.GetInt(WeaponMaterials);
                case EquipmentPieceType.Locket:
                    return PlayerPrefs.GetInt(LocketMaterials);
                case EquipmentPieceType.Ring:
                    return PlayerPrefs.GetInt(RingMaterials);
                case EquipmentPieceType.Helm:
                    return PlayerPrefs.GetInt(HelmMaterials);
                case EquipmentPieceType.Armor:
                    return PlayerPrefs.GetInt(ArmorMaterials);
                case EquipmentPieceType.Boots:
                    return PlayerPrefs.GetInt(BootsMaterials);
            }

            return 0;
        }

        public void Clear()
        {
            PlayerPrefs.SetInt(Gold, 0);
            PlayerPrefs.SetInt(Gems, 0);
            PlayerPrefs.SetInt(Trophies, 0);
            PlayerPrefs.SetInt(SilverKeys, 0);
            PlayerPrefs.SetInt(GoldenKeys, 0);
            PlayerPrefs.SetInt(WeaponMaterials, 0);
            PlayerPrefs.SetInt(LocketMaterials, 0);
            PlayerPrefs.SetInt(RingMaterials, 0);
            PlayerPrefs.SetInt(HelmMaterials, 0);
            PlayerPrefs.SetInt(ArmorMaterials, 0);
            PlayerPrefs.SetInt(BootsMaterials, 0);
        }
    }
}