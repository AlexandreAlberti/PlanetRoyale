using System;
using System.Collections;
using System.Collections.Generic;
using Application.Sound;
using Game.Ranking;
using Game.Reward;
using UI.Button;
using UI.MainMenu;
using UnityEngine;

namespace Game.UI
{
    public class GoldDungeonRewardsPanel : Popup
    {
        [SerializeField] private GameObject _rewardsGameObject;
        [SerializeField] private Transform _rewardsContainer;
        [SerializeField] private RewardCoinsHolder _rewardedCoins;
        [SerializeField] private GameObject _winParticles;

        private static readonly int Hidden = Animator.StringToHash("Hidden");
        private static readonly int Appear = Animator.StringToHash("Appear");

        private const float RewardAnimationDelay = 0.15f;
        private const int FirstRank = 0;

        private bool _isPlayerWinner;
        private int _totalGold;
        private List<Animator> _rewardAnimators;
        public Action OnAcceptedRewards;
        
        public void Initialize(bool isHumanPlayerDead)
        {
            int humanPlayerRank = RankingManager.Instance.GetHumanPlayerRank();
            _winParticles.SetActive(humanPlayerRank == FirstRank);
            _rewardAnimators = new List<Animator>();
            _totalGold = RewardManager.Instance.GetTotalGold();
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
            _rewardsGameObject.SetActive(_rewardAnimators.Count > 0);
            gameObject.SetActive(true);
            base.Show();

            StartCoroutine(ShowRewards());
        }

        private IEnumerator ShowRewards()
        {
            foreach (Animator rewardAnimator in _rewardAnimators)
            {
                SoundManager.Instance.PlayRewardSound();
                yield return new WaitForSeconds(RewardAnimationDelay);
                rewardAnimator.SetTrigger(Appear);
            }

            yield return null;
        }

        protected override void CloseButton_OnClick(InteractableButton button)
        {
            _closeButton.Disable();
            _clickableBackground.Disable();
            UnsubscribeButtons();

            OnAcceptedRewards?.Invoke();
        }
    }
}