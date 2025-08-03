using System;
using Application.Data.Currency;
using Application.Data.Equipment;
using Application.Telemetry;
using Application.Utils;
using Game.Equipment;
using Game.PlayerUnit;
using TMPro;
using UI.Button;
using UnityEngine;

namespace UI.MainMenu
{
    public class EquipmentScreen : MonoBehaviour
    {
        [SerializeField] private PlayerBaseStats _playerBaseStats;
        [SerializeField] private EquipmentManager _equipmentManager;
        [SerializeField] private EquipmentPieceButton[] _equippedPieceSlotButtons;
        [SerializeField] private TextMeshProUGUI _totalAttackText;
        [SerializeField] private TextMeshProUGUI _totalHpText;
        [SerializeField] private EquipmentPieceDetail _equipmentPieceDetail;
        [SerializeField] private EquipmentMaterial[] _equipmentMaterials;
        [SerializeField] private InteractableButton _equipmentMergeButton;
        [SerializeField] private EquipmentMergeScreen _equipmentMergeScreen;
        [SerializeField] private EquipmentGrid _equipmentGrid;
        [SerializeField] private HeroPrefabManager _humanHeroPrefabManager;

        public Action OnEquipmentPieceButtonClicked;
        public Action OnPiecesUnsetAsNew;
        
        public void OnEnable()
        {
            _equipmentGrid.Initialize();
            _equipmentPieceDetail.OnEquipButtonClicked += EquipmentPieceDetailPanel_OnEquipButtonClicked;
            _equipmentPieceDetail.OnUnEquipButtonClicked += EquipmentPieceDetailPanel_OnUnEquipButtonClicked;
            _equipmentPieceDetail.OnLevelUpButtonClicked += EquipmentPieceDetailPanel_OnLevelUpButtonClicked;
            _equipmentMergeButton.OnClicked += EquipmentMergeButton_OnClicked;
            _equipmentGrid.OnEquipmentPieceClicked += InventoryEquipmentPieceButton_OnClicked;

            CurrencyDataManager.Instance.OnMaterialsUpdated -= CurrencyDataManager_OnMaterialsUpdated;
            CurrencyDataManager.Instance.OnMaterialsUpdated += CurrencyDataManager_OnMaterialsUpdated;
            EquipmentDataManager.Instance.OnEquipmentUpdated -= EquipmentDataManager_OnEquipmentUpdated;
            EquipmentDataManager.Instance.OnEquipmentUpdated += EquipmentDataManager_OnEquipmentUpdated;

            Refresh(true);
        }
        
        private void OnDestroy()
        {
            CurrencyDataManager.Instance.OnMaterialsUpdated -= CurrencyDataManager_OnMaterialsUpdated;
            EquipmentDataManager.Instance.OnEquipmentUpdated -= EquipmentDataManager_OnEquipmentUpdated;
        }

        private void EquipmentDataManager_OnEquipmentUpdated()
        {
            _equipmentGrid.Refresh();
        }

        private void CurrencyDataManager_OnMaterialsUpdated()
        {
            LoadMaterials();
        }

        private void EquipmentMergeButton_OnClicked(InteractableButton button)
        {
            _equipmentMergeScreen.OnBackButtonPressed += EquipmentMergeScreen_OnBackButtonPressed;
            _equipmentMergeScreen.Show();
            _equipmentMergeScreen.Initialize();
        }

        private void EquipmentMergeScreen_OnBackButtonPressed()
        {
            _equipmentMergeScreen.OnBackButtonPressed -= EquipmentMergeScreen_OnBackButtonPressed;
            Refresh(true);
        }

        private void OnDisable()
        {
            _equipmentPieceDetail.OnEquipButtonClicked -= EquipmentPieceDetailPanel_OnEquipButtonClicked;
            _equipmentPieceDetail.OnUnEquipButtonClicked -= EquipmentPieceDetailPanel_OnUnEquipButtonClicked;
            _equipmentPieceDetail.OnLevelUpButtonClicked -= EquipmentPieceDetailPanel_OnLevelUpButtonClicked;
            _equipmentMergeButton.OnClicked -= EquipmentMergeButton_OnClicked;
            _equipmentGrid.OnEquipmentPieceClicked -= InventoryEquipmentPieceButton_OnClicked;

            foreach (EquipmentPieceInstance equipmentPieceInstance in EquipmentDataManager.Instance.GetInventoryPieces())
            {
                if (equipmentPieceInstance.IsNew)
                {
                    EquipmentDataManager.Instance.UnsetNewEquipmentPiece(equipmentPieceInstance);
                }
            }

            OnPiecesUnsetAsNew?.Invoke();
        }

        private void EquipmentPieceDetailPanel_OnLevelUpButtonClicked(EquipmentPieceType equipmentPieceType)
        {
            EquipmentSlotData equipmentSlotData = EquipmentDataManager.Instance.GetEquipmentSlotData();
            int slotLevel = EquipmentDataManager.Instance.GetSlotLevel(equipmentPieceType);
            int goldUpgradeCost = equipmentSlotData.GoldUpgradeCostPerLevel[slotLevel];
            int materialUpgradeCost = equipmentSlotData.MaterialUpgradeCostPerLevel[slotLevel];

            CurrencyDataManager.Instance.RemoveGold(goldUpgradeCost);
            CurrencyDataManager.Instance.RemoveMaterials(equipmentPieceType, materialUpgradeCost);
            EquipmentDataManager.Instance.IncreaseNumberOfEquipmentUpgrades();
            EquipmentDataManager.Instance.LevelUpSlot(equipmentPieceType);
            SendAnalytics(slotLevel, goldUpgradeCost, materialUpgradeCost, equipmentPieceType);
            Refresh();
        }
        
        private static void SendAnalytics(int slotLevel, int goldUpgradeCost, int materialUpgradeCost, EquipmentPieceType equipmentPieceType)
        {
            AmplitudeManager.Instance.EquipmentSlotUpgrade(equipmentPieceType.ToString(), slotLevel, EquipmentDataManager.Instance.GetNumberOfEquipmentUpgrades());
            AmplitudeManager.Instance.SoftCurrencyBalanceChange(SoftCurrencyType.Gold, -goldUpgradeCost, CurrencyDataManager.Instance.CurrentGold(), SoftCurrencyModificationType.EquipmentLevelUp);
        
            if (materialUpgradeCost > 0)
            {
                AmplitudeManager.Instance.SoftCurrencyBalanceChange(EnumUtils.SoftCurrencyTypeByPieceType(equipmentPieceType), -materialUpgradeCost, CurrencyDataManager.Instance.CurrentMaterials(equipmentPieceType),
                    SoftCurrencyModificationType.EquipmentLevelUp);
            }
        }

        private void Refresh(bool refreshGrid = false)
        {
            if (refreshGrid)
            {
                _equipmentGrid.Refresh();
            }

            WeaponType weaponType = EquipmentDataManager.Instance.GetEquippedWeaponType();
            GameObject playerPrefab = _humanHeroPrefabManager.GetHeroPrefabByWeaponType(weaponType);
            
            LoadMaterials();
            LoadSlots();
            LoadStats();

            _humanHeroPrefabManager.DisableAllPrefabs();
            playerPrefab.SetActive(true);
        }

        private void LoadMaterials()
        {
            _equipmentMaterials[(int)EquipmentPieceType.Weapon].Initialize(EquipmentPieceType.Weapon);
            _equipmentMaterials[(int)EquipmentPieceType.Locket].Initialize(EquipmentPieceType.Locket);
            _equipmentMaterials[(int)EquipmentPieceType.Ring].Initialize(EquipmentPieceType.Ring);
            _equipmentMaterials[(int)EquipmentPieceType.Helm].Initialize(EquipmentPieceType.Helm);
            _equipmentMaterials[(int)EquipmentPieceType.Armor].Initialize(EquipmentPieceType.Armor);
            _equipmentMaterials[(int)EquipmentPieceType.Boots].Initialize(EquipmentPieceType.Boots);
        }

        private void LoadStats()
        {
            _totalAttackText.text = $"{Mathf.CeilToInt(_playerBaseStats.GetBaseAttack())}";
            _totalHpText.text = $"{Mathf.CeilToInt(_playerBaseStats.GetBaseMaxHp())}";
        }

        private void InventoryEquipmentPieceButton_OnClicked(InteractableButton interactableButton)
        {
            if (interactableButton is not EquipmentPieceButton equipmentPieceButton)
            {
                return;
            }

            EquipmentDataManager.Instance.UnsetNewEquipmentPiece(equipmentPieceButton.EquipmentPieceInstance());
            _equipmentPieceDetail.Initialize(equipmentPieceButton.EquipmentPieceInstance(), equipmentPieceButton.IsSlot());
            _equipmentPieceDetail.Show();

            OnEquipmentPieceButtonClicked?.Invoke();
        }

        private void EquipmentPieceDetailPanel_OnEquipButtonClicked(EquipmentPieceInstance equipmentPieceInstance)
        {
            _equipmentPieceDetail.Hide();
            _equipmentManager.EquipPiece(equipmentPieceInstance);
            Refresh(true);
        }

        private void EquipmentPieceDetailPanel_OnUnEquipButtonClicked(EquipmentPieceInstance equipmentPieceInstance)
        {
            _equipmentPieceDetail.Hide();
            _equipmentManager.UnEquipPiece(equipmentPieceInstance);
            Refresh(true);
        }

        private void LoadSlots()
        {
            LoadSlot(EquipmentPieceType.Weapon);
            LoadSlot(EquipmentPieceType.Locket);
            LoadSlot(EquipmentPieceType.Ring);
            LoadSlot(EquipmentPieceType.Helm);
            LoadSlot(EquipmentPieceType.Armor);
            LoadSlot(EquipmentPieceType.Boots);
        }

        private void LoadSlot(EquipmentPieceType equipmentPieceType)
        {
            EquipmentPieceInstance equippedPiece = EquipmentDataManager.Instance.GetEquippedPieces()[(int)equipmentPieceType];

            if (equippedPiece.Id == -1)
            {
                _equippedPieceSlotButtons[(int)equipmentPieceType].Empty();
                _equippedPieceSlotButtons[(int)equipmentPieceType].OnClicked -= SlotEquipmentPieceButton_OnClicked;
            }
            else
            {
                _equippedPieceSlotButtons[(int)equipmentPieceType].Initialize(equippedPiece);
                _equippedPieceSlotButtons[(int)equipmentPieceType].OnClicked += SlotEquipmentPieceButton_OnClicked;
            }
        }

        private void SlotEquipmentPieceButton_OnClicked(InteractableButton interactableButton)
        {
            if (interactableButton is not EquipmentPieceButton equipmentPieceButton)
            {
                return;
            }

            _equipmentPieceDetail.Initialize(equipmentPieceButton.EquipmentPieceInstance(), equipmentPieceButton.IsSlot());
            _equipmentPieceDetail.Show();
        }

        public InteractableButton FirstEquipmentPiece()
        {
            return _equipmentGrid.FirstEquipmentPiece();
        }

        public InteractableButton SecondEquipmentPiece()
        {
            return _equipmentGrid.SecondEquipmentPiece();
        }

        public InteractableButton ThirdEquipmentPiece()
        {
            return _equipmentGrid.ThirdEquipmentPiece();
        }

        public float GetGridScaleFactor()
        {
            return _equipmentGrid.GetScaleFactor();
        }
    }
}