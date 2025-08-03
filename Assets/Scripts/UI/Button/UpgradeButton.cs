using Application.Sound;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Button
{
    public class UpgradeButton : InteractableButton
    {
        [SerializeField] private Image _buttonImage;
        [SerializeField] private Color _onColor;
        [SerializeField] private Color _offColor;
        
        private void Awake()
        {
            transform.localScale = Vector3.one;
        }
        
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
        
        protected override void OnClick()
        {
            if (!_isEnabled)
            {
                return;
            }
            
            if (_playSounds)
            {
                SoundManager.Instance.PlayMenuPositiveButtonSound();
            }

            if (_animateClick)
            {
                transform.DOScale(new Vector3(_normalScale + _normalScale * _scaleFactor, _normalScale + _normalScale * _scaleFactor), _scaleChangeDuration).SetUpdate(true).onComplete = OnButtonUp;
            }

            Disable();
            OnClicked?.Invoke(this);
        }

        public bool IsEnabled()
        {
            return _isEnabled;
        }
    }
}
