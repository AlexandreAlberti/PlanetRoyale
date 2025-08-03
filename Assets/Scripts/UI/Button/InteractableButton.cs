using System;
using Application.Sound;
using DG.Tweening;
using UnityEngine;

namespace UI.Button
{
    [RequireComponent(typeof(UnityEngine.UI.Button))]
    [RequireComponent(typeof(PointerListener))]
    public class InteractableButton : MonoBehaviour
    {
        [SerializeField] protected UnityEngine.UI.Button _button;
        [SerializeField] protected PointerListener _pointerListener;
        [SerializeField] protected CanvasGroup _canvasGroup;
        [Header("Sound")]
        [SerializeField] protected bool _playSounds = true;
        [Header("Animation")]
        [SerializeField] protected bool _animateClick = true;
        [SerializeField] protected float _normalScale;
        [SerializeField] protected float _scaleFactor;
        [SerializeField] protected float _scaleChangeDuration;
        [SerializeField] protected float _appearScaleFactor;
        [SerializeField] protected float _appearScaleChangeDuration;
        [SerializeField] protected float _appearFadeDuration;
        [SerializeField] protected bool _isEnabled;
        
        public Action<InteractableButton> OnClicked;

        private void Awake()
        {
            Enable();
        }

        public virtual void OnButtonUp()
        {
            if (!_isEnabled)
            {
                return;
            }

            if (_animateClick)
            {
                transform.DOScale(new Vector3(_normalScale, _normalScale), _scaleChangeDuration).SetUpdate(true);
            }
        }

        protected virtual void OnButtonDown()
        {
            if (!_isEnabled)
            {
                return;
            }

            if (_animateClick)
            {
                float scale = _normalScale - _normalScale * _scaleFactor;
                transform.DOScale(new Vector3(scale, scale), _scaleChangeDuration).SetUpdate(true);
            }
        }

        protected virtual void OnClick()
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

            OnClicked?.Invoke(this);
        }

        public virtual void Enable()
        {
            if (_isEnabled)
            {
                return;
            }

            _button.interactable = true;
            _button.onClick.AddListener(OnClick);
            _pointerListener.OnButtonDown += OnButtonDown;
            _pointerListener.OnButtonUp += OnButtonUp;
            _isEnabled = true;
        }

        public virtual void Disable()
        {
            if (!_isEnabled)
            {
                return;
            }

            _button.interactable = false;
            _button.onClick.RemoveListener(OnClick);
            _pointerListener.OnButtonDown -= OnButtonDown;
            _pointerListener.OnButtonUp -= OnButtonUp;
            _isEnabled = false;
        }
        
        public void Appear()
        {
            transform.localScale = new Vector3(_normalScale * _appearScaleFactor, _normalScale * _appearScaleFactor);
            transform.DOScale(new Vector3(_normalScale - _normalScale * _scaleFactor, _normalScale - _normalScale * _scaleFactor), _appearScaleChangeDuration).SetUpdate(true).onComplete = OnButtonUp;
            _canvasGroup.alpha = 0;
            _canvasGroup.DOFade(1, _appearFadeDuration);
        }
    }
}