using System;
using System.Collections.Generic;
using System.Linq;
using Application.Data.Currency;
using Application.Data.Equipment;
using Application.Data.Progression;
using Application.Data.Talent;
using Application.Telemetry;
using Application.Utils;
using Game.Equipment;
using Game.Ftue;
using UI.MainMenu;
using UnityEngine;

namespace Game.Reward
{
    public class RewardManager : MonoBehaviour
    {
        [SerializeField] private TalentsData _talentsData;
        [SerializeField] private LeagueData _firstLeagueData;
        [SerializeField] private LeagueData _secondLeagueData;
        [SerializeField] private LeagueData _thirdLeagueData;
        [SerializeField] private EquipmentSlotData _equipmentSlotData;
        
        public Action<int> OnGoldUpdated;

        public static RewardManager Instance;
        
        private int _goldWon;
        private int _trophiesWon;
        private Dictionary<EquipmentPieceType, int> _equipmentMaterialsRewards;
        private List<EquipmentPieceInstance> _equipmentPiecesRewards;
        private List<EquipmentPieceType> _materialParticlesToCreate;
        private List<int> _equipmentParticlesToCreate;
        private LeagueData _leagueData;

        private void Awake()
        {
            Instance = this;
        }

        public void Initialize()
        {
            _goldWon = 0;
            _trophiesWon = 0;
            _equipmentMaterialsRewards = new Dictionary<EquipmentPieceType, int>();
            _equipmentPiecesRewards = new List<EquipmentPieceInstance>();
            _materialParticlesToCreate = new List<EquipmentPieceType>();
            _equipmentParticlesToCreate = new List<int>();
            
            OnGoldUpdated?.Invoke(_goldWon);
        }

        public void ReceiveGameRewards(int playerRank)
        {
            _equipmentMaterialsRewards = new Dictionary<EquipmentPieceType, int>();
            _equipmentPiecesRewards = new List<EquipmentPieceInstance>();
            _leagueData = LeaguesManager.Instance.GetCurrentLeagueData();
            ReceiveGameRewardsCurrencies(playerRank);
            ReceiveGameRewardsEquipment(playerRank);
            ReceiveGameRewardsMaterials(playerRank);
            
            RewardUIParticleManager.Instance.AddRewardsForNextMainMenuScreen(_goldWon, 0, 0, 0, _equipmentParticlesToCreate, _materialParticlesToCreate, _trophiesWon);
        }

        public void ReceiveGoldDungeonReward(int collectedGoldAmount)
        {
            _equipmentMaterialsRewards = new Dictionary<EquipmentPieceType, int>();
            _equipmentPiecesRewards = new List<EquipmentPieceInstance>();
            _goldWon = collectedGoldAmount;
            _trophiesWon = 0;
            CurrencyDataManager.Instance.GainGold(_goldWon);
            RewardUIParticleManager.Instance.AddRewardsForNextMainMenuScreen(_goldWon, 0, 0, 0, _equipmentParticlesToCreate, _materialParticlesToCreate, _trophiesWon);
        }

        private void ReceiveGameRewardsCurrencies(int playerRank)
        {
            if (FtueManager.Instance.HasFinishedFirstRealGame() && !FtueManager.Instance.HasFinishedSecondRealGame())
            {
                _goldWon = _talentsData.GoldCostPerUpgrade[0];
                _trophiesWon = _firstLeagueData.MaxTrophiesExclusive - _firstLeagueData.MinTrophiesInclusive;
            }
            else if (FtueManager.Instance.HasFinishedSecondRealGame() && !FtueManager.Instance.HasFinishedThirdRealGame())
            {
                _goldWon = _equipmentSlotData.GoldUpgradeCostPerLevel[0];
                _trophiesWon = _secondLeagueData.MaxTrophiesExclusive - _secondLeagueData.MinTrophiesInclusive;
            }
            else if (FtueManager.Instance.HasFinishedThirdRealGame() && !FtueManager.Instance.HasFinishedFourthRealGame())
            {
                _goldWon = _leagueData.GoldRewardByRank[playerRank];
                _trophiesWon = 0;
            }
            else if (FtueManager.Instance.HasFinishedFourthRealGame() && !FtueManager.Instance.HasFinishedFtue())
            {
                _goldWon = _leagueData.GoldRewardByRank[playerRank];
                _trophiesWon = _thirdLeagueData.MaxTrophiesExclusive - _thirdLeagueData.MinTrophiesInclusive;
            }
            else
            {
                _goldWon = _leagueData.GoldRewardByRank[playerRank];
                _trophiesWon = _leagueData.TrophiesRewardByRank[playerRank];
            }

            CurrencyDataManager.Instance.GainGold(_goldWon);
            CurrencyDataManager.Instance.UpdateTrophies(_trophiesWon);

            AmplitudeManager.Instance.SoftCurrencyBalanceChange(SoftCurrencyType.Gold,
                _goldWon,
                CurrencyDataManager.Instance.CurrentGold(),
                SoftCurrencyModificationType.LevelCompletedReward);

            AmplitudeManager.Instance.SoftCurrencyBalanceChange(SoftCurrencyType.Trophies,
                _goldWon,
                CurrencyDataManager.Instance.CurrentGold(),
                SoftCurrencyModificationType.LevelCompletedReward);
        }

        private void ReceiveGameRewardsMaterials(int playerRank)
        {
            if (FtueManager.Instance.HasFinishedFirstRealGame() && !FtueManager.Instance.HasFinishedSecondRealGame()
                || FtueManager.Instance.HasFinishedThirdRealGame() && !FtueManager.Instance.HasFinishedFourthRealGame())
            {
                return;
            }

            int amountOfMaterials;
            
            if (FtueManager.Instance.HasFinishedSecondRealGame() && !FtueManager.Instance.HasFinishedThirdRealGame())
            {
                amountOfMaterials = _equipmentSlotData.MaterialUpgradeCostPerLevel[0];
                _equipmentMaterialsRewards.Add(EquipmentPieceType.Weapon, amountOfMaterials);
            }
            else
            {
                amountOfMaterials = _leagueData.MaterialsRewardByRank[playerRank];
                
                for (int i = 0; i < amountOfMaterials; ++i)
                {
                    RewardEquipmentMaterials();
                }
            }

            if (amountOfMaterials > 0)
            {
                foreach (KeyValuePair<EquipmentPieceType, int> typeAmountPair in _equipmentMaterialsRewards)
                {
                    CurrencyDataManager.Instance.GainMaterials(typeAmountPair.Key, typeAmountPair.Value);
                    AmplitudeManager.Instance.SoftCurrencyBalanceChange(EnumUtils.SoftCurrencyTypeByPieceType(typeAmountPair.Key), typeAmountPair.Value, CurrencyDataManager.Instance.CurrentMaterials(typeAmountPair.Key), SoftCurrencyModificationType.LevelCompletedReward);
                    _materialParticlesToCreate.Add(typeAmountPair.Key);
                }
            }
        }
        
        private void ReceiveGameRewardsEquipment(int playerRank)
        {
            if (FtueManager.Instance.HasFinishedFirstRealGame() && !FtueManager.Instance.HasFinishedSecondRealGame()
                || FtueManager.Instance.HasFinishedSecondRealGame() && !FtueManager.Instance.HasFinishedThirdRealGame()
                || FtueManager.Instance.HasFinishedThirdRealGame() && !FtueManager.Instance.HasFinishedFourthRealGame())
            {
                return;
            }

            int amountOfEquipmentPieces = _leagueData.EquipmentRewardByRank[playerRank];
                
            for (int i = 0; i < amountOfEquipmentPieces; ++i)
            {
                RewardEquipmentPieces();
            }

            if (amountOfEquipmentPieces > 0)
            {
                foreach (EquipmentPieceInstance equipmentPieceInstance in _equipmentPiecesRewards)
                {
                    EquipmentDataManager.Instance.AddPieceToInventory(equipmentPieceInstance);
                    _equipmentParticlesToCreate.Add(equipmentPieceInstance.Id);
                }
            }
        }

        private void RewardEquipmentMaterials()
        {
            EquipmentPieceType equipmentPieceType = EquipmentPieceTypeFactory.GetRandomEquipmentPieceType();
            _equipmentMaterialsRewards[equipmentPieceType] = _equipmentMaterialsRewards.GetValueOrDefault(equipmentPieceType, 0) + 1;
        }
        
        private void RewardEquipmentPieces()
        {
            EquipmentPieceInstance equipmentPieceInstance = EquipmentPieceFactory.GenerateRandomEquipmentPieceByLeague(_leagueData);
            _equipmentPiecesRewards.Add(equipmentPieceInstance);
        }

        public int GetTotalGold()
        {
            return _goldWon;
        }

        public int GetTotalTrophies()
        {
            return _trophiesWon;
        }
        
        public Dictionary<EquipmentPieceType, int> GetEquipmentMaterials()
        {
            return _equipmentMaterialsRewards;
        }

        public List<EquipmentPieceInstance> GetEquipmentPieces()
        {
            return _equipmentPiecesRewards;
        }

        public string ObtainRewardsJson()
        {   
            return $"{{gold:{_goldWon}, trophies:{_trophiesWon}, materials:{_equipmentMaterialsRewards.Values.Sum()}, equipments:{_equipmentPiecesRewards.Count}}}";
        }
    }
}