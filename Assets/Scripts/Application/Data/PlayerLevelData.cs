using UnityEngine;

namespace Application.Data
{
    [CreateAssetMenu(fileName = "PlayerLevelData", menuName = "ScriptableObjects/PlayerLevelData")]
    public class PlayerLevelData : ScriptableObject
    {
        [field: SerializeField] public int[] XpThresholdPerLevel { get; private set; }
    }
}
