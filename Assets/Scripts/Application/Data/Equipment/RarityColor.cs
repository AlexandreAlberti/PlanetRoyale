using UnityEngine;

namespace Application.Data.Equipment
{
    public class RarityColor : MonoBehaviour
    {
        [SerializeField] private Color[] _rarityColors;
        
        public static RarityColor Instance;

        private void Awake()
        {
            Instance = this;
        }

        public Color GetColorByRarity(EquipmentPieceRarity rarity)
        {
            return _rarityColors[(int)rarity];
        }
    }
}