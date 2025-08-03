using System.Collections.Generic;
using Application.Data.Currency;
using Application.Data.Equipment;
using Application.Data.Progression;
using Application.Telemetry;
using Application.Utils;
using Game.Drop;
using Game.Equipment;
using UI.MainMenu;
using UnityEngine;

namespace Game.Reward
{
    public partial class ScoutRewardsManager : MonoBehaviour
    {
        public static ScoutRewardsManager Instance { get; set; }

        private Dictionary<EquipmentPieceType, int> _equipmentMaterialsRewards;
        private List<EquipmentPieceInstance> _equipmentPiecesRewards;
        private List<EquipmentPieceType> _materialParticlesToCreate;
        private List<int> _equipmentParticlesToCreate;
        private LeagueData _leagueData;
        private int _amountOfGold;

        private void Awake()
        {
            Instance = this;
        }

        public void GenerateRewards(float secondsSinceLastClaim)
        {
            _equipmentMaterialsRewards = new Dictionary<EquipmentPieceType, int>();
            _equipmentPiecesRewards = new List<EquipmentPieceInstance>();
            _materialParticlesToCreate = new List<EquipmentPieceType>();
            _equipmentParticlesToCreate = new List<int>();
            _leagueData = LeaguesManager.Instance.GetCurrentLeagueData();
            
            _amountOfGold = GetGoldByLeagueData(_leagueData, secondsSinceLastClaim);

            if (_amountOfGold > 0)
            {
                CurrencyDataManager.Instance.GainGold(_amountOfGold);
            }
            
            int amountOfEquipmentPieces = GetEquipmentPiecesByLeagueData(_leagueData, secondsSinceLastClaim);

            if (amountOfEquipmentPieces > 0)
            {
                ReceiveGameRewardsEquipment(amountOfEquipmentPieces);
            }
            
            int amountOfMaterials = GetMaterialsByLeagueData(_leagueData, secondsSinceLastClaim);

            if (amountOfMaterials > 0)
            {
                ReceiveGameRewardsMaterials(amountOfMaterials);
            }
            
            RewardUIParticleManager.Instance.AddRewardsForNextMainMenuScreen(_amountOfGold, 0, 0, 0, _equipmentParticlesToCreate, _materialParticlesToCreate, 0);
        }

        private void ReceiveGameRewardsMaterials(int amount)
        {
            for (int i = 0; i < amount; ++i)
            {
                RewardEquipmentMaterials();
            }

            if (amount > 0)
            {
                foreach (KeyValuePair<EquipmentPieceType, int> typeAmountPair in _equipmentMaterialsRewards)
                {
                    CurrencyDataManager.Instance.GainMaterials(typeAmountPair.Key, typeAmountPair.Value);
                    AmplitudeManager.Instance.SoftCurrencyBalanceChange(
                        EnumUtils.SoftCurrencyTypeByPieceType(typeAmountPair.Key), typeAmountPair.Value,
                        CurrencyDataManager.Instance.CurrentMaterials(typeAmountPair.Key),
                        SoftCurrencyModificationType.LevelCompletedReward);
                    _materialParticlesToCreate.Add(typeAmountPair.Key);
                }
            }
        }

        private void RewardEquipmentMaterials()
        {
            EquipmentPieceType equipmentPieceType = EquipmentPieceTypeFactory.GetRandomEquipmentPieceType();
            _equipmentMaterialsRewards[equipmentPieceType] =
                _equipmentMaterialsRewards.GetValueOrDefault(equipmentPieceType, 0) + 1;
        }

        public Dictionary<EquipmentPieceType, int> GetEquipmentMaterials()
        {
            return _equipmentMaterialsRewards;
        }

        private void ReceiveGameRewardsEquipment(int amount)
        {
            for (int i = 0; i < amount; ++i)
            {
                RewardEquipmentPieces();
            }

            if (amount > 0)
            {
                foreach (EquipmentPieceInstance equipmentPieceInstance in _equipmentPiecesRewards)
                {
                    EquipmentDataManager.Instance.AddPieceToInventory(equipmentPieceInstance);
                    _equipmentParticlesToCreate.Add(equipmentPieceInstance.Id);
                }
            }
        }
        
        private void RewardEquipmentPieces()
        {
            EquipmentPieceInstance equipmentPieceInstance = EquipmentPieceFactory.GenerateRandomEquipmentPieceByLeague(_leagueData);
            _equipmentPiecesRewards.Add(equipmentPieceInstance);
        }
        
        public List<EquipmentPieceInstance> GetEquipmentPieces()
        {
            return _equipmentPiecesRewards;
        }
        
        public int GetGold()
        {
            return _amountOfGold;
        }

        public int GetGoldByLeagueData(LeagueData leagueData, float secondsSinceLastClaim)
        {
            int goldPerHour = leagueData.ScoutRewardsData.GoldAmountPerHour;
            return Mathf.FloorToInt(goldPerHour * secondsSinceLastClaim / TimeUtils.SecondsInHour);
        }

        public int GetMaterialsByLeagueData(LeagueData leagueData, float secondsSinceLastClaim)
        {
            float materialsPerHour = leagueData.ScoutRewardsData.MaterialsPerHour;
            return Mathf.FloorToInt(materialsPerHour * secondsSinceLastClaim / TimeUtils.SecondsInHour);
        }

        public int GetEquipmentPiecesRollsPerHour(LeagueData leagueData, float secondsSinceLastClaim)
        {
            float equipmentPiecesPerHour = leagueData.ScoutRewardsData.EquipmentPiecesRollsPerHour;
            return Mathf.FloorToInt(equipmentPiecesPerHour * secondsSinceLastClaim / TimeUtils.SecondsInHour);
        }

        public int GetEquipmentPiecesByLeagueData(LeagueData leagueData, float secondsSinceLastClaim)
        {
            int numberOfRolls = GetEquipmentPiecesRollsPerHour(leagueData, secondsSinceLastClaim);
            int numberOfEquipmentPieces = 0;
            
            for (int i = 0; i < numberOfRolls; i++)
            {
                float randomRoll = Random.Range(0.0f, 1.0f);

                for (int j = leagueData.ScoutRewardsData.ScoutEquipmentPiecesConfig.Length - 1; j >= 0; j--)
                {
                    if (randomRoll <= leagueData.ScoutRewardsData.ScoutEquipmentPiecesConfig[j].Chance)
                    {
                        numberOfEquipmentPieces += leagueData.ScoutRewardsData.ScoutEquipmentPiecesConfig[j].Amount;
                        break;
                    }
                }
            }
            
            return numberOfEquipmentPieces;
        }
    }
}