using Game.Equipment;
using UI.Button;

namespace UI.MainMenu
{
    public class EquipPieceButton : InteractableButton
    {
        private EquipmentPieceInstance _equipmentPieceInstance;

        public void Initialize(EquipmentPieceInstance equipmentPieceInstance)
        {
            _equipmentPieceInstance = equipmentPieceInstance;
        }

        public EquipmentPieceInstance EquipmentPieceInstance()
        {
            return _equipmentPieceInstance;
        }
    }
}
