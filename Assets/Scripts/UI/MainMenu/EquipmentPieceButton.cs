using Application.Data.Currency;
using Application.Data.Equipment;
using Game.Equipment;
using TMPro;
using UI.Button;
using UnityEngine;
using UnityEngine.UI;

namespace UI.MainMenu
{
    public class EquipmentPieceButton : InteractableButton
    {
        [SerializeField] private bool _isSlot;
        [SerializeField] private Image _slotIcon;
        [SerializeField] private Image _pieceIcon;
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private GameObject _filledSlot;
        [SerializeField] private GameObject _emptySlot;
        [SerializeField] private GameObject _lockedSlot;
        [SerializeField] private GameObject _selectedSlot;
        [SerializeField] private GameObject _grayLayer;
        [SerializeField] private GameObject _equippedBanner;
        [SerializeField] private Sprite[] _equipmentTypeSprites;
        [SerializeField] private GameObject _upgradeArrow;
        [SerializeField] private Animator _upgradeArrowAnimator;
        [SerializeField] private GameObject _pieceIndicator;
        [SerializeField] private GameObject[] _rarityBackgrounds;

        private static readonly int IdleState = Animator.StringToHash("Idle");
        private const int AnimationLayer = -1;
        private const float AnimationTime = 0.0f;
        private const string LevelShort = "Lv.";
        private const string LevelLong = "Level:";
        private const string Bar = "/";

        private EquipmentPieceInstance _equipmentPieceInstance;
        private EquipmentPieceData _equipmentPieceData;
        private bool _isInventoryPiece;
        private EquipmentSlotData _equipmentSlotData;
        private int _slotLevel;
        private int _maxSlotLevel;

        public void Initialize(EquipmentPieceInstance equipmentPieceInstance, bool animateClick = true, bool isInventoryPiece = false, bool isReward = false, bool isDetailPopup = false, bool showMergeAvailable = false, bool showEquippedBanner = false)
        {
            _animateClick = animateClick;
            _isInventoryPiece = isInventoryPiece;
            _equipmentPieceInstance = equipmentPieceInstance;
            _equipmentPieceData = EquipmentDataManager.Instance.GetEquipmentPieceData(equipmentPieceInstance.Id);
            _slotLevel = EquipmentDataManager.Instance.GetSlotLevel(_equipmentPieceData.EquipmentPieceType);
            _maxSlotLevel = EquipmentDataManager.Instance.GetMaxSlotLevel();
            _equipmentSlotData = EquipmentDataManager.Instance.GetEquipmentSlotData();
            
            if (!_equipmentPieceData)
            {
                return;
            }

            if (_isSlot || isDetailPopup)
            {
                _levelText.text = isDetailPopup ? $"{LevelLong} {_slotLevel + 1}{Bar}{_maxSlotLevel}" : $"{LevelShort}{_slotLevel + 1}";
            }
            else
            {
                _levelText.text = "";
            }
            
            _pieceIcon.sprite = _equipmentPieceData.Sprite;
            _slotIcon.sprite = _equipmentTypeSprites[(int)_equipmentPieceData.EquipmentPieceType];
            
            _filledSlot.SetActive(true);
            _emptySlot.SetActive(false);

            _pieceIndicator.SetActive(showMergeAvailable || CanShowItemIndicator());
            _upgradeArrow.SetActive(CanShowUpgradeArrow());
            
            if (_upgradeArrow.activeSelf)
            {
                _upgradeArrowAnimator.Play(IdleState, AnimationLayer, AnimationTime);
            }
            
            for (int i = 0; i < _rarityBackgrounds.Length; i++)
            {
                _rarityBackgrounds[i].SetActive(i == equipmentPieceInstance.Rarity);
            }

            _equippedBanner.SetActive(showEquippedBanner);
            
            Enable();
        }

        private bool CanShowItemIndicator()
        {
            return _isInventoryPiece && (_equipmentPieceInstance.IsNew || EquipmentDataManager.Instance.IsSlotEmpty(_equipmentPieceData.EquipmentPieceType));
        }

        private bool CanShowUpgradeArrow()
        {
            if (!_isSlot)
            {
                return false;
            }
            
            int currentGold = CurrencyDataManager.Instance.CurrentGold();
            int goldUpgradeCost = _equipmentSlotData.GoldUpgradeCostPerLevel[_slotLevel];
            int currentMaterials = CurrencyDataManager.Instance.CurrentMaterials(_equipmentPieceData.EquipmentPieceType);
            int materialUpgradeCost = _equipmentSlotData.MaterialUpgradeCostPerLevel[_slotLevel];

            return currentGold >= goldUpgradeCost && currentMaterials >= materialUpgradeCost && _slotLevel < _maxSlotLevel - 1;
        }

        public void Empty()
        {
            _filledSlot.SetActive(false);
            _emptySlot.SetActive(true);
            _upgradeArrow.SetActive(false);
            _pieceIndicator.SetActive(false);
            Disable();
            _animateClick = false;
            _playSounds = false;
        }

        public EquipmentPieceInstance EquipmentPieceInstance()
        {
            return _equipmentPieceInstance;
        }

        public bool IsSlot()
        {
            return _isSlot;
        }

        public Sprite GetSprite()
        {
            return _pieceIcon.sprite;
        }

        public void DisableSound()
        {
            _playSounds = false;
        }

        public int Rarity()
        {
            return _equipmentPieceInstance.Rarity;
        }

        public int Id()
        {
            return _equipmentPieceInstance.Id;
        }

        public void Lock()
        {
            _lockedSlot.SetActive(true);
        }

        public void MarkAsSelected()
        {
            _selectedSlot.SetActive(true);
        }
        
        public void Restore()
        {
            _lockedSlot.SetActive(false);
            _selectedSlot.SetActive(false);
        }

        public void EnableGrayLayer()
        {
            _levelText.enabled = false;
            _grayLayer.SetActive(true);
        }

        public string Name()
        {
            return _equipmentPieceData.Name;
        }
        
        public string BaseStatName()
        {
            return _equipmentPieceData.EquipmentBaseStatType.ToString();
        }
        
        public int BaseStatAmount()
        {
            return _equipmentPieceData.BaseStatAmount(_slotLevel, _equipmentPieceInstance.Rarity);
        }

        public EquipmentBoostData RarityTrait()
        {
            return _equipmentPieceData.EquipmentTraitsPerRarity[_equipmentPieceInstance.Rarity - 1];
        }

        public void DisableEquippedBanner()
        {
            _equippedBanner.SetActive(false);
        }

        public EquipmentPieceType GetEquipmentPieceType()
        {
            return _equipmentPieceData.EquipmentPieceType;
        }
    }
}
