using System;
using Application.Data.Equipment;

namespace Game.Equipment
{
    [Serializable]
    public class EquipmentPieceInstance
    {
        public int InstanceId;
        public int Id;
        public bool IsNew;
        public bool IsEquipped;
        public int Rarity;

        public EquipmentPieceInstance()
        {
            InstanceId = -1;
            Id = -1;
            IsNew = false;
            IsEquipped = false;
            Rarity = (int) EquipmentPieceRarity.Common;
        }
        
        public EquipmentPieceInstance(int id, int rarity = (int) EquipmentPieceRarity.Common)
        {
            InstanceId = new Random().Next(0, int.MaxValue);
            Id = id;
            IsNew = true;
            IsEquipped = false;
            Rarity = rarity;
        }
        
        public void UpgradeRarity()
        {
            Rarity++;

            if ((EquipmentPieceRarity) Rarity > EquipmentPieceRarity.Legendary)
            {
                Rarity = (int) EquipmentPieceRarity.Legendary;
            }
        }

        public void Equip()
        {
            IsEquipped = true;
        }

        public void UnEquip()
        {
            IsEquipped = false;
        }
    }
}
