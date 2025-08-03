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
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class QuickScoutPanel : Popup
    {
        [SerializeField] private TextMeshProUGUI _energyCostText;
        [SerializeField] private TextMeshProUGUI _chancesText;
        [SerializeField] private Image _energyIcon;
        [SerializeField] private RewardCoinsHolder _rewardCoinsHolder;
        [SerializeField] private GameObject _commonEquipmentPiece;
        [SerializeField] private GameObject _uncommonEquipmentPiece;
        [SerializeField] private GameObject _rareEquipmentPiece;
        [SerializeField] private GameObject _epicEquipmentPiece;
        [SerializeField] private GameObject _legendaryEquipmentPiece;
        [SerializeField] private EquipmentMaterial _equipmentMaterial;
        [SerializeField] private UpgradeButton _claimWithEnergyButton;
        [SerializeField] private ScoutRewardsPanel _scoutRewardsPanel;
        
        private LeagueData _leagueData;
        private Coroutine _updateClaimWithEnergyTimeCoroutine;

        public void Initialize()
        {
            _leagueData = LeaguesManager.Instance.GetCurrentLeagueData();
            _energyCostText.text = $"x {ScoutDataManager.Instance.GetQuickScoutEnergyCost()}";
            
            SetGoldReward();
            SetEquipmentPiecesReward();
            SetMaterialsReward();

            StartUpdatingClaimWithEnergyTime();
        }
        
        private void StartUpdatingClaimWithEnergyTime()
        {
            UpdateClaimWithEnergyButton();
            
            if (_updateClaimWithEnergyTimeCoroutine != null)
            {
                StopCoroutine(_updateClaimWithEnergyTimeCoroutine);
            }
            
            _updateClaimWithEnergyTimeCoroutine = StartCoroutine(UpdateClaimWithEnergyTimeCoroutine());
        }
        
        private void StopUpdatingClaimWithEnergyTime()
        {
            if (_updateClaimWithEnergyTimeCoroutine != null)
            {
                StopCoroutine(_updateClaimWithEnergyTimeCoroutine);
                _updateClaimWithEnergyTimeCoroutine = null;
            }
        }
        
        private IEnumerator UpdateClaimWithEnergyTimeCoroutine()
        {
            while (ScoutDataManager.Instance.GetQuickScoutDailyChances() <= 0)
            {
                UpdateClaimWithEnergyButton();
                yield return new WaitForSeconds(NumberConstants.One);
            }
            
            UpdateClaimWithEnergyButton();
        }
        
        private void UpdateClaimWithEnergyButton()
        {
            if (ScoutDataManager.Instance.GetQuickScoutDailyChances() <= 0)
            {
                long nextClaimTime = ScoutDataManager.Instance.GetNextQuickClaimTime();
                TimeSpan timeSpan = TimeSpan.FromSeconds(nextClaimTime);
                int totalHours = (int)timeSpan.TotalHours;
                _chancesText.text = $"New chances in {totalHours:00}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
                _chancesText.color = Color.red;
                _energyCostText.color = EnergyDataManager.Instance.GetEnergy() < ScoutDataManager.Instance.GetQuickScoutEnergyCost() ? Color.red : Color.gray;
                _energyIcon.color = Color.gray;
                _claimWithEnergyButton.Disable();
                return;
            }
            
            if (EnergyDataManager.Instance.GetEnergy() < ScoutDataManager.Instance.GetQuickScoutEnergyCost())
            {
                _chancesText.text = $"Chances: {ScoutDataManager.Instance.GetQuickScoutDailyChances()}";
                _chancesText.color = ScoutDataManager.Instance.GetQuickScoutDailyChances() <= 0 ? Color.red : Color.white;
                _energyCostText.color = Color.red;
                _energyIcon.color = Color.white;
                _claimWithEnergyButton.Disable();
                return;
            }
            
            _chancesText.color = Color.white;
            _chancesText.text = $"Chances: {ScoutDataManager.Instance.GetQuickScoutDailyChances()}";
            _energyCostText.color = Color.white;
            _energyIcon.color = Color.white;
            _claimWithEnergyButton.Enable();
            _claimWithEnergyButton.OnClicked -= ClaimWithEnergyButton_OnClicked;
            _claimWithEnergyButton.OnClicked += ClaimWithEnergyButton_OnClicked;
        }

        private void ClaimWithEnergyButton_OnClicked(InteractableButton button)
        {
            _claimWithEnergyButton.OnClicked -= ClaimWithEnergyButton_OnClicked;
            _scoutRewardsPanel.OnClosed += ScoutRewardsPanel_OnClosed;
            _scoutRewardsPanel.Initialize(ScoutDataManager.Instance.GetQuickScoutRewardValue());
            _scoutRewardsPanel.Show();
            EnergyDataManager.Instance.SpendEnergy(ScoutDataManager.Instance.GetQuickScoutEnergyCost());
            ScoutDataManager.Instance.ClaimQuickScout();
        }

        private void ScoutRewardsPanel_OnClosed()
        {
            _scoutRewardsPanel.OnClosed -= ScoutRewardsPanel_OnClosed;
            Initialize();
        }

        private void SetGoldReward()
        {
            int goldAmount = ScoutRewardsManager.Instance.GetGoldByLeagueData(_leagueData, ScoutDataManager.Instance.GetQuickScoutRewardValue());

            if (goldAmount > 0)
            {
                _rewardCoinsHolder.Initialize(goldAmount);
                _rewardCoinsHolder.gameObject.SetActive(true);
            }
            else
            {
                _rewardCoinsHolder.gameObject.SetActive(false);
            }
        }

        private void SetEquipmentPiecesReward()
        {
            int equipmentPiecesRollsAmount = ScoutRewardsManager.Instance.GetEquipmentPiecesRollsPerHour(_leagueData, ScoutDataManager.Instance.GetQuickScoutRewardValue());
            _commonEquipmentPiece.SetActive(equipmentPiecesRollsAmount > 0 && _leagueData.RewardEquipmentPiecesRarityPercentages[0] > 0);
            _uncommonEquipmentPiece.SetActive(equipmentPiecesRollsAmount > 0 && _leagueData.RewardEquipmentPiecesRarityPercentages[1] > 0);
            _rareEquipmentPiece.SetActive(equipmentPiecesRollsAmount > 0 && _leagueData.RewardEquipmentPiecesRarityPercentages[2] > 0);
            _epicEquipmentPiece.SetActive(equipmentPiecesRollsAmount > 0 && _leagueData.RewardEquipmentPiecesRarityPercentages[3] > 0);
            _legendaryEquipmentPiece.SetActive(equipmentPiecesRollsAmount > 0 && _leagueData.RewardEquipmentPiecesRarityPercentages[4] > 0);
        }

        private void SetMaterialsReward()
        {
            int materialsAmount = ScoutRewardsManager.Instance.GetMaterialsByLeagueData(_leagueData, ScoutDataManager.Instance.GetQuickScoutRewardValue());

            if (materialsAmount > 0)
            {
                _equipmentMaterial.Initialize(EquipmentPieceType.Random);
                _equipmentMaterial.UpdateAmount(materialsAmount);
                _equipmentMaterial.gameObject.SetActive(true);
            }
            else
            {
                _equipmentMaterial.gameObject.SetActive(false);
            }
        }
        
        public override void Hide()
        {
            _animator.Play("Hidden");
            
            OnClosed?.Invoke();
        }
    }
}