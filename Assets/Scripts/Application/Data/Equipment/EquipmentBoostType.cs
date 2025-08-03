using System;

namespace Application.Data.Equipment
{
    [Serializable]
    public enum EquipmentBoostType
    {
        Attack,
        MaxHp,
        AttackRange,
        AttackSpeed,
        DodgeChance,
        LevelUpHealing,
        Revive,
        MovementSpeed,
        CollectRadius,
        DamageReduction,
        ReflectMelee,
        ReflectRanged,
        Fake
    }
}