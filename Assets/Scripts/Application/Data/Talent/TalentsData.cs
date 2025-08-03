using UnityEngine;

namespace Application.Data.Talent
{
    [CreateAssetMenu(fileName = "data_talents", menuName = "ScriptableObjects/TalentsData", order = 0)]
    public class TalentsData : ScriptableObject
    {
        [field: SerializeField] public TalentData[] Talents { get; set; }
        [field: SerializeField] public int[] GoldCostPerUpgrade { get; set; }
    }
}