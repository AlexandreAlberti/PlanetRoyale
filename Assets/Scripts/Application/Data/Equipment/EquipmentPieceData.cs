using Application.Utils;
using UnityEngine;

namespace Application.Data.Equipment
{
    [CreateAssetMenu(fileName = "equipmentPiece_data", menuName = "ScriptableObjects/Equipment/EquipmentPieceData", order = 1)]
    public class EquipmentPieceData : ScriptableObject
    {
        [field: SerializeField] public int Id { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public Sprite Sprite { get; private set; }
        [field: SerializeField] public EquipmentPieceType EquipmentPieceType { get; private set; }
        [field: SerializeField] public EquipmentBaseStatType EquipmentBaseStatType { get; private set; }
        [field: SerializeField] public int[] BaseStatAmountByLevel { get; private set; }
        [field: SerializeField] public int[] BaseStatMultiplierByRarity { get; private set; }
        [field: SerializeField] public EquipmentBoostData[] EquipmentTraitsPerRarity { get; private set; }

        public int BaseStatAmount(int level, int rarity)
        {
            return Mathf.CeilToInt(BaseStatAmountByLevel[level] + BaseStatAmountByLevel[level] * BaseStatMultiplierByRarity[rarity] / NumberConstants.OneHundred);
        }
        
        public int FlatBaseStatAmount(int level)
        {
            return BaseStatAmountByLevel[level];
        }
    }
}