using UnityEngine;

namespace Game.PlayerUnit
{
    [CreateAssetMenu(fileName = "WeaponConfig_data", menuName = "ScriptableObjects/WeaponConfigData", order = 1)]
    public class WeaponConfigData : ScriptableObject
    {
        [field: SerializeField] public float FactorOfPlayerRadiusToSee { get; private set; }
        [field: SerializeField, Range(0.0f, 1.0f)] public float LevelUpAttackDamageIncreasePercentage { get; private set; }
        [field: SerializeField, Range(0.0f, 1.0f)] public float LevelUpAttackRangeIncreasePercentage { get; private set; }
        [field: SerializeField] public float WeaponDamageMultiplier { get; private set; }
    }
}
