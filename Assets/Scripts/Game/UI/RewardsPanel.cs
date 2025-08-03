using System;
using System.Collections;
using System.Collections.Generic;
using Application.Data.Equipment;
using Application.Sound;
using DG.Tweening;
using Game.Equipment;
using Game.PlayerUnit;
using Game.Ranking;
using Game.Reward;
using TMPro;
using UI.Button;
using UI.MainMenu;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class RewardsPanel : Popup
    {
        [SerializeField] private TextMeshProUGUI _rankText;
        [SerializeField] private TextMeshProUGUI _trophiesText;
        [SerializeField] private GameObject _rewardsGameObject;
        [SerializeField] private Transform _rewardsContainer;
        [SerializeField] private EquipmentPieceButton _equipmentPieceButtonPrefab;
        [SerializeField] private EquipmentMaterial _equipmentMaterialPrefab;
        [SerializeField] private RewardCoinsHolder _rewardedCoins;
        [SerializeField] private GameObject _winParticles;
        [SerializeField] private RankRow[] _rankRows;
        [SerializeField] private GameObject _trophiesContainer;

        private static readonly int Hidden = Animator.StringToHash("Hidden");
        private static readonly int Appear = Animator.StringToHash("Appear");

        private const string Rank = "Rank";
        private const float RewardAnimationDelay = 0.15f;
        private const float LoadDuration = 0.5f;
        private const int FirstRank = 0;

        private bool _isPlayerWinner;

        private int _totalGold;

        private Dictionary<EquipmentPieceType, int> _equipmentMaterialsRewards;
        private List<EquipmentPieceInstance> _equipmentPiecesRewards;
        private List<Animator> _rewardAnimators;

        public Action OnAcceptedRewards;
        
        public void Initialize(bool isHumanPlayerDead, bool isMatchTutorial)
        {
            int humanPlayerRank = RankingManager.Instance.GetHumanPlayerRank();
            _winParticles.SetActive(humanPlayerRank == FirstRank);
            _rankText.text = $"{Rank} {humanPlayerRank + 1}";

            foreach (Player player in PlayerManager.Instance.GetPlayersList())
            {
                int playerRank = RankingManager.Instance.GetRankByPlayerIndex(player.GetPlayerIndex());

                _rankRows[playerRank].SetPlayerName(player.GetName());
                _rankRows[playerRank].SetPlayerIcon(player.GetIcon());
                _rankRows[playerRank].SetPlayerScore(player.IsAlive() ? RankingManager.Instance.GetPlayerScore(player.GetPlayerIndex()) : -1);

                if (player.GetPlayerIndex() == Player.GetHumanPlayerIndex())
                {
                    if (isHumanPlayerDead)
                    {
                        _rankRows[playerRank].SetHumanPlayerDeadBackground();
                    }
                    else
                    {
                        _rankRows[playerRank].SetHumanPlayerBackground();
                    }
                }
                else
                {
                    _rankRows[playerRank].SetOtherPlayerBackground();
                }
            }

            _rewardAnimators = new List<Animator>();
            _equipmentPiecesRewards = RewardManager.Instance.GetEquipmentPieces();
            _equipmentMaterialsRewards = RewardManager.Instance.GetEquipmentMaterials();

            _totalGold = RewardManager.Instance.GetTotalGold();
            bool showRewardedGold = _totalGold > 0;
            _rewardedCoins.gameObject.SetActive(showRewardedGold);
            _rewardedCoins.Initialize(_totalGold);

            if (showRewardedGold)
            {
                _rewardAnimators.Add(_rewardedCoins.GetComponent<Animator>());
            }

            int totalTrophies = RewardManager.Instance.GetTotalTrophies();

            if (totalTrophies == 0)
            {
                _trophiesContainer.SetActive(false);
            }
            
            string symbol = totalTrophies > 0 ? "+" : "";
            _trophiesText.text = $"{symbol}{totalTrophies}";

            if (isMatchTutorial)
            {
                _rankRows[2].transform.parent.GetComponent<VerticalLayoutGroup>().enabled = false;
                _rankRows[2].gameObject.SetActive(false);
                _rankRows[3].gameObject.SetActive(false);
            }
        }

        public override void Show()
        {
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

            foreach (Animator animator in _rewardAnimators)
            {
                DOTween.Kill(animator.gameObject);
            }
            
            OnAcceptedRewards?.Invoke();
        }
    }
}