using System;
using System.Collections;
using Application.Data.Equipment;
using Application.Data.Progression;
using Application.Data.Rewards;
using Application.Utils;
using Game.Reward;
using TMPro;
using UI.Button;
using UI.MainMenu;
using UI.MainMenu.RedDot;
using UnityEngine;

namespace Game.UI
{
    public class ScoutPanel : Popup
    {
        [SerializeField] private UpgradeButton _claimButton;
        [SerializeField] private TextMeshProUGUI _claimButtonText;
        [SerializeField] private TextMeshProUGUI _noRewardsText;
        [SerializeField] private ScoutRewardsPanel _scoutRewardsPanel;
        [SerializeField] private TextMeshProUGUI _scoutGoldPerHourText;
        [SerializeField] private TextMeshProUGUI _scoutTimeText;
        [SerializeField] private RewardCoinsHolder _rewardCoinsHolder;
        [SerializeField] private GameObject _commonEquipmentPiece;
        [SerializeField] private GameObject _uncommonEquipmentPiece;
        [SerializeField] private GameObject _rareEquipmentPiece;
        [SerializeField] private GameObject _epicEquipmentPiece;
        [SerializeField] private GameObject _legendaryEquipmentPiece;
        [SerializeField] private EquipmentMaterial _equipmentMaterial;
        [SerializeField] private InteractableButton _quickScoutButton;
        [SerializeField] private QuickScoutPanel _quickScoutPanel;

        private LeagueData _leagueData;
        private Coroutine _updateTimeCoroutine;
        private int _equipmentPiecesRollsAmount;
        private int _materialsAmount;
        private int _goldAmount;
        private float _indicatorsCooldown;

        public void Initialize()
        {
            _leagueData = LeaguesManager.Instance.GetCurrentLeagueData();
            _claimButton.OnClicked += ClaimButton_OnClicked;
            _quickScoutButton.OnClicked += QuickScoutButton_OnClicked;
            _scoutGoldPerHourText.text = $"{_leagueData.ScoutRewardsData.GoldAmountPerHour}/h";
            StartUpdatingScoutTime();
        }

        private void QuickScoutButton_OnClicked(InteractableButton button)
        {
            _quickScoutButton.OnClicked -= QuickScoutButton_OnClicked;
            _quickScoutPanel.OnClosed += QuickScoutButton_OnClosed;
            _quickScoutPanel.Initialize();
            _quickScoutPanel.Show();
        }

        private void QuickScoutButton_OnClosed()
        {
            _quickScoutPanel.OnClosed -= QuickScoutButton_OnClosed;
            _quickScoutButton.OnClicked += QuickScoutButton_OnClicked;
            StartUpdatingScoutTime();
        }

        private void ScoutRewardsPanel_OnClosed()
        {
            _scoutRewardsPanel.OnClosed -= ScoutRewardsPanel_OnClosed;
            _claimButton.OnClicked += ClaimButton_OnClicked;
            StartUpdatingScoutTime();
        }

        private void ClaimButton_OnClicked(InteractableButton button)
        {
            _claimButton.OnClicked -= ClaimButton_OnClicked;
            _scoutRewardsPanel.OnClosed += ScoutRewardsPanel_OnClosed;
            _scoutRewardsPanel.Initialize(ScoutDataManager.Instance.GetScoutTime());
            _scoutRewardsPanel.Show();
            ScoutDataManager.Instance.ClaimScout();
            StopUpdatingScoutTime();
        }
        
        private void StartUpdatingScoutTime()
        {
            UpdatePanel();
            
            if (_updateTimeCoroutine != null)
            {
                StopCoroutine(_updateTimeCoroutine);
            }
            
            _updateTimeCoroutine = StartCoroutine(UpdateScoutTimeCoroutine());
        }

        private void StopUpdatingScoutTime()
        {
            if (_updateTimeCoroutine != null)
            {
                StopCoroutine(_updateTimeCoroutine);
                _updateTimeCoroutine = null;
            }
        }
        
        private IEnumerator UpdateScoutTimeCoroutine()
        {
            while (!ScoutDataManager.Instance.IsAtMaxScoutTime())
            {
                UpdatePanel();
                yield return new WaitForSeconds(NumberConstants.One);
            }
            
            UpdatePanel();
        }

        private void UpdatePanel()
        {
            UpdateScoutTime();
            UpdateClaimButton();
            UpdateRewards();
        }

        private void UpdateRewards()
        {
            UpdateGoldReward();
            UpdateEquipmentPiecesReward();
            UpdateMaterialsReward();

            if (_goldAmount == 0 && _materialsAmount == 0 && _equipmentPiecesRollsAmount == 0)
            {
                _noRewardsText.gameObject.SetActive(true);
            }
            else
            {
                _noRewardsText.gameObject.SetActive(false);
            }
        }

        private void UpdateClaimButton()
        {
            if (ScoutDataManager.Instance.CanClaim())
            {
                _claimButtonText.color = Color.white;
                _claimButtonText.text = $"Claim";
                _claimButton.Enable();
            }
            else
            {
                long nextClaimTime = ScoutDataManager.Instance.GetNextClaimTime();
                TimeSpan timeSpan = TimeSpan.FromSeconds(nextClaimTime);
                int totalHours = (int)timeSpan.TotalHours;
                _claimButtonText.color = Color.gray;
                _claimButtonText.text = $"Claimable in {totalHours:00}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
                _claimButton.Disable();
            }
        }

        private void UpdateEquipmentPiecesReward()
        {
            _equipmentPiecesRollsAmount = ScoutRewardsManager.Instance.GetEquipmentPiecesRollsPerHour(_leagueData, ScoutDataManager.Instance.GetScoutTime());
            _commonEquipmentPiece.SetActive(_equipmentPiecesRollsAmount > 0 && _leagueData.RewardEquipmentPiecesRarityPercentages[0] > 0);
            _uncommonEquipmentPiece.SetActive(_equipmentPiecesRollsAmount > 0 && _leagueData.RewardEquipmentPiecesRarityPercentages[1] > 0);
            _rareEquipmentPiece.SetActive(_equipmentPiecesRollsAmount > 0 && _leagueData.RewardEquipmentPiecesRarityPercentages[2] > 0);
            _epicEquipmentPiece.SetActive(_equipmentPiecesRollsAmount > 0 && _leagueData.RewardEquipmentPiecesRarityPercentages[3] > 0);
            _legendaryEquipmentPiece.SetActive(_equipmentPiecesRollsAmount > 0 && _leagueData.RewardEquipmentPiecesRarityPercentages[4] > 0);
        }

        private void UpdateMaterialsReward()
        {
            _materialsAmount = ScoutRewardsManager.Instance.GetMaterialsByLeagueData(_leagueData, ScoutDataManager.Instance.GetScoutTime());

            if (_materialsAmount > 0)
            {
                _equipmentMaterial.Initialize(EquipmentPieceType.Random);
                _equipmentMaterial.UpdateAmount(_materialsAmount);
                _equipmentMaterial.gameObject.SetActive(true);
            }
            else
            {
                _equipmentMaterial.gameObject.SetActive(false);
            }
        }

        private void UpdateGoldReward()
        {
            _goldAmount = ScoutRewardsManager.Instance.GetGoldByLeagueData(_leagueData, ScoutDataManager.Instance.GetScoutTime());

            if (_goldAmount > 0)
            {
                _rewardCoinsHolder.Initialize(_goldAmount);
                _rewardCoinsHolder.gameObject.SetActive(true);
            }
            else
            {
                _rewardCoinsHolder.gameObject.SetActive(false);
            }
        }

        private void UpdateScoutTime()
        {
            _scoutTimeText.color = ScoutDataManager.Instance.IsAtMaxScoutTime() ? Color.red : Color.green;
            TimeSpan timeSpan = TimeSpan.FromSeconds(ScoutDataManager.Instance.GetScoutTime());
            int totalHours = (int) timeSpan.TotalHours;
            _scoutTimeText.text = $"Scout time: {totalHours:00}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
        }

        public override void Hide()
        {
            StopUpdatingScoutTime();
            _animator.Play("Hidden");
        }
    }
}