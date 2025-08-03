using Application.Data.Equipment;
using Application.Data.Progression;
using Application.Utils;
using UnityEngine;

namespace Game.Equipment
{
    public static class EquipmentPieceFactory
    {
        public static EquipmentPieceInstance GenerateRandomEquipmentPiece()
        {
            EquipmentPieceData[] equipmentPiecesPool = EquipmentDataManager.Instance.GetEquipmentPiecesPool();
            return new EquipmentPieceInstance(equipmentPiecesPool[Random.Range(0, equipmentPiecesPool.Length)].Id);
        }

        public static EquipmentPieceInstance GenerateEquipmentPieceById(int id, int rarity = 0)
        {
            EquipmentPieceData[] equipmentPiecesPool = EquipmentDataManager.Instance.GetEquipmentPiecesPool();

            foreach (EquipmentPieceData equipmentPieceData in equipmentPiecesPool)
            {
                if (equipmentPieceData.Id == id)
                {
                    return new EquipmentPieceInstance(equipmentPieceData.Id, rarity);
                }
            }

            return null;
        }

        public static EquipmentPieceInstance GenerateRandomEquipmentPieceByLeague(LeagueData leagueData)
        {
            EquipmentPieceData[] equipmentPiecesPool = leagueData.RewardEquipmentPiecesPool;
            int randomEquipmentPieceIndex = Random.Range(0, equipmentPiecesPool.Length);
            EquipmentPieceData equipmentPieceData = equipmentPiecesPool[randomEquipmentPieceIndex];
            
            float rarityChance = Random.Range(0, NumberConstants.OneHundred);
            
            int rarity = (int) EquipmentPieceRarity.Common;
        
            if (rarityChance <= leagueData.RewardEquipmentPiecesRarityPercentages[(int) EquipmentPieceRarity.Common])
            {
                rarity = (int) EquipmentPieceRarity.Common;
            }
            else if (rarityChance <= leagueData.RewardEquipmentPiecesRarityPercentages[(int) EquipmentPieceRarity.Common]
                     + leagueData.RewardEquipmentPiecesRarityPercentages[(int) EquipmentPieceRarity.Uncommon])
            {
                rarity = (int) EquipmentPieceRarity.Uncommon;
            }
            else if (rarityChance <= leagueData.RewardEquipmentPiecesRarityPercentages[(int) EquipmentPieceRarity.Common]
                     + leagueData.RewardEquipmentPiecesRarityPercentages[(int) EquipmentPieceRarity.Uncommon]
                     + leagueData.RewardEquipmentPiecesRarityPercentages[(int) EquipmentPieceRarity.Rare])
            {
                rarity = (int) EquipmentPieceRarity.Rare;
            }
            else if (rarityChance <= leagueData.RewardEquipmentPiecesRarityPercentages[(int) EquipmentPieceRarity.Common]
                     + leagueData.RewardEquipmentPiecesRarityPercentages[(int) EquipmentPieceRarity.Uncommon]
                     + leagueData.RewardEquipmentPiecesRarityPercentages[(int) EquipmentPieceRarity.Rare]
                     + leagueData.RewardEquipmentPiecesRarityPercentages[(int) EquipmentPieceRarity.Epic])
            {
                rarity = (int) EquipmentPieceRarity.Epic;
            }
            else if (rarityChance <= leagueData.RewardEquipmentPiecesRarityPercentages[(int) EquipmentPieceRarity.Common]
                     + leagueData.RewardEquipmentPiecesRarityPercentages[(int) EquipmentPieceRarity.Uncommon]
                     + leagueData.RewardEquipmentPiecesRarityPercentages[(int) EquipmentPieceRarity.Rare]
                     + leagueData.RewardEquipmentPiecesRarityPercentages[(int) EquipmentPieceRarity.Epic]
                     + leagueData.RewardEquipmentPiecesRarityPercentages[(int) EquipmentPieceRarity.Legendary])
            {
                rarity = (int) EquipmentPieceRarity.Legendary;
            }
            
            return new EquipmentPieceInstance(equipmentPieceData.Id, rarity);
        }
    }
}