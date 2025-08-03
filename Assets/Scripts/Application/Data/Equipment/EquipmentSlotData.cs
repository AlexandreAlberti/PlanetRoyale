using UnityEngine;

namespace Application.Data.Equipment
{
    [CreateAssetMenu(fileName = "equipmentSlot_data", menuName = "ScriptableObjects/Equipment/EquipmentSlotData", order = 1)]
    public class EquipmentSlotData : ScriptableObject
    {
        [field: SerializeField] public int[] GoldUpgradeCostPerLevel { get; private set; }
        [field: SerializeField] public int[] MaterialUpgradeCostPerLevel { get; private set; }
    }
}