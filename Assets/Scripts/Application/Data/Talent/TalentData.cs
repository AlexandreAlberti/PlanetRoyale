using UnityEngine;

namespace Application.Data.Talent
{
    [CreateAssetMenu(fileName = "data_talent", menuName = "ScriptableObjects/TalentData", order = 0)]
    public class TalentData : ScriptableObject
    {
        [field: SerializeField] public TalentType Type { get; set; }
        [field: SerializeField] public Sprite Icon { get; set; }
        [field: SerializeField] public string Name { get; set; }
        [field: SerializeField] public string Description { get; set; }
        [field: SerializeField] public string UpgradeComparisonDescription { get; set; }
        [field: SerializeField] public float[] UpgradeAmountPerLevel { get; set; }
    }
}