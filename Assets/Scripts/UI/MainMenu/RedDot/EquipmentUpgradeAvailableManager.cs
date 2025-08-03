using Application.Data.Currency;
using Application.Data.Equipment;
using UnityEngine;

namespace UI.MainMenu.RedDot
{
    public class EquipmentUpgradeAvailableManager : MonoBehaviour
    {
        [SerializeField] private GameObject _indicator;
        
        private EquipmentScreen _equipmentScreen;

        private void Start()
        {
            _equipmentScreen = FindObjectOfType<EquipmentScreen>(true);
            
            CurrencyDataManager.Instance.OnGoldUpdated += CurrencyDataManager_OnGoldUpdated;
            EquipmentDataManager.Instance.OnPieceEquipped += EquipmentDataManager_OnPieceEquipped;
            EquipmentDataManager.Instance.OnPieceUnEquipped += EquipmentDataManager_OnPieceUnEquipped;
            _equipmentScreen.OnPiecesUnsetAsNew += EquipmentScreen_OnPiecesUnsetAsNew;
            _equipmentScreen.OnEquipmentPieceButtonClicked += EquipmentScreen_OnEquipmentPieceButtonClicked;
            
            RefreshIndicator();
        }

        private void OnDestroy()
        {
            CurrencyDataManager.Instance.OnGoldUpdated -= CurrencyDataManager_OnGoldUpdated;
            EquipmentDataManager.Instance.OnPieceEquipped -= EquipmentDataManager_OnPieceEquipped;
            EquipmentDataManager.Instance.OnPieceUnEquipped -= EquipmentDataManager_OnPieceUnEquipped;
        }

        private void EquipmentScreen_OnEquipmentPieceButtonClicked()
        {
            RefreshIndicator();
        }

        private void EquipmentDataManager_OnPieceUnEquipped()
        {
            RefreshIndicator();
        }

        private void EquipmentScreen_OnPiecesUnsetAsNew()
        {
            RefreshIndicator();
        }

        private void EquipmentDataManager_OnPieceEquipped()
        {
            RefreshIndicator();
        }

        private void CurrencyDataManager_OnGoldUpdated(int goldAmount)
        {
            RefreshIndicator();
        }

        private void RefreshIndicator()
        {
            _indicator.SetActive(EquipmentDataManager.Instance.HasEmptySlots() || EquipmentDataManager.Instance.HasNewEquipmentPieces() || EquipmentDataManager.Instance.HasAvailableMerges());
        }
    }
}
