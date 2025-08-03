using Application.Data.Equipment;
using Game.Equipment;
using UI.Button;
using UnityEngine;
using UnityEngine.UI;

namespace UI.MainMenu
{
    public class LevelUpPieceButton : InteractableButton
    {
        [SerializeField] private Image _buttonImage;
        [SerializeField] private Color _onColor;
        [SerializeField] private Color _offColor;
        
        private EquipmentPieceType _equipmentPieceType;

        public override void Enable()
        {
            base.Enable();
            _buttonImage.color = _onColor;
        }

        public override void Disable()
        {
            base.Disable();
            _buttonImage.color = _offColor;
        }
        
        public void Initialize(EquipmentPieceType equipmentPieceType, bool hasRequiredGoldAmount, bool hasRequiredMaterialsAmount)
        {
            _equipmentPieceType = equipmentPieceType;

            if (EquipmentDataManager.Instance.IsSlotAtMaxLevel(equipmentPieceType) || !hasRequiredGoldAmount || ! hasRequiredMaterialsAmount)
            {
                Disable();
            }
            else
            {
                Enable();
            }
        }

        public EquipmentPieceType GetEquipmentPieceType()
        {
            return _equipmentPieceType;
        }
    }
}
