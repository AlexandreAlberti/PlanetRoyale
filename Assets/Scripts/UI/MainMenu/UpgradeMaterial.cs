using Application.Data.Equipment;
using UnityEngine;
using UnityEngine.UI;

namespace UI.MainMenu
{
    public class UpgradeMaterial : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        
        public void Initialize(EquipmentPieceType equipmentPieceType)
        {
            _icon.sprite = MaterialsSpritesManager.Instance.GetSprite(equipmentPieceType);
        }
    }
}
