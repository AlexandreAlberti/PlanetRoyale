using TMPro;
using UI.Button;
using UnityEngine;

namespace UI.MainMenu
{
    public class EquipmentSortButton : InteractableButton
    {
        [SerializeField] private TextMeshProUGUI _text;

        private const string ByText = "By";
        private const string SlotText = "Slot";
        private const string QualityText = "Quality";
        
        public void ChangeToSlotText()
        {
            _text.text = $"{ByText} {SlotText}";
        }
        
        public void ChangeToQualityText()
        {
            _text.text = $"{ByText} {QualityText}";
        }
    }
}
