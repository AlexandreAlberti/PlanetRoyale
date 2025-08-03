using System;
using System.Collections.Generic;
using System.Linq;
using Application.Data.Equipment;
using Game.Equipment;
using UI.Button;
using UnityEngine;
using UnityEngine.UI;

namespace UI.MainMenu
{
    public class EquipmentGrid : MonoBehaviour
    {
        [SerializeField] private Transform _inventoryContainer;
        [SerializeField] private GridLayoutGroup _gridLayoutGroup;
        [SerializeField] private EquipmentPieceButton _equipmentPieceButtonPrefab;
        [SerializeField] private EquipmentSortButton _equipmentSortButton;

        private List<EquipmentPieceButton> _inventoryEquipmentPieces;
        private int _currentSortingMode;
        private bool _isMergeGrid;
        private float _scaleFactor;

        private const int QualitySortingMode = 0;
        private const int SlotSortingMode = 1;
        private const float BaseCellSize = 175;
        private const float MinCellSize = 100;
        private const float MaxCellSize = 300;
        private const float BaseResolutionX = 1440.0f;
        private const float BaseResolutionY = 3088.0f;
        private const string Container = "Container";
        
        public Action<InteractableButton> OnEquipmentPieceClicked;

        public void Initialize(bool isMergeGrid = false)
        {
            _isMergeGrid = isMergeGrid;
            _inventoryEquipmentPieces = new List<EquipmentPieceButton>();
            _equipmentSortButton.OnClicked += SortButton_OnClicked;
            _currentSortingMode = QualitySortingMode;
            AdjustCellSize();

            Refresh();
        }

        private void AdjustCellSize()
        {
            _scaleFactor = (float) Screen.width / Screen.height / (BaseResolutionX / BaseResolutionY);
            Vector2 adjustedCellSize = new Vector2(BaseCellSize, BaseCellSize) * _scaleFactor;
            adjustedCellSize.x = Mathf.Clamp(adjustedCellSize.x, MinCellSize, MaxCellSize);
            adjustedCellSize.y = Mathf.Clamp(adjustedCellSize.y, MinCellSize, MaxCellSize);
            _gridLayoutGroup.cellSize = adjustedCellSize;
        }

        private void SortEquipmentByCurrentFilter()
        {
            switch (_currentSortingMode)
            {
                case QualitySortingMode:
                    SortEquipmentByQuality();
                    break;
                case SlotSortingMode:
                    SortEquipmentBySlot();
                    break;
            }
        }
        
        private void SortEquipmentByQuality()
        {
            List<EquipmentPieceButton> sortedInventoryEquipmentPieces = _inventoryEquipmentPieces
                .OrderByDescending(equipmentPieceButton => equipmentPieceButton.Rarity())
                .ThenBy(equipmentPieceButton => equipmentPieceButton.Id())
                .ToList();

            ApplyListToGrid(sortedInventoryEquipmentPieces);
        }

        private void SortEquipmentBySlot()
        {
            List<EquipmentPieceButton> sortedInventoryEquipmentPieces = _inventoryEquipmentPieces
                .OrderBy(equipmentPieceButton => equipmentPieceButton.Id())
                .ThenByDescending(equipmentPieceButton => equipmentPieceButton.Rarity())
                .ToList();

            ApplyListToGrid(sortedInventoryEquipmentPieces);
        }

        private void ApplyListToGrid(List<EquipmentPieceButton> sortedInventoryEquipmentPieces)
        {
            for (int i = 0; i < sortedInventoryEquipmentPieces.Count; i++)
            {
                foreach (Transform inventoryItem in _inventoryContainer)
                {
                    if (sortedInventoryEquipmentPieces[i].gameObject.GetInstanceID() ==
                        inventoryItem.gameObject.GetInstanceID())
                    {
                        inventoryItem.SetSiblingIndex(i);
                    }
                }
            }
        }
        
        private void SortButton_OnClicked(InteractableButton button)
        {
            if (button is not EquipmentSortButton equipmentSortButton)
            {
                return;
            }
            
            if (_currentSortingMode == QualitySortingMode)
            {
                equipmentSortButton.ChangeToSlotText();
                _currentSortingMode = SlotSortingMode;
            }
            else if (_currentSortingMode == SlotSortingMode)
            {
                equipmentSortButton.ChangeToQualityText();
                _currentSortingMode = QualitySortingMode;
            }
            
            SortEquipmentByCurrentFilter();
        }

        public void Refresh()
        {
            LoadInventory();
            SortEquipmentByCurrentFilter();
        }
        
        private void LoadInventory()
        {
            _inventoryEquipmentPieces.Clear();
            
            foreach (Transform equipmentPiece in _inventoryContainer)
            {
                equipmentPiece.gameObject.SetActive(false);
            }

            if (_isMergeGrid)
            {
                foreach (EquipmentPieceInstance inventoryPiece in EquipmentDataManager.Instance.GetAllEquipmentPieces())
                {
                    if (inventoryPiece.Rarity == (int) EquipmentPieceRarity.Legendary)
                    {
                        continue;
                    }
                    
                    int equipmentPieceCopies = 0;

                    foreach (EquipmentPieceInstance inventoryPieceCopy in EquipmentDataManager.Instance.GetAllEquipmentPieces())
                    {
                        if (inventoryPiece.Rarity == (int) EquipmentPieceRarity.Legendary)
                        {
                            continue;
                        }
                        
                        if (inventoryPieceCopy.Id == inventoryPiece.Id
                            && inventoryPieceCopy.Rarity == inventoryPiece.Rarity)
                        {
                            equipmentPieceCopies++;
                        }
                    }
                    
                    EquipmentPieceButton equipmentPieceButton = Instantiate(_equipmentPieceButtonPrefab, _inventoryContainer);
                    equipmentPieceButton.Initialize(inventoryPiece, showMergeAvailable: equipmentPieceCopies >= 3, showEquippedBanner: inventoryPiece.IsEquipped);
                    equipmentPieceButton.OnClicked -= EquipmentPieceButton_OnClicked;
                    equipmentPieceButton.OnClicked += EquipmentPieceButton_OnClicked;
                    equipmentPieceButton.transform.Find(Container).localScale *= _scaleFactor;
                    _inventoryEquipmentPieces.Add(equipmentPieceButton);
                }
            }
            else
            {
                foreach (EquipmentPieceInstance inventoryPiece in EquipmentDataManager.Instance.GetInventoryPieces())
                {
                    EquipmentPieceButton equipmentPieceButton = Instantiate(_equipmentPieceButtonPrefab, _inventoryContainer);
                    equipmentPieceButton.Initialize(inventoryPiece, isInventoryPiece: true);
                    equipmentPieceButton.OnClicked -= EquipmentPieceButton_OnClicked;
                    equipmentPieceButton.OnClicked += EquipmentPieceButton_OnClicked;
                    equipmentPieceButton.transform.Find(Container).localScale *= _scaleFactor;
                    _inventoryEquipmentPieces.Add(equipmentPieceButton);
                }
            }
        }

        private void EquipmentPieceButton_OnClicked(InteractableButton interactableButton)
        {
            OnEquipmentPieceClicked?.Invoke(interactableButton);
        }
        

        public List<EquipmentPieceButton> GetItems()
        {
            return _inventoryEquipmentPieces;
        }

        public void HideSortButton()
        {
            _equipmentSortButton.gameObject.SetActive(false);
        }

        public void ShowSortButton()
        {
            _equipmentSortButton.ChangeToQualityText();
            _equipmentSortButton.gameObject.SetActive(true);
        }

        public InteractableButton FirstEquipmentPiece()
        {
            return _inventoryContainer.GetChild(0).GetComponent<InteractableButton>();
        }

        public InteractableButton SecondEquipmentPiece()
        {
            return _inventoryContainer.GetChild(1).GetComponent<InteractableButton>();
        }

        public InteractableButton ThirdEquipmentPiece()
        {
            return _inventoryContainer.GetChild(2).GetComponent<InteractableButton>();
        }

        public InteractableButton FourthEquipmentPiece()
        {
            return _inventoryContainer.GetChild(3).GetComponent<InteractableButton>();
        }

        public float GetScaleFactor()
        {
            return _scaleFactor;
        }
    }
}
