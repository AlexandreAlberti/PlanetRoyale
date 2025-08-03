using UnityEngine;
using UnityEngine.UI;

namespace UI.Button
{
    public class SettingButton : InteractableButton
    {
        [SerializeField] private Image _buttonImage;
        [SerializeField] private Color _onColor;
        [SerializeField] private Color _offColor;

        protected bool _isOn;
    
        protected virtual void Start()
        {
            UpdateSettingButtonVisuals();
        }

        protected override void OnClick()
        {
            base.OnClick();
            _isOn = !_isOn;
            UpdateSettingButtonVisuals();
        }

        private void UpdateSettingButtonVisuals()
        {
            _buttonImage.color = _isOn ? _onColor : _offColor;
        }
    }
}
