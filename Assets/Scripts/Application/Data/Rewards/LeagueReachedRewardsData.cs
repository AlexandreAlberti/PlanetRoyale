using System;
using Application.Data.Equipment;
using Application.Data.Planet;
using UnityEngine;

namespace Application.Data.Rewards
{
    [Serializable]
    public class MaterialReward {
        public EquipmentPieceType Type;
        public int Amount;
    }
    
    [CreateAssetMenu(fileName = "LeagueReachedRewardsData", menuName = "ScriptableObjects/LeagueReachedRewardsData", order = 0)]
    public class LeagueReachedRewardsData : ScriptableObject
    {
        [field: SerializeField] public int Gold { get; private set; }
        [field: SerializeField] public int Gems { get; private set; }
        [field: SerializeField] public int SilverKeys { get; private set; }
        [field: SerializeField] public int GoldenKeys { get; private set; }
        [field: SerializeField] public MaterialReward[] Materials { get; private set; }
        [field: SerializeField] public EquipmentReward[] Equipments { get; private set; }
        [field: SerializeField] public PlanetData[] Planets { get; private set; }
    }
}