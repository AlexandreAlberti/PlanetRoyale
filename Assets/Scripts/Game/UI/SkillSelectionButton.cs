using System;
using Game.Skills;
using TMPro;
using UI.Button;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    [RequireComponent(typeof(InteractableButton))]
    public class SkillSelectionButton : MonoBehaviour
    {
        [SerializeField] private InteractableButton _interactableButton;
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [SerializeField] private Image _iconSprite;
        
        private PlayerSkillData _skillData;

        public Action<PlayerSkillData> OnSkillSelected;

        public void Initialize(PlayerSkillData skillData)
        {
            _skillData = skillData;
            _interactableButton.Enable();
            _interactableButton.OnClicked += InteractableButton_OnClicked;
            _titleText.text = _skillData.Name;
            _descriptionText.text = _skillData.Description;
            _iconSprite.sprite = _skillData.Sprite;
        }

        private void InteractableButton_OnClicked(InteractableButton button)
        {
            _interactableButton.OnClicked -= InteractableButton_OnClicked;
            OnSkillSelected?.Invoke(_skillData);
        }
    }
}