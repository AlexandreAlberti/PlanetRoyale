using Application.Data.Equipment;
using UnityEngine;

namespace UI.MainMenu.RedDot
{
    public class MergeAvailableManager : MonoBehaviour
    {
        [SerializeField] private GameObject _indicator;
        [SerializeField] private EquipmentMergeScreen _equipmentMergeScreen;
        
        public void OnEnable()
        {
            _equipmentMergeScreen.OnBackButtonPressed += EquipmentMergeScreen_OnBackButtonPressed;
            EquipmentDataManager.Instance.OnEquipmentUpdated -= EquipmentDataManager_OnEquipmentUpdated;
            EquipmentDataManager.Instance.OnEquipmentUpdated += EquipmentDataManager_OnEquipmentUpdated;
            RefreshIndicator();
        }

        private void OnDestroy()
        {
            _equipmentMergeScreen.OnBackButtonPressed -= EquipmentMergeScreen_OnBackButtonPressed;
            EquipmentDataManager.Instance.OnEquipmentUpdated -= EquipmentDataManager_OnEquipmentUpdated;
        }

        
        private void EquipmentDataManager_OnEquipmentUpdated()
        {
            RefreshIndicator();
        }
        
        private void EquipmentMergeScreen_OnBackButtonPressed()
        {
            RefreshIndicator();
        }

        private void RefreshIndicator()
        {
            _indicator.SetActive(EquipmentDataManager.Instance.HasAvailableMerges());
        }
    }
}
