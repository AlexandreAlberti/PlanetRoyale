using Application.Data.Equipment;
using UnityEngine;

namespace Game.Equipment
{
    public static class EquipmentPieceTypeFactory
    {
        private const int NumberOfEquipmentPieceTypes = 6;
        
        public static EquipmentPieceType GetRandomEquipmentPieceType()
        {
            int randomIndex = Random.Range(0, NumberOfEquipmentPieceTypes);
            
            switch (randomIndex)
            {
                case 0:
                    return EquipmentPieceType.Weapon;
                case 1:
                    return EquipmentPieceType.Locket;
                case 2:
                    return EquipmentPieceType.Ring;
                case 3:
                    return EquipmentPieceType.Helm;
                case 4:
                    return EquipmentPieceType.Armor;
                case 5:
                    return EquipmentPieceType.Boots;
            }
            
            return EquipmentPieceType.Weapon;
        }
    }
}