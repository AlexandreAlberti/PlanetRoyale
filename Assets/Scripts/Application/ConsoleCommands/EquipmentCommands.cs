using System.Collections.Generic;
using Application.Data.Currency;
using Application.Data.Equipment;
using Game.Equipment;
using MobileConsole;
using UnityEngine;

namespace Application.ConsoleCommands
{
    public class ECommandsquipment : MonoBehaviour
    {
        [ExecutableCommand(name = "Equipment/Add 10 Materials of each")]
        public class AddMaterials10Command : Command
        {
            public override void Execute()
            {
                CurrencyDataManager.Instance.GainMaterials(EquipmentPieceType.Weapon, 10);
                CurrencyDataManager.Instance.GainMaterials(EquipmentPieceType.Locket, 10);
                CurrencyDataManager.Instance.GainMaterials(EquipmentPieceType.Ring, 10);
                CurrencyDataManager.Instance.GainMaterials(EquipmentPieceType.Helm, 10);
                CurrencyDataManager.Instance.GainMaterials(EquipmentPieceType.Armor, 10);
                CurrencyDataManager.Instance.GainMaterials(EquipmentPieceType.Boots, 10);
            }
        }

        [ExecutableCommand(name = "Equipment/Add Random Equipment Piece")]
        public class AddRandomEquipmentPieceCommand : Command
        {
            public override void Execute()
            {
                EquipmentDataManager.Instance.AddPieceToInventory(EquipmentPieceFactory.GenerateRandomEquipmentPiece());
            }
        }

        [ExecutableCommand(name = "Equipment/Add Merge Test Equipment Pieces")]
        public class AddMergeTestEquipmentPiecesCommand : Command
        {
            public override void Execute()
            {
                EquipmentDataManager.Instance.AddPieceListToInventory(new List<EquipmentPieceInstance>
                {
                    EquipmentPieceFactory.GenerateEquipmentPieceById(0),
                    EquipmentPieceFactory.GenerateEquipmentPieceById(0),
                    EquipmentPieceFactory.GenerateEquipmentPieceById(0),
                    EquipmentPieceFactory.GenerateEquipmentPieceById(0),
                    EquipmentPieceFactory.GenerateEquipmentPieceById(0),
                    EquipmentPieceFactory.GenerateEquipmentPieceById(0),
                    EquipmentPieceFactory.GenerateEquipmentPieceById(0),
                    EquipmentPieceFactory.GenerateEquipmentPieceById(0),
                    EquipmentPieceFactory.GenerateEquipmentPieceById(0),
                    EquipmentPieceFactory.GenerateEquipmentPieceById(500),
                    EquipmentPieceFactory.GenerateEquipmentPieceById(500),
                    EquipmentPieceFactory.GenerateEquipmentPieceById(500),
                    EquipmentPieceFactory.GenerateEquipmentPieceById(500),
                    EquipmentPieceFactory.GenerateEquipmentPieceById(500),
                    EquipmentPieceFactory.GenerateEquipmentPieceById(500),
                    EquipmentPieceFactory.GenerateEquipmentPieceById(500),
                    EquipmentPieceFactory.GenerateEquipmentPieceById(500),
                    EquipmentPieceFactory.GenerateEquipmentPieceById(500),
                });
            }
        }

        [ExecutableCommand(name = "Equipment/Add 1 equipment of each type")]
        public class AddEquipmentOneOfEachType : Command
        {
            public override void Execute()
            {
                EquipmentDataManager.Instance.AddPieceListToInventory(new List<EquipmentPieceInstance>
                {
                    EquipmentPieceFactory.GenerateEquipmentPieceById(0),
                    EquipmentPieceFactory.GenerateEquipmentPieceById(1),
                    EquipmentPieceFactory.GenerateEquipmentPieceById(2),
                    EquipmentPieceFactory.GenerateEquipmentPieceById(3),
                    EquipmentPieceFactory.GenerateEquipmentPieceById(4),
                    EquipmentPieceFactory.GenerateEquipmentPieceById(100),
                    EquipmentPieceFactory.GenerateEquipmentPieceById(101),
                    EquipmentPieceFactory.GenerateEquipmentPieceById(102),
                    EquipmentPieceFactory.GenerateEquipmentPieceById(200),
                    EquipmentPieceFactory.GenerateEquipmentPieceById(201),
                    EquipmentPieceFactory.GenerateEquipmentPieceById(202),
                    EquipmentPieceFactory.GenerateEquipmentPieceById(300),
                    EquipmentPieceFactory.GenerateEquipmentPieceById(301),
                    EquipmentPieceFactory.GenerateEquipmentPieceById(302),
                    EquipmentPieceFactory.GenerateEquipmentPieceById(400),
                    EquipmentPieceFactory.GenerateEquipmentPieceById(401),
                    EquipmentPieceFactory.GenerateEquipmentPieceById(402),
                    EquipmentPieceFactory.GenerateEquipmentPieceById(500),
                    EquipmentPieceFactory.GenerateEquipmentPieceById(501),
                    EquipmentPieceFactory.GenerateEquipmentPieceById(502)
                });
            }
        }
        
        [ExecutableCommand(name = "Equipment/Upgrade weapon slot to level 10")]
        public class UpgradeSlotLevel10Command : Command
        {
            public override void Execute()
            {
                for (int i = 1; i < 10; i++)
                {
                    EquipmentDataManager.Instance.LevelUpSlot(EquipmentPieceType.Weapon);
                }
            }
        }
    }
}