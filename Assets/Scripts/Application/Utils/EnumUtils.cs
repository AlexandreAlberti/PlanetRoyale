using System;
using Application.Data.Equipment;
using Application.Telemetry;

namespace Application.Utils
{
    public static class EnumUtils
    {
        public static Array GetValues<T>()
        {
            return Enum.GetValues(typeof(T));
        }

        public static SoftCurrencyType SoftCurrencyTypeByPieceType(EquipmentPieceType type)
        {
            switch (type)
            {
                default:
                case EquipmentPieceType.Weapon:
                    return SoftCurrencyType.WeaponMaterial;
                case EquipmentPieceType.Locket:
                    return SoftCurrencyType.LocketMaterial;
                case EquipmentPieceType.Ring:
                    return SoftCurrencyType.RingMaterial;
                case EquipmentPieceType.Helm:
                    return SoftCurrencyType.HelmMaterial;
                case EquipmentPieceType.Armor:
                    return SoftCurrencyType.ArmorMaterial;
                case EquipmentPieceType.Boots:
                    return SoftCurrencyType.BootsMaterial;
            }
        }
    }
}