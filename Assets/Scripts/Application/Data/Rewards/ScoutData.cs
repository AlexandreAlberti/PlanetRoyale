using Game.Reward;
using UnityEngine;

namespace Application.Data.Rewards
{
    [CreateAssetMenu(fileName = "ScoutData", menuName = "ScriptableObjects/ScoutData", order = 0)]
    public class ScoutData : ScriptableObject
    {
        [field: SerializeField] public int GoldAmountPerHour { get; private set; }
        [field: SerializeField] public float MaterialsPerHour { get; private set; }
        [field: SerializeField] public float EquipmentPiecesRollsPerHour { get; private set; }
        [field: SerializeField] public ScoutEquipmentRewardConfig[] ScoutEquipmentPiecesConfig { get; private set; }
    }
}