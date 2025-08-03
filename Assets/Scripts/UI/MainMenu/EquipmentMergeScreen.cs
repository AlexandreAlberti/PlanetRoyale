using System;
using System.Collections;
using System.Collections.Generic;
using Application.Data.Equipment;
using Application.Sound;
using Application.Utils;
using DG.Tweening;
using Game.Equipment;
using Game.PlayerUnit;
using TMPro;
using UI.Button;
using UnityEngine;

namespace UI.MainMenu
{
    public class EquipmentMergeScreen : Popup
    {
        [SerializeField] private Transform _inventoryContainer;
        [SerializeField] private EquipmentPieceButton _equipmentPieceButtonPrefab;
        [SerializeField] private EquipmentGrid _equipmentGrid;
        [SerializeField] private Transform _newEquipmentPieceDestination;
        [SerializeField] private Transform _selectedEquipmentPieceDestination;
        [SerializeField] private Transform _equipmentPieceCopy1Destination;
        [SerializeField] private Transform _equipmentPieceCopy2Destination;
        [SerializeField] private Animator _mergingItemsHolderAnimator;
        [SerializeField] private Animator _requirementsHolderAnimator;
        [SerializeField] private InteractableButton _mergeButton;
        [SerializeField] private InteractableButton _backButton;
        [SerializeField] private GameObject[] _readyToMergeVisuals;
        [SerializeField] private GameObject[] _notReadyToMergeVisuals;
        [SerializeField] private Animator[] _readyToMergeAnimators;
        [SerializeField] private TextMeshProUGUI _requirementsText;
        [SerializeField] private TextMeshProUGUI _newEquipmentName;
        [SerializeField] private TextMeshProUGUI _newEquipmentBaseStat;
        [SerializeField] private RarityTraitDescription _newEquipmentTrait;
        [SerializeField] private GameObject _newEquipmentFullDescription;
        [SerializeField] private GameObject _selectNewEquipmentText;
        [SerializeField] private MergeAnimationScreen _mergeAnimationScreen;
        [SerializeField] private PlayerBaseStats _playerBaseStats;

        private const float GoToHolderPositionAnimationDuration = 0.5f;
        private const string RequiredPieces = "Required pieces";
        private const string Colon = ":";
        private const string RequiredNumberOfCopies = "2x";
        private const string Arrow = "->";
        
        private static readonly int AppearTrigger = Animator.StringToHash("Appear");
        private static readonly int HideTrigger = Animator.StringToHash("Hide");
        private static readonly int HiddenState = Animator.StringToHash("Hidden");
        
        private EquipmentPieceButton _selectedInitialEquipmentPiece;
        private EquipmentPieceButton _duplicatedInitialEquipmentPieceButton;
        private EquipmentPieceButton _selectedEquipmentPieceCopy1;
        private EquipmentPieceButton _selectedEquipmentPieceCopy2;
        private EquipmentPieceButton _duplicatedEquipmentPieceCopy1Button;
        private EquipmentPieceButton _duplicatedEquipmentPieceCopy2Button;
        private EquipmentPieceButton _grayedOutEquipmentPieceCopy1Button;
        private EquipmentPieceButton _grayedOutEquipmentPieceCopy2Button;
        private EquipmentPieceButton _newEquipmentPieceButton;
        private List<EquipmentPieceButton> _potentialEquipmentPieceCopies;
        private bool _isSelectingCopy1;
        private bool _isSelectingCopy2;
        private static readonly int FadeTrigger = Animator.StringToHash("Fade");
        private static readonly int IdleTrigger = Animator.StringToHash("Idle");

        public Action OnBackButtonPressed;
        public Action OnEquipmentPieceButtonClicked;

        public void Initialize()
        {
            _potentialEquipmentPieceCopies = new List<EquipmentPieceButton>();
            _selectedInitialEquipmentPiece = null;
            _equipmentGrid.Initialize(true);

            if (_duplicatedEquipmentPieceCopy1Button != null)
            {
                _duplicatedEquipmentPieceCopy1Button.OnClicked -= EquipmentPieceCopy1Button_OnClicked;
                Destroy(_duplicatedEquipmentPieceCopy1Button.gameObject);
            }
            
            if (_duplicatedEquipmentPieceCopy2Button != null)
            {
                _duplicatedEquipmentPieceCopy2Button.OnClicked -= EquipmentPieceCopy2Button_OnClicked;
                Destroy(_duplicatedEquipmentPieceCopy2Button.gameObject);
            }
            
            if (_duplicatedInitialEquipmentPieceButton != null)
            {
                _duplicatedInitialEquipmentPieceButton.OnClicked -= InitialEquipmentPieceButton_OnClicked;
                Destroy(_duplicatedInitialEquipmentPieceButton.gameObject);
            }
            
            if (_grayedOutEquipmentPieceCopy1Button != null)
            {
                Destroy(_grayedOutEquipmentPieceCopy1Button.gameObject);
            }

            if (_grayedOutEquipmentPieceCopy2Button != null)
            {
                Destroy(_grayedOutEquipmentPieceCopy2Button.gameObject);
            }

            if (_newEquipmentPieceButton != null)
            {
                Destroy(_newEquipmentPieceButton.gameObject);
            }
            
            _equipmentGrid.OnEquipmentPieceClicked -= EquipmentGrid_OnEquipmentPieceClicked;
            _equipmentGrid.OnEquipmentPieceClicked += EquipmentGrid_OnEquipmentPieceClicked;
            _equipmentGrid.ShowSortButton();
            _mergeButton.gameObject.SetActive(false);
            DisableReadyToMergeVisuals();
            EnableNotReadyToMergeVisuals();
            _newEquipmentFullDescription.SetActive(false);
            _selectNewEquipmentText.SetActive(true);
            _isSelectingCopy1 = false;
            _isSelectingCopy2 = false;
            _selectedInitialEquipmentPiece = null;
            _selectedEquipmentPieceCopy1 = null;
            _selectedEquipmentPieceCopy2 = null;

            if (_mergingItemsHolderAnimator.gameObject.activeInHierarchy)
            {
                _mergingItemsHolderAnimator.Play(HiddenState);
            }
            
            _mergeButton.OnClicked -= MergeButton_OnClicked;
            _mergeButton.OnClicked += MergeButton_OnClicked;
            
            _closeButton.OnClicked -= CloseButton_OnClicked;
            _closeButton.OnClicked += CloseButton_OnClicked;
            _closeButton.Enable();
            
            _requirementsHolderAnimator.SetTrigger(HideTrigger);
            
            StartCoroutine(WaitAndRefresh());
        }

        private void MergeButton_OnClicked(InteractableButton interactableButton)
        {
            _mergeButton.OnClicked -= MergeButton_OnClicked;
            _closeButton.Disable();
            _mergeAnimationScreen.Show();
            _mergeAnimationScreen.Initialize(_selectedInitialEquipmentPiece, _selectedEquipmentPieceCopy1, _selectedEquipmentPieceCopy2);
            
            EquipmentDataManager.Instance.UpgradeEquipmentPieceRarity(_selectedInitialEquipmentPiece.EquipmentPieceInstance());
            EquipmentDataManager.Instance.RemoveEquipmentPiece(_selectedEquipmentPieceCopy1.EquipmentPieceInstance());
            EquipmentDataManager.Instance.RemoveEquipmentPiece(_selectedEquipmentPieceCopy2.EquipmentPieceInstance());
            
            SoundManager.Instance.PlayMenuPlayGameButtonSound();
            
            StartCoroutine(WaitAndInitialize());
        }

        private IEnumerator WaitAndInitialize()
        {
            yield return new WaitForSeconds(NumberConstants.Half);
            Initialize();
        }

        private void EquipmentGrid_OnEquipmentPieceClicked(InteractableButton interactableButton)
        {
            if (interactableButton is not EquipmentPieceButton equipmentPieceButton)
            {
                return;
            }
            
            if (equipmentPieceButton == _selectedInitialEquipmentPiece)
            {
                InitialEquipmentPieceButton_OnClicked(equipmentPieceButton);
                return;
            }
            
            if (equipmentPieceButton == _selectedEquipmentPieceCopy1)
            {
                EquipmentPieceCopy1Button_OnClicked(equipmentPieceButton);
                return;
            }
            
            if (equipmentPieceButton == _selectedEquipmentPieceCopy2)
            {
                EquipmentPieceCopy2Button_OnClicked(equipmentPieceButton);
                return;
            }
            
            if (_isSelectingCopy2)
            {
                _duplicatedEquipmentPieceCopy1Button.OnClicked -= EquipmentPieceCopy1Button_OnClicked;
                _duplicatedInitialEquipmentPieceButton.OnClicked -= InitialEquipmentPieceButton_OnClicked;
                _selectedEquipmentPieceCopy2 = equipmentPieceButton;
                EquipmentPieceButton duplicatedEquipmentPieceButton = Instantiate(_equipmentPieceButtonPrefab, equipmentPieceButton.transform.position, Quaternion.identity, _inventoryContainer);
                duplicatedEquipmentPieceButton.Initialize(equipmentPieceButton.EquipmentPieceInstance());
                duplicatedEquipmentPieceButton.transform.SetParent(transform);
                duplicatedEquipmentPieceButton.transform.DOMove(_equipmentPieceCopy2Destination.position, GoToHolderPositionAnimationDuration).onComplete += OnCopy2DestinationReached;
                duplicatedEquipmentPieceButton.Disable();
                _duplicatedEquipmentPieceCopy2Button = duplicatedEquipmentPieceButton;
                LockItems(equipmentPieceButton);
            }
            else if (_isSelectingCopy1)
            {
                _duplicatedInitialEquipmentPieceButton.OnClicked -= InitialEquipmentPieceButton_OnClicked;
                _selectedEquipmentPieceCopy1 = equipmentPieceButton;
                EquipmentPieceButton duplicatedEquipmentPieceButton = Instantiate(_equipmentPieceButtonPrefab, equipmentPieceButton.transform.position, Quaternion.identity, _inventoryContainer);
                duplicatedEquipmentPieceButton.Initialize(equipmentPieceButton.EquipmentPieceInstance());
                duplicatedEquipmentPieceButton.transform.SetParent(transform);
                duplicatedEquipmentPieceButton.transform.DOMove(_equipmentPieceCopy1Destination.position, GoToHolderPositionAnimationDuration).onComplete += OnCopy1DestinationReached;
                duplicatedEquipmentPieceButton.Disable();
                _duplicatedEquipmentPieceCopy1Button = duplicatedEquipmentPieceButton;
                LockItems(equipmentPieceButton);
            }
            else
            {
                _equipmentGrid.HideSortButton();
                _mergingItemsHolderAnimator.SetTrigger(AppearTrigger);
                _selectedInitialEquipmentPiece = equipmentPieceButton;
                EquipmentPieceButton duplicatedEquipmentPieceButton = Instantiate(_equipmentPieceButtonPrefab, equipmentPieceButton.transform.position, Quaternion.identity, _inventoryContainer);
                duplicatedEquipmentPieceButton.Initialize(equipmentPieceButton.EquipmentPieceInstance(), showEquippedBanner: equipmentPieceButton.EquipmentPieceInstance().IsEquipped);
                duplicatedEquipmentPieceButton.transform.SetParent(transform);
                duplicatedEquipmentPieceButton.transform.DOMove(_selectedEquipmentPieceDestination.position, GoToHolderPositionAnimationDuration).onComplete += OnMergeItemDestinationReached;
                duplicatedEquipmentPieceButton.Disable();
                _duplicatedInitialEquipmentPieceButton = duplicatedEquipmentPieceButton;
                LockItems(equipmentPieceButton);
            }

            _equipmentGrid.OnEquipmentPieceClicked -= EquipmentGrid_OnEquipmentPieceClicked;
            
            OnEquipmentPieceButtonClicked?.Invoke();
        }
        
        private void LockItems(EquipmentPieceButton equipmentPieceButton)
        {
            foreach (EquipmentPieceButton inventoryEquipmentPiece in _equipmentGrid.GetItems())
            {
                if (inventoryEquipmentPiece.EquipmentPieceInstance().Id != equipmentPieceButton.EquipmentPieceInstance().Id
                    || inventoryEquipmentPiece.EquipmentPieceInstance().Rarity != equipmentPieceButton.EquipmentPieceInstance().Rarity)
                {
                    inventoryEquipmentPiece.Lock();
                }
                else
                {
                    _potentialEquipmentPieceCopies.Add(inventoryEquipmentPiece);
                }
                
                inventoryEquipmentPiece.Disable();
            }
        }
        
        private void UnlockItems()
        {
            foreach (EquipmentPieceButton inventoryEquipmentPiece in _equipmentGrid.GetItems())
            {
                inventoryEquipmentPiece.Enable();
                inventoryEquipmentPiece.Restore();
            }
        }

        private void OnMergeItemDestinationReached()
        {
            _equipmentGrid.OnEquipmentPieceClicked += EquipmentGrid_OnEquipmentPieceClicked;
            _duplicatedInitialEquipmentPieceButton.OnClicked += InitialEquipmentPieceButton_OnClicked;
            _duplicatedInitialEquipmentPieceButton.Enable();
            _duplicatedInitialEquipmentPieceButton.DisableEquippedBanner();
            _selectedInitialEquipmentPiece.MarkAsSelected();
            _requirementsHolderAnimator.SetTrigger(AppearTrigger);
            _isSelectingCopy1 = true;
            
            EquipmentPieceButton duplicatedEquipmentPieceButtonCopy1 = Instantiate(_equipmentPieceButtonPrefab, _equipmentPieceCopy1Destination.transform.position, Quaternion.identity, _inventoryContainer);
            duplicatedEquipmentPieceButtonCopy1.Initialize(_selectedInitialEquipmentPiece.EquipmentPieceInstance());
            duplicatedEquipmentPieceButtonCopy1.transform.SetParent(transform);
            duplicatedEquipmentPieceButtonCopy1.EnableGrayLayer();
            duplicatedEquipmentPieceButtonCopy1.Disable();
            _grayedOutEquipmentPieceCopy1Button = duplicatedEquipmentPieceButtonCopy1;
            
            EquipmentPieceButton duplicatedEquipmentPieceButtonCopy2 = Instantiate(_equipmentPieceButtonPrefab, _equipmentPieceCopy2Destination.transform.position, Quaternion.identity, _inventoryContainer);
            duplicatedEquipmentPieceButtonCopy2.Initialize(_selectedInitialEquipmentPiece.EquipmentPieceInstance());
            duplicatedEquipmentPieceButtonCopy2.transform.SetParent(transform);
            duplicatedEquipmentPieceButtonCopy2.EnableGrayLayer();
            duplicatedEquipmentPieceButtonCopy2.Disable();
            _grayedOutEquipmentPieceCopy2Button = duplicatedEquipmentPieceButtonCopy2;
            
            EquipmentPieceButton newEquipmentPieceButton = Instantiate(_equipmentPieceButtonPrefab, _newEquipmentPieceDestination.transform.position, Quaternion.identity, _inventoryContainer);
            EquipmentPieceInstance newEquipmentPieceInstance = 
                EquipmentPieceFactory.GenerateEquipmentPieceById(
                    _selectedInitialEquipmentPiece.Id(),
                    _selectedInitialEquipmentPiece.Rarity());
            newEquipmentPieceInstance.UpgradeRarity();
            newEquipmentPieceButton.Initialize(newEquipmentPieceInstance);
            newEquipmentPieceButton.Disable();
            newEquipmentPieceButton.transform.SetParent(_newEquipmentPieceDestination);
            newEquipmentPieceButton.transform.localScale = Vector3.one;
            _newEquipmentPieceButton = newEquipmentPieceButton;

            EquipmentPieceRarity equipmentPieceRarity = (EquipmentPieceRarity) _selectedInitialEquipmentPiece.Rarity();
            string rarityColor = ColorUtility.ToHtmlStringRGB(RarityColor.Instance.GetColorByRarity(equipmentPieceRarity));
            string equipmentPieceName = _selectedInitialEquipmentPiece.Name();
            _requirementsText.text = $"{RequiredPieces}{Colon} {RequiredNumberOfCopies} <color=#{rarityColor}>{equipmentPieceRarity}</color> {equipmentPieceName}";
            _newEquipmentFullDescription.SetActive(true);
            _selectNewEquipmentText.SetActive(false);
            
            EquipmentPieceRarity newEquipmentPieceRarity = (EquipmentPieceRarity) newEquipmentPieceButton.Rarity();
            string newRarityColor = ColorUtility.ToHtmlStringRGB(RarityColor.Instance.GetColorByRarity(newEquipmentPieceRarity));
            _newEquipmentName.text = $"<color=#{newRarityColor}>{equipmentPieceName}</color>";
            string greenColor = ColorUtility.ToHtmlStringRGB(Color.green);

            string baseStatName = _selectedInitialEquipmentPiece.BaseStatName();
            int previousBaseStat = _selectedInitialEquipmentPiece.BaseStatAmount();
            int newBaseStat = newEquipmentPieceButton.BaseStatAmount();
            _newEquipmentBaseStat.text = $"{baseStatName}{Colon} {previousBaseStat} {Arrow} <color=#{greenColor}>{newBaseStat}</color>";
            
            _newEquipmentTrait.Initialize(_newEquipmentPieceButton.RarityTrait());
            
            foreach (EquipmentPieceButton potentialEquipmentPieceCopy in _potentialEquipmentPieceCopies)
            {
                potentialEquipmentPieceCopy.Enable();
            }
        }

        private void OnCopy1DestinationReached()
        {
            _equipmentGrid.OnEquipmentPieceClicked += EquipmentGrid_OnEquipmentPieceClicked;
            _duplicatedInitialEquipmentPieceButton.OnClicked += InitialEquipmentPieceButton_OnClicked;
            _duplicatedEquipmentPieceCopy1Button.OnClicked += EquipmentPieceCopy1Button_OnClicked;
            _duplicatedEquipmentPieceCopy1Button.Enable();
            _selectedEquipmentPieceCopy1.MarkAsSelected();
            _isSelectingCopy2 = true;
            
            bool isMergeReady = _selectedEquipmentPieceCopy1 != null && _selectedEquipmentPieceCopy2 != null;
            
            if (isMergeReady)
            {
                _mergeButton.gameObject.SetActive(true);
                _mergeButton.Appear();
                EnableReadyToMergeVisuals();
                DisableNotReadyToMergeVisuals();
                
                foreach (EquipmentPieceButton potentialEquipmentPieceCopy in _potentialEquipmentPieceCopies)
                {
                    if (potentialEquipmentPieceCopy == _selectedInitialEquipmentPiece
                        || potentialEquipmentPieceCopy == _selectedEquipmentPieceCopy1
                        || potentialEquipmentPieceCopy == _selectedEquipmentPieceCopy2)
                    {
                        potentialEquipmentPieceCopy.Enable();
                    }
                    else
                    {
                        potentialEquipmentPieceCopy.Disable();
                        potentialEquipmentPieceCopy.Lock();
                    }
                }
            }
            else
            {
                foreach (EquipmentPieceButton potentialEquipmentPieceCopy in _potentialEquipmentPieceCopies)
                {
                    potentialEquipmentPieceCopy.Enable();
                }
            }
        }
        
        private void OnCopy2DestinationReached()
        {
            _equipmentGrid.OnEquipmentPieceClicked += EquipmentGrid_OnEquipmentPieceClicked;
            _duplicatedInitialEquipmentPieceButton.OnClicked += InitialEquipmentPieceButton_OnClicked;
            _duplicatedEquipmentPieceCopy1Button.OnClicked += EquipmentPieceCopy1Button_OnClicked;
            _duplicatedEquipmentPieceCopy2Button.OnClicked += EquipmentPieceCopy2Button_OnClicked;
            _duplicatedEquipmentPieceCopy2Button.Enable();
            _selectedEquipmentPieceCopy2.MarkAsSelected();

            bool isMergeReady = _selectedEquipmentPieceCopy1 != null && _selectedEquipmentPieceCopy2 != null;

            if (isMergeReady)
            {
                _mergeButton.gameObject.SetActive(true);
                _mergeButton.Appear();
                EnableReadyToMergeVisuals();
                DisableNotReadyToMergeVisuals();
                
                foreach (EquipmentPieceButton potentialEquipmentPieceCopy in _potentialEquipmentPieceCopies)
                {
                    if (potentialEquipmentPieceCopy == _selectedInitialEquipmentPiece
                        || potentialEquipmentPieceCopy == _selectedEquipmentPieceCopy1
                        || potentialEquipmentPieceCopy == _selectedEquipmentPieceCopy2)
                    {
                        potentialEquipmentPieceCopy.Enable();
                        potentialEquipmentPieceCopy.MarkAsSelected();
                    }
                    else
                    {
                        potentialEquipmentPieceCopy.Disable();
                        potentialEquipmentPieceCopy.Lock();
                    }
                }
            }
            else
            {
                foreach (EquipmentPieceButton potentialEquipmentPieceCopy in _potentialEquipmentPieceCopies)
                {
                    potentialEquipmentPieceCopy.Enable();
                }
            }
        }

        private void InitialEquipmentPieceButton_OnClicked(InteractableButton interactableButton)
        {
            _mergeButton.gameObject.SetActive(false);
            DisableReadyToMergeVisuals();
            EnableNotReadyToMergeVisuals();
            _newEquipmentFullDescription.SetActive(false);
            _selectNewEquipmentText.SetActive(true);
            _isSelectingCopy1 = false;
            _isSelectingCopy2 = false;
            _selectedInitialEquipmentPiece = null;
            _selectedEquipmentPieceCopy1 = null;
            _selectedEquipmentPieceCopy2 = null;
            _mergingItemsHolderAnimator.Play(HiddenState);
            _requirementsHolderAnimator.SetTrigger(HideTrigger);
            UnlockItems();

            if (_duplicatedEquipmentPieceCopy1Button != null)
            {
                _duplicatedEquipmentPieceCopy1Button.OnClicked -= EquipmentPieceCopy1Button_OnClicked;
                Destroy(_duplicatedEquipmentPieceCopy1Button.gameObject);
            }
            
            if (_duplicatedEquipmentPieceCopy2Button != null)
            {
                _duplicatedEquipmentPieceCopy2Button.OnClicked -= EquipmentPieceCopy2Button_OnClicked;
                Destroy(_duplicatedEquipmentPieceCopy2Button.gameObject);
            }
            
            if (_duplicatedInitialEquipmentPieceButton != null)
            {
                _duplicatedInitialEquipmentPieceButton.OnClicked -= InitialEquipmentPieceButton_OnClicked;
                Destroy(_duplicatedInitialEquipmentPieceButton.gameObject);
            }

            if (_grayedOutEquipmentPieceCopy1Button != null)
            {
                Destroy(_grayedOutEquipmentPieceCopy1Button.gameObject);
            }

            if (_grayedOutEquipmentPieceCopy2Button != null)
            {
                Destroy(_grayedOutEquipmentPieceCopy2Button.gameObject);
            }

            if (_newEquipmentPieceButton != null)
            {
                Destroy(_newEquipmentPieceButton.gameObject);
            }
            
            _equipmentGrid.ShowSortButton();
            _potentialEquipmentPieceCopies.Clear();
        }
        
        private void EquipmentPieceCopy1Button_OnClicked(InteractableButton interactableButton)
        {
            _mergeButton.gameObject.SetActive(false);
            DisableReadyToMergeVisuals();
            EnableNotReadyToMergeVisuals();
            _isSelectingCopy2 = false;
            _selectedEquipmentPieceCopy1.Restore();
            _selectedEquipmentPieceCopy1 = null;
            _duplicatedEquipmentPieceCopy1Button.OnClicked -= EquipmentPieceCopy1Button_OnClicked;
            
            if (_duplicatedEquipmentPieceCopy1Button != null)
            {
                Destroy(_duplicatedEquipmentPieceCopy1Button.gameObject);
            }
            
            foreach (EquipmentPieceButton potentialEquipmentPieceCopy in _potentialEquipmentPieceCopies)
            {
                if (potentialEquipmentPieceCopy == _selectedInitialEquipmentPiece
                    || potentialEquipmentPieceCopy == _selectedEquipmentPieceCopy1
                    || potentialEquipmentPieceCopy == _selectedEquipmentPieceCopy2)
                {
                    potentialEquipmentPieceCopy.Enable();
                    potentialEquipmentPieceCopy.MarkAsSelected();
                }
                else
                {
                    potentialEquipmentPieceCopy.Enable();
                    potentialEquipmentPieceCopy.Restore();
                }
            }
        }
        
        private void EquipmentPieceCopy2Button_OnClicked(InteractableButton interactableButton)
        {
            _mergeButton.gameObject.SetActive(false);
            DisableReadyToMergeVisuals();
            EnableNotReadyToMergeVisuals();
            _selectedEquipmentPieceCopy2.Restore();
            _selectedEquipmentPieceCopy2 = null;
            _duplicatedEquipmentPieceCopy2Button.OnClicked -= EquipmentPieceCopy2Button_OnClicked;
            
            if (_duplicatedEquipmentPieceCopy2Button != null)
            {
                Destroy(_duplicatedEquipmentPieceCopy2Button.gameObject);
            }
            
            foreach (EquipmentPieceButton potentialEquipmentPieceCopy in _potentialEquipmentPieceCopies)
            {
                if (potentialEquipmentPieceCopy == _selectedInitialEquipmentPiece
                    || potentialEquipmentPieceCopy == _selectedEquipmentPieceCopy1
                    || potentialEquipmentPieceCopy == _selectedEquipmentPieceCopy2)
                {
                    potentialEquipmentPieceCopy.Enable();
                    potentialEquipmentPieceCopy.MarkAsSelected();
                }
                else
                {
                    potentialEquipmentPieceCopy.Enable();
                    potentialEquipmentPieceCopy.Restore();
                }
            }
        }

        private IEnumerator WaitAndRefresh()
        {
            yield return new WaitForEndOfFrame();
            Refresh();
        }

        private void Refresh()
        {
            _equipmentGrid.Refresh();
        }

        private void EnableNotReadyToMergeVisuals()
        {
            foreach (GameObject visual in _notReadyToMergeVisuals)
            {
                visual.SetActive(true);
            }
        }
       
        private void DisableNotReadyToMergeVisuals()
        {
            foreach (GameObject visual in _notReadyToMergeVisuals)
            {
                visual.SetActive(false);
            }
        }
        
        private void EnableReadyToMergeVisuals()
        {
            foreach (GameObject visual in _readyToMergeVisuals)
            {
                visual.SetActive(true);
            }

            foreach (Animator readyToMergeAnimator in _readyToMergeAnimators)
            {
                readyToMergeAnimator.SetTrigger(FadeTrigger);
            }
        }
        
        private void DisableReadyToMergeVisuals()
        {
            foreach (GameObject visual in _readyToMergeVisuals)
            {
                visual.SetActive(false);
            }
            
            foreach (Animator readyToMergeAnimator in _readyToMergeAnimators)
            {
                if (!readyToMergeAnimator.gameObject.activeInHierarchy)
                {
                    return;
                }
                
                readyToMergeAnimator.SetTrigger(IdleTrigger);
            }
        }
        
        private void CloseButton_OnClicked(InteractableButton interactableButton)
        {
            OnBackButtonPressed?.Invoke();
        }

        public InteractableButton FirstMergePiece()
        {
            return _equipmentGrid.SecondEquipmentPiece();
        }

        public InteractableButton SecondMergePiece()
        {
            return _equipmentGrid.ThirdEquipmentPiece();
        }

        public InteractableButton ThirdMergePiece()
        {
            return _equipmentGrid.FourthEquipmentPiece();
        }
    }
}
