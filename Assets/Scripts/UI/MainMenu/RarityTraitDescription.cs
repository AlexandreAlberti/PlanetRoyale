using System;
using Application.Data.Equipment;
using TMPro;
using UnityEngine;

namespace UI.MainMenu
{
    public class RarityTraitDescription : MonoBehaviour
    {
        [SerializeField] private Color _enabledRarityTraitColor;
        [SerializeField] private Color _disabledRarityTraitColor;
        [SerializeField] private GameObject _lockIcon;
        [SerializeField] private TextMeshProUGUI _text;

        public void Initialize(EquipmentBoostData equipmentBoostData)
        {
            _text.text = String.Format(equipmentBoostData.EquipmentBoostDescription, equipmentBoostData.EquipmentBoostAmount);
            _lockIcon.SetActive(false);
            _text.color = _enabledRarityTraitColor;
        }

        public void Lock()
        {
            _lockIcon.SetActive(true);
            _text.color = _disabledRarityTraitColor;
        }
    }
}
