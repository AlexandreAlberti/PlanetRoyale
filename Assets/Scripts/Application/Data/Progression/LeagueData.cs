using Application.Data.Equipment;
using Application.Data.Rewards;
using UnityEngine;

namespace Application.Data.Progression
{
    [CreateAssetMenu(fileName = "LeagueData", menuName = "ScriptableObjects/LeagueData", order = 0)]
    public class LeagueData : ScriptableObject
    {
        [field: SerializeField] public Sprite LeagueSprite { get; set; }
        [field: SerializeField] public int MinTrophiesInclusive { get; set; }
        [field: SerializeField] public int MaxTrophiesExclusive { get; set; }
        [field: SerializeField] public string LeagueTitle { get; set; }
        [field: SerializeField] public string TrophyRoadTitle { get; set; }
        [field: SerializeField] public int[] GoldRewardByRank { get; private set; }
        [field: SerializeField] public int[] TrophiesRewardByRank { get; private set; }
        [field: SerializeField] public int[] MaterialsRewardByRank { get; private set; }
        [field: SerializeField] public int[] EquipmentRewardByRank { get; private set; }
        [field: SerializeField] public EquipmentPieceData[] RewardEquipmentPiecesPool { get; private set; }
        [field: SerializeField] public float[] RewardEquipmentPiecesRarityPercentages { get; private set; }
        [field: SerializeField] public float EnemyHealthMultiplier { get; private set; }
        [field: SerializeField] public float EnemyDamageMultiplier { get; private set; }
        [field: SerializeField] public LeagueReachedRewardsData LeagueReachedReward { get; private set; }
        [field: SerializeField] public ScoutData ScoutRewardsData { get; private set; }
        [field: SerializeField] public GoldDungeonData GoldDungeonRewardsData { get; private set; }
    }
}