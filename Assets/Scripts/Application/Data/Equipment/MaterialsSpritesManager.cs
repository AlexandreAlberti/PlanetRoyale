using UnityEngine;

namespace Application.Data.Equipment
{
    public class MaterialsSpritesManager : MonoBehaviour
    {
        [SerializeField] private Sprite[] _equipmentTypeSprites;

        public static MaterialsSpritesManager Instance { get; set; }

        private void Awake()
        {
            Instance = this;
        }

        public Sprite GetSprite(EquipmentPieceType equipmentPieceType)
        {
            return _equipmentTypeSprites[(int)equipmentPieceType];
        }
    }
}