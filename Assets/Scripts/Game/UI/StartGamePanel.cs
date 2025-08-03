using System;
using System.Collections;
using Application.Utils;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class StartGamePanel : Panel
    {
        [SerializeField] private Image _background;
        [SerializeField] private Transform _rotatingLight;
        [SerializeField] private Transform _text;
        [SerializeField] private float _backgroundFadeDuration;
        [SerializeField] private float _rotatingLightScaleUpDuration;
        [SerializeField] private float _textScaleUpDuration;
        [SerializeField] private float _textScaleDownDuration;
        [SerializeField] private float _timeBeforeTextFinalScaleUp;
        [SerializeField] private float _textFinalScaleUpDuration;
        [SerializeField] private float _timeBeforeEnding;

        private const float BackgroundAlpha = 0.667f;
        private const float TextScaleUpSize = 2.0f;
        private const float TextScaleDownSize = 1.0f;
        private const float TextFinalScaleUpSize = 4.0f;
        private const float RotatingLightScaleUpSize = 1.0f;

        public Action OnEnded;
        
        public override void Show()
        {
            base.Show();
            StartCoroutine(ShowCoroutine());
        }

        private IEnumerator ShowCoroutine()
        {
            _background.color = new Color(NumberConstants.Zero, NumberConstants.Zero, NumberConstants.Zero, NumberConstants.Zero);
            _background.DOFade(BackgroundAlpha, _backgroundFadeDuration);
                
            _rotatingLight.localScale = Vector3.zero;
            _rotatingLight.DOScale(RotatingLightScaleUpSize,_rotatingLightScaleUpDuration);
            
            _text.localScale = Vector3.zero;
            _text.DOScale(TextScaleUpSize,_textScaleUpDuration).OnComplete(() => _text.DOScale(TextScaleDownSize, _textScaleDownDuration));
            
            yield return new WaitForSeconds(_timeBeforeTextFinalScaleUp);
            
            _text.DOScale(TextFinalScaleUpSize,_textScaleUpDuration);
            
            yield return new WaitForSeconds(_timeBeforeEnding);
            
            _background.DOFade(NumberConstants.Zero, _backgroundFadeDuration).OnComplete(() => gameObject.SetActive(false));
            _rotatingLight.localScale = Vector3.zero;
            _text.localScale = Vector3.zero;
            OnEnded?.Invoke();
        }
    }
}