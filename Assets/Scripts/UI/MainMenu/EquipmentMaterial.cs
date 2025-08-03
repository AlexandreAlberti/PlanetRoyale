using Application.Data.Currency;
using Application.Data.Equipment;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.MainMenu
{
    public class EquipmentMaterial : MonoBehaviour
    {
        [SerializeField] private Image _equipmentPieceTypeIcon;
        [SerializeField] private TextMeshProUGUI _materialAmountText;
        [SerializeField] private Animator _animator;

        private const string Times = "x";
        private static readonly int Hidden = Animator.StringToHash("Hidden");
        private static readonly int Normal = Animator.StringToHash("Normal");
        
        public void Initialize(EquipmentPieceType equipmentPieceType, bool isReward = false)
        {
            if (gameObject.activeInHierarchy)
            {
                _animator.Play(isReward ? Hidden : Normal);
            }
            
            _equipmentPieceTypeIcon.sprite = MaterialsSpritesManager.Instance.GetSprite(equipmentPieceType);
            UpdateAmount(CurrencyDataManager.Instance.CurrentMaterials(equipmentPieceType));
        }

        public void UpdateAmount(int amount)
        {
            _materialAmountText.text = $"{Times} {amount}";
        }
    }
}
