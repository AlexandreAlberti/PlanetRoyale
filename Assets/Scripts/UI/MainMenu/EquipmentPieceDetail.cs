using System;
using System.Collections;
using Application.Data.Currency;
using Application.Data.Equipment;
using Application.Sound;
using DG.Tweening;
using Game.Equipment;
using TMPro;
using UI.Button;
using UnityEngine;

namespace UI.MainMenu
{
    public class EquipmentPieceDetail : Popup
    {
        [SerializeField] private EquipPieceButton _equipButton;
        [SerializeField] private GameObject _equipButtonIndicator;
        [SerializeField] private EquipPieceButton _unEquipButton;
        [SerializeField] private LevelUpPieceButton _levelUpButton;
        [SerializeField] private EquipmentPieceButton _equipmentPieceButton;
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [SerializeField] private TextMeshProUGUI _baseStatText;
        [SerializeField] private TextMeshProUGUI _upgradeGoldText;
        [SerializeField] private TextMeshProUGUI _upgradeMaterialsText;
        [SerializeField] private EquipmentMaterial _upgradeMaterial;
        [SerializeField] private RarityTraitDescription[] _rarityTraitDescriptions;
        [SerializeField] private GameObject _upgradeCost;
        
        private const float RefreshEquipmentPieceTime = 0.1f;

        public Action<EquipmentPieceInstance> OnEquipButtonClicked;
        public Action<EquipmentPieceInstance> OnUnEquipButtonClicked;
        public Action<EquipmentPieceType> OnLevelUpButtonClicked;
        private bool _isSlot;
        private Coroutine _enableButtonsCoroutine;
        private EquipmentPieceInstance _equipmentPieceInstance;
        private EquipmentPieceData _equipmentPieceData;
        private EquipmentSlotData _equipmentSlotData;
        private int _slotLevel;

        public void Initialize(EquipmentPieceInstance equipmentPieceInstance, bool isSlot)
        {
            _isSlot = isSlot;
            _equipmentPieceData = EquipmentDataManager.Instance.GetEquipmentPieceData(equipmentPieceInstance.Id);
            _equipmentSlotData = EquipmentDataManager.Instance.GetEquipmentSlotData();
            _slotLevel = EquipmentDataManager.Instance.GetSlotLevel(_equipmentPieceData.EquipmentPieceType);
            CurrencyDataManager.Instance.OnMaterialsUpdated -= CurrencyDataManager_OnMaterialsUpdated;
            CurrencyDataManager.Instance.OnMaterialsUpdated += CurrencyDataManager_OnMaterialsUpdated;
            CurrencyDataManager.Instance.OnGoldUpdated -= CurrencyDataManager_OnGoldUpdated;
            CurrencyDataManager.Instance.OnGoldUpdated += CurrencyDataManager_OnGoldUpdated;
            
            StartCoroutine(Refresh(equipmentPieceInstance));
        }
        
        private void OnDestroy()
        {
            CurrencyDataManager.Instance.OnGoldUpdated -= CurrencyDataManager_OnGoldUpdated;
            CurrencyDataManager.Instance.OnMaterialsUpdated -= CurrencyDataManager_OnMaterialsUpdated;
        }
        
        private void CurrencyDataManager_OnGoldUpdated(int goldAmount)
        {
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(Refresh(_equipmentPieceInstance));
            }
        }

        private void CurrencyDataManager_OnMaterialsUpdated()
        {
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(Refresh(_equipmentPieceInstance));
            }
        }

        protected override IEnumerator WaitAndSubscribeButtons()
        {
            yield return new WaitForSeconds(AnimationDuration);

            _closeButton.OnClicked += CloseButton_OnClick;
            _clickableBackground.OnClicked += CloseButton_OnClick;
            _clickableBackground.Enable();
            _equipButton.OnClicked += EquipButton_OnClick;
            _unEquipButton.OnClicked += UnEquipButton_OnClick;
            _levelUpButton.OnClicked += LevelUpButton_OnClick;
        }

        protected override void UnsubscribeButtons()
        {
            _closeButton.OnClicked -= CloseButton_OnClick;
            _clickableBackground.OnClicked -= CloseButton_OnClick;
            _equipButton.OnClicked -= EquipButton_OnClick;
            _unEquipButton.OnClicked -= UnEquipButton_OnClick;
            _levelUpButton.OnClicked -= LevelUpButton_OnClick;
        }

        private void LevelUpButton_OnClick(InteractableButton interactableButton)
        {
            if (interactableButton is not LevelUpPieceButton levelUpPieceButton)
            {
                return;
            }

            _slotLevel++;
            _levelUpButton.OnButtonUp();
            UnsubscribeButtons();
            EnableButtons();
            BaseStatTextAnimation();
            SoundManager.Instance.PlayUpgradeEquipmentPieceSound();
            EquipmentPieceData equipmentPieceData = EquipmentDataManager.Instance.GetEquipmentPieceData(_equipmentPieceInstance.Id);
            UpdateBaseStatText(equipmentPieceData, _slotLevel);
            
            OnLevelUpButtonClicked?.Invoke(levelUpPieceButton.GetEquipmentPieceType());
        }

        private void UpdateBaseStatText(EquipmentPieceData equipmentPieceData, int slotLevel)
        {
            if (equipmentPieceData.EquipmentBaseStatType == EquipmentBaseStatType.Attack)
            {
                _baseStatText.text = $"Attack +{equipmentPieceData.FlatBaseStatAmount(slotLevel).ToString()}";
            }
            else
            {
                _baseStatText.text = $"Max Hp +{equipmentPieceData.FlatBaseStatAmount(slotLevel).ToString()}";
            }
        }

        private void BaseStatTextAnimation()
        {
            TextMeshProUGUI statText = _baseStatText;
            Color highlightColor = Color.green;
            float transitionDuration = 0.2f;
            float holdDuration = 0.2f;
            float fontSizePunch = 4f;
            float originalFontSize = 25f;

            DOTween.Kill(statText, complete: false);

            statText.fontSize = originalFontSize;
            statText.color = Color.white;

            Sequence animationSequence = DOTween.Sequence();

            animationSequence.Append(statText.DOColor(highlightColor, transitionDuration));
            animationSequence.Join(DOTween.To(
                () => statText.fontSize,
                x => statText.fontSize = x,
                originalFontSize + fontSizePunch,
                transitionDuration
            ));
            animationSequence.AppendInterval(holdDuration);
            animationSequence.Append(statText.DOColor(Color.white, transitionDuration));
            animationSequence.Join(DOTween.To(
                () => statText.fontSize,
                x => statText.fontSize = x,
                originalFontSize,
                transitionDuration
            ));

            animationSequence.SetId(statText);
        }

        private void EquipButton_OnClick(InteractableButton interactableButton)
        {
            if (interactableButton is not EquipPieceButton equipPieceButton)
            {
                return;
            }
            
            UnsubscribeButtons();
            
            OnEquipButtonClicked?.Invoke(equipPieceButton.EquipmentPieceInstance());
        }

        private void UnEquipButton_OnClick(InteractableButton interactableButton)
        {
            if (interactableButton is not EquipPieceButton equipPieceButton)
            {
                return;
            }
            
            UnsubscribeButtons();

            OnUnEquipButtonClicked?.Invoke(equipPieceButton.EquipmentPieceInstance());
        }

        private IEnumerator Refresh(EquipmentPieceInstance equipmentPieceInstance)
        {
            _equipmentPieceInstance = equipmentPieceInstance;
            EquipmentPieceData equipmentPieceData = EquipmentDataManager.Instance.GetEquipmentPieceData(equipmentPieceInstance.Id);

            if (_isSlot)
            {
                _unEquipButton.Initialize(equipmentPieceInstance);
                _unEquipButton.gameObject.SetActive(true);
                _equipButton.gameObject.SetActive(false);
                _equipButtonIndicator.SetActive(false);
                _levelUpButton.gameObject.SetActive(true);
                _upgradeCost.SetActive(true);
            }
            else
            {
                _equipButton.Initialize(equipmentPieceInstance);
                _unEquipButton.gameObject.SetActive(false);
                _equipButton.gameObject.SetActive(true);
                _equipButtonIndicator.SetActive(EquipmentDataManager.Instance.IsSlotEmpty(equipmentPieceData.EquipmentPieceType));
                _levelUpButton.gameObject.SetActive(false);
                _upgradeCost.SetActive(false);
            }
            
            _titleText.text = equipmentPieceData.Name;
            _descriptionText.text = equipmentPieceData.Description;

            RefreshRarityTraitDescriptions(equipmentPieceInstance);
            UpdateBaseStatText(equipmentPieceData, _slotLevel);
            
            if (equipmentPieceData.EquipmentBaseStatType == EquipmentBaseStatType.Attack)
            {
                _baseStatText.text = $"Attack +{equipmentPieceData.FlatBaseStatAmount(_slotLevel).ToString()}";
            }
            else
            {
                _baseStatText.text = $"Max Hp +{equipmentPieceData.FlatBaseStatAmount(_slotLevel).ToString()}";
            }

            int currentGold = CurrencyDataManager.Instance.CurrentGold();
            int goldUpgradeCost = _equipmentSlotData.GoldUpgradeCostPerLevel[_slotLevel];
            _upgradeGoldText.text = currentGold < goldUpgradeCost ? $"<color=#FF1F09>{currentGold}</color>/{goldUpgradeCost}" : $"{currentGold}/{goldUpgradeCost}";
            
            int currentMaterials = CurrencyDataManager.Instance.CurrentMaterials(equipmentPieceData.EquipmentPieceType);
            int materialUpgradeCost = _equipmentSlotData.MaterialUpgradeCostPerLevel[_slotLevel];
            _upgradeMaterialsText.text = currentMaterials < materialUpgradeCost ? $"<color=#FF1F09>{currentMaterials}</color>/{materialUpgradeCost}" : $"{currentMaterials}/{materialUpgradeCost}";
            _levelUpButton.Initialize(equipmentPieceData.EquipmentPieceType, currentGold >= goldUpgradeCost, currentMaterials >= materialUpgradeCost);

            yield return new WaitForSeconds(RefreshEquipmentPieceTime);
            _upgradeMaterial.Initialize(equipmentPieceData.EquipmentPieceType);
            _equipmentPieceButton.Initialize(_equipmentPieceInstance, animateClick: false, isDetailPopup: true);
        }
        
        private void RefreshRarityTraitDescriptions(EquipmentPieceInstance equipmentPieceInstance)
        {
            EquipmentPieceData equipmentPieceData = EquipmentDataManager.Instance.GetEquipmentPieceData(equipmentPieceInstance.Id);

            for (int i = 0; i < _rarityTraitDescriptions.Length; i++)
            {
                _rarityTraitDescriptions[i].Initialize(equipmentPieceData.EquipmentTraitsPerRarity[i]);
            }
            
            for (int i = equipmentPieceInstance.Rarity; i < _rarityTraitDescriptions.Length; i++)
            {
                _rarityTraitDescriptions[i].Lock();
            }
        }
    }
}
