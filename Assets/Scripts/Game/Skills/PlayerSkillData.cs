using UnityEngine;

namespace Game.Skills
{
    [CreateAssetMenu(fileName = "player_skill_data", menuName = "ScriptableObjects/PlayerSkillData")]
    public class PlayerSkillData : SkillData
    {
        [field: SerializeField] public PlayerSkillType PlayerSkillType { get; private set; }
        [field: SerializeField] public bool IsFlatModifier { get; private set; }
        [field: SerializeField] public float[] ModifierAmountPerLevel { get; private set; }
    }
}
