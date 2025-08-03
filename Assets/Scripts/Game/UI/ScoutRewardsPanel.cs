using System;
using System.Collections;
using System.Collections.Generic;
using Application.Data.Equipment;
using Application.Sound;
using Game.Equipment;
using Game.Reward;
using UI.Button;
using UI.MainMenu;
using UnityEngine;

namespace Game.UI
{
    public class ScoutRewardsPanel : Popup
    {
        [SerializeField] private GameObject _rewardsGameObject;
        [SerializeField] private Transform _rewardsContainer;
        [SerializeField] private EquipmentPieceButton _equipmentPieceButtonPrefab;
        [SerializeField] private EquipmentMaterial _equipmentMaterialPrefab;
        [SerializeField] private RewardCoinsHolder _rewardedCoins;
        
        private static readonly int Hidden = Animator.StringToHash("Hidden");
        private static readonly int Appear = Animator.StringToHash("Appear");
        
        private const float RewardAnimationDelay = 0.15f;

        private int _totalGold;

        private Dictionary<EquipmentPieceType, int> _equipmentMaterialsRewards;
        private List<EquipmentPieceInstance> _equipmentPiecesRewards;
        private List<Animator> _rewardAnimators;
        
        public void Initialize(float scoutTime)
        {
            bool isFirstReward = true;
            foreach (Transform reward in _rewardsContainer)
            {
                if (isFirstReward)
                {
                    isFirstReward = false;
                    continue;
                }
                
                Destroy(reward.gameObject);
            }
            
            _rewardAnimators = new List<Animator>();
            _equipmentMaterialsRewards = new Dictionary<EquipmentPieceType, int>();
            _equipmentPiecesRewards = new List<EquipmentPieceInstance>();
            _totalGold = 0;

            ScoutRewardsManager.Instance.GenerateRewards(scoutTime);
            _totalGold = ScoutRewardsManager.Instance.GetGold();
            _equipmentMaterialsRewards = ScoutRewardsManager.Instance.GetEquipmentMaterials();
            _equipmentPiecesRewards = ScoutRewardsManager.Instance.GetEquipmentPieces();
            
            bool showRewardedGold = _totalGold > 0;
            _rewardedCoins.gameObject.SetActive(showRewardedGold);
            _rewardedCoins.Initialize(_totalGold);

            if (showRewardedGold)
            {
                _rewardAnimators.Add(_rewardedCoins.GetComponent<Animator>());
            }
        }

        public override void Show()
        {
            UnsubscribeButtons();
            
            foreach (EquipmentPieceInstance equipmentPieceInstance in _equipmentPiecesRewards)
            {
                EquipmentPieceButton equipmentPieceButton = Instantiate(_equipmentPieceButtonPrefab, _rewardsContainer);
                equipmentPieceButton.Initialize(equipmentPieceInstance, animateClick: false, isReward: true);
                equipmentPieceButton.DisableSound();
                equipmentPieceButton.Disable();
                _rewardAnimators.Add(equipmentPieceButton.GetComponent<Animator>());
            }

            foreach (KeyValuePair<EquipmentPieceType, int> equipmentPieceType in _equipmentMaterialsRewards)
            {
                EquipmentMaterial equipmentMaterial = Instantiate(_equipmentMaterialPrefab, _rewardsContainer);
                equipmentMaterial.Initialize(equipmentPieceType.Key, isReward: true);
                equipmentMaterial.UpdateAmount(equipmentPieceType.Value);
                _rewardAnimators.Add(equipmentMaterial.GetComponent<Animator>());
            }

            _rewardsGameObject.SetActive(_rewardAnimators.Count > 0);
            gameObject.SetActive(true);
            _animator.SetTrigger(ShowTrigger);

            StartCoroutine(ShowRewards());
        }

        private IEnumerator ShowRewards()
        {
            yield return new WaitForEndOfFrame();
            foreach (Animator rewardAnimator in _rewardAnimators)
            {
                SoundManager.Instance.PlayRewardSound();
                yield return new WaitForSeconds(RewardAnimationDelay);
                rewardAnimator.SetTrigger(Appear);
            }

            EnableButtons();
        }

        protected override void CloseButton_OnClick(InteractableButton button)
        {
            _closeButton.Disable();
            _clickableBackground.Disable();
            UnsubscribeButtons();
            Hide();
        }
        
        public override void Hide()
        {
            _animator.Play("Hidden");
            
            OnClosed?.Invoke();
        }
    }
}