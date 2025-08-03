using UnityEngine;

namespace Application.Data.Rewards
{
    [CreateAssetMenu(fileName = "GoldDungeonData", menuName = "ScriptableObjects/GoldDungeonData", order = 0)]
    public class GoldDungeonData : ScriptableObject
    {
        [field: SerializeField] public int GoldAmountForTier1Enemies { get; private set; }
        [field: SerializeField] public int GoldAmountForTier2Enemies { get; private set; }
        [field: SerializeField] public int GoldAmountForTier3Enemies { get; private set; }

    }
}