using System.Collections;
using Application.Data.Equipment;
using Application.Sound;
using Application.Utils;
using DG.Tweening;
using Game.Equipment;
using TMPro;
using UI.Button;
using UnityEngine;

namespace UI.MainMenu
{
    public class MergeAnimationScreen : Popup
    {
        [SerializeField] private EquipmentPieceButton _equipmentPieceButtonPrefab;
        [SerializeField] private Transform _oldItemPosition;
        [SerializeField] private Transform _newItemPosition;
        [SerializeField] private Transform _copy1ItemPosition;
        [SerializeField] private Transform _copy2ItemPosition;
        [SerializeField] private Animator _copy1Animator;
        [SerializeField] private Animator _copy2Animator;
        [SerializeField] private Animator _copy1HolderAnimator;
        [SerializeField] private Animator _copy2HolderAnimator;
        [SerializeField] private Transform _descriptionArea;
        [SerializeField] private TextMeshProUGUI _newEquipmentName;
        [SerializeField] private TextMeshProUGUI _newEquipmentBaseStat;
        [SerializeField] private RarityTraitDescription _newEquipmentTrait;
        [SerializeField] private Animator _fullDescriptionAnimator;
        [SerializeField] private GameObject _vfxRays;
        
        private static readonly int IdleTrigger = Animator.StringToHash("Idle");
        private static readonly int ShowTrigger = Animator.StringToHash("Show");
        private static readonly int HideTrigger = Animator.StringToHash("Hide");
        private static readonly int ShakeTrigger = Animator.StringToHash("Shake");
        private const float GoToMergePositionAnimationDuration = 0.5f;
        private const float VfxAppearTime = 0.2f;
        private const float DescriptionAppearTime = 0.5f;
        private const string Colon = ":";
        private const string Arrow = "->";
        
        private EquipmentPieceButton _oldEquipmentPiece;
        private EquipmentPieceButton _copy1EquipmentPiece;
        private EquipmentPieceButton _copy2EquipmentPiece;
        private EquipmentPieceButton _newEquipmentPiece;
        private Vector3 _originalDescriptionAreaPosition;

        public void Initialize(EquipmentPieceButton equipmentPieceButton, EquipmentPieceButton copy1EquipmentPieceButton, EquipmentPieceButton copy2EquipmentPieceButton)
        {
            _vfxRays.SetActive(false);
            _clickableBackground.gameObject.SetActive(true); 
            UnsubscribeButtons();
            
            EquipmentPieceButton oldEquipmentPiece = Instantiate(_equipmentPieceButtonPrefab, _oldItemPosition.position, Quaternion.identity, _oldItemPosition);
            oldEquipmentPiece.Initialize(equipmentPieceButton.EquipmentPieceInstance());
            oldEquipmentPiece.DisableEquippedBanner();
            _oldEquipmentPiece = oldEquipmentPiece;
            
            EquipmentPieceButton copy1EquipmentPiece = Instantiate(_equipmentPieceButtonPrefab, _copy1ItemPosition.position, Quaternion.identity, _copy1ItemPosition);
            copy1EquipmentPiece.Initialize(copy1EquipmentPieceButton.EquipmentPieceInstance());
            _copy1EquipmentPiece = copy1EquipmentPiece;
            
            EquipmentPieceButton copy2EquipmentPiece = Instantiate(_equipmentPieceButtonPrefab, _copy2ItemPosition.position, Quaternion.identity, _copy2ItemPosition);
            copy2EquipmentPiece.Initialize(copy2EquipmentPieceButton.EquipmentPieceInstance());
            _copy2EquipmentPiece = copy2EquipmentPiece;
            
            EquipmentPieceButton newEquipmentPiece = Instantiate(_equipmentPieceButtonPrefab, _newItemPosition.position, Quaternion.identity, _newItemPosition);
            EquipmentPieceInstance newEquipmentPieceInstance = 
                EquipmentPieceFactory.GenerateEquipmentPieceById(
                    equipmentPieceButton.Id(),
                    equipmentPieceButton.Rarity());
            newEquipmentPieceInstance.UpgradeRarity();
            newEquipmentPiece.Initialize(newEquipmentPieceInstance);
            newEquipmentPiece.gameObject.SetActive(false);
            _newEquipmentPiece = newEquipmentPiece;
            
            string equipmentPieceName = newEquipmentPiece.Name();
            EquipmentPieceRarity newEquipmentPieceRarity = (EquipmentPieceRarity) newEquipmentPiece.Rarity();
            string newRarityColor = ColorUtility.ToHtmlStringRGB(RarityColor.Instance.GetColorByRarity(newEquipmentPieceRarity));
            _newEquipmentName.text = $"<color=#{newRarityColor}>{equipmentPieceName}</color>";
            string greenColor = ColorUtility.ToHtmlStringRGB(Color.green);

            string baseStatName = oldEquipmentPiece.BaseStatName();
            int previousBaseStat = oldEquipmentPiece.BaseStatAmount();
            int newBaseStat = newEquipmentPiece.BaseStatAmount();
            _newEquipmentBaseStat.text = $"{baseStatName}{Colon} {previousBaseStat} {Arrow} <color=#{greenColor}>{newBaseStat}</color>";
            
            _newEquipmentTrait.Initialize(newEquipmentPiece.RarityTrait());

            if (_originalDescriptionAreaPosition == Vector3.zero)
            {
                _originalDescriptionAreaPosition = _descriptionArea.position;
            }

            _descriptionArea.position = _originalDescriptionAreaPosition;
            
            StartCoroutine(MergeAnimation());
        }

        private void OnMergeItemDestinationReached()
        {
            _oldEquipmentPiece.gameObject.SetActive(false);
            _copy1EquipmentPiece.gameObject.SetActive(false);
            _copy2EquipmentPiece.gameObject.SetActive(false);
            _newEquipmentPiece.gameObject.SetActive(true);
            _newEquipmentPiece.Appear();
        }

        private IEnumerator MergeAnimation()
        {
            yield return new WaitForEndOfFrame();
            _oldEquipmentPiece.Disable();
            yield return new WaitForEndOfFrame();
            _copy1HolderAnimator.SetTrigger(ShowTrigger);
            _copy2HolderAnimator.SetTrigger(ShowTrigger);
            yield return new WaitForEndOfFrame();
            _copy1EquipmentPiece.Disable();
            _copy2EquipmentPiece.Disable();
            yield return new WaitForSeconds(NumberConstants.Half);
            _copy1Animator.SetTrigger(ShakeTrigger);
            _copy2Animator.SetTrigger(ShakeTrigger);
            yield return new WaitForSeconds(1.0f);
            _copy1Animator.SetTrigger(IdleTrigger);
            _copy2Animator.SetTrigger(IdleTrigger);
            _copy1EquipmentPiece.transform.DOMove(_newItemPosition.position, GoToMergePositionAnimationDuration).onComplete += OnMergeItemDestinationReached;
            _copy2EquipmentPiece.transform.DOMove(_newItemPosition.position, GoToMergePositionAnimationDuration);
            yield return new WaitForSeconds(GoToMergePositionAnimationDuration);
            _copy1HolderAnimator.SetTrigger(HideTrigger);
            _copy2HolderAnimator.SetTrigger(HideTrigger);
            _fullDescriptionAnimator.SetTrigger(ShowTrigger);
            _descriptionArea.DOMove(_descriptionArea.position + new Vector3(0.0f, 400.0f), DescriptionAppearTime);
            yield return new WaitForSeconds(VfxAppearTime);
            _vfxRays.SetActive(true);
            SoundManager.Instance.PlayUpgradeEquipmentPieceSound();
            yield return new WaitForSeconds(DescriptionAppearTime);
            _clickableBackground.OnClicked += ClickableBackground_OnClicked;
        }

        private void ClickableBackground_OnClicked(InteractableButton button)
        {
            _clickableBackground.OnClicked -= ClickableBackground_OnClicked;
            _clickableBackground.gameObject.SetActive(false);
            Hide();
        }
    }
}
