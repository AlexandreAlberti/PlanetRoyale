using System;
using UnityEngine;

namespace Application.Data.Equipment
{
    [Serializable]
    [CreateAssetMenu(fileName = "equipmentBoost_data", menuName = "ScriptableObjects/Equipment/EquipmentBoostData", order = 2)]
    public class EquipmentBoostData : ScriptableObject
    {
        [field: SerializeField] public EquipmentBoostType EquipmentBoostType { get; private set; }
        [field: SerializeField] public string EquipmentBoostDescription { get; set; }
        [field: SerializeField] public int EquipmentBoostAmount { get; private set; }
    }
}