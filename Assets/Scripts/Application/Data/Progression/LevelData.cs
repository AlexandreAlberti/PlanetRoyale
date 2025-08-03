using UnityEngine;

namespace Application.Data.Progression
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/LevelData")]
    public class LevelData : ScriptableObject
    {
        [field: SerializeField] public string NodeTitle { get; private set; }
        [field: SerializeField] public int[] GoldAmountOnCompletionByRank { get; private set; }
        [field: SerializeField] public int MaxRankingToCompleteLevel { get; private set; }
        [field: SerializeField] public bool IsFinalNode { get; private set; }
        [field: SerializeField] public int GoldAmountOnFinalNodeCompletion { get; private set; }
        [field: SerializeField] public int[] EquipmentRewardByRank { get; private set; }
        [field: SerializeField] public int[] MaterialsRewardByRank { get; private set; }
    }
}