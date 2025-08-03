using System;
using UnityEngine;

namespace Application.Data.Hero
{
    [Serializable]
    [CreateAssetMenu(fileName = "baseStats_data", menuName = "ScriptableObjects/Hero/BaseStatsData", order = 1)]
    public class BaseStatsData : ScriptableObject
    {
        [field: SerializeField] public int BaseAttack { get; private set; }
        [field: SerializeField] public int BaseMaxHp { get; private set; }
        [field: SerializeField] public float BaseRangedAttackRange { get; private set; }
        [field: SerializeField] public float BaseGreatSwordAttackRange { get; private set; }
        [field: SerializeField] public float BaseTridentAttackRange { get; private set; }
        [field: SerializeField] public float BaseMovementSpeed { get; private set; }
        [field: SerializeField] public float BaseLevelUpHealing { get; private set; }
        [field: SerializeField] public float BaseCollectRadius { get; private set; }
        [field: SerializeField] public float BaseAttackSpeed { get; private set; }
    }
}