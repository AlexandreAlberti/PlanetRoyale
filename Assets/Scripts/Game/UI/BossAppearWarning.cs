using System.Collections;
using Application.Utils;
using DG.Tweening;
using Game.Enabler;
using UnityEngine;

namespace Game.UI
{
    public class BossAppearWarning : EnablerMonoBehaviour
    {
        [SerializeField] private RectTransform _topText;
        [SerializeField] private RectTransform _bottomText;
        [SerializeField] private RectTransform _iLetter;
        [SerializeField] private RectTransform _nLetter;
        [SerializeField] private RectTransform _cLetter;
        [SerializeField] private RectTransform _oLetter;
        [SerializeField] private RectTransform _mLetter;
        [SerializeField] private RectTransform _i2Letter;
        [SerializeField] private RectTransform _n2Letter;
        [SerializeField] private RectTransform _gLetter;
        [SerializeField] private float _topAndBottomTextMovementSpeed;
        [SerializeField] private float _topAndBottomTextWidthOffsetPercentage;
        [SerializeField] private float _dangerTextDelayBetweenLetters;
        [SerializeField] private float _dangerTextUpScalePercentage;
        [SerializeField] private float _dangerTextUpScaleDuration;
        [SerializeField] private float _dangerTextDownScaleDuration;

        private float _textWidth;

        public void Initialize()
        {
            _textWidth = _topText.rect.width;
            _topText.anchoredPosition = new Vector2(-_textWidth * _topAndBottomTextWidthOffsetPercentage, NumberConstants.Zero);
            _bottomText.anchoredPosition = new Vector2(_textWidth * _topAndBottomTextWidthOffsetPercentage, NumberConstants.Zero);
            _iLetter.transform.localScale = Vector3.zero;
            _nLetter.transform.localScale = Vector3.zero;
            _cLetter.transform.localScale = Vector3.zero;
            _oLetter.transform.localScale = Vector3.zero;
            _mLetter.transform.localScale = Vector3.zero;
            _i2Letter.transform.localScale = Vector3.zero;
            _n2Letter.transform.localScale = Vector3.zero;
            _gLetter.transform.localScale = Vector3.zero;
            Enable();
            StartCoroutine(ShowDangerLabel());
        }

        private IEnumerator ShowDangerLabel()
        {
            Vector3 upScale = Vector3.one * _dangerTextUpScalePercentage;
            yield return new WaitForSeconds(_dangerTextDelayBetweenLetters);
            UpScale(_iLetter, upScale, Vector3.one);
            yield return new WaitForSeconds(_dangerTextDelayBetweenLetters);
            UpScale(_nLetter, upScale, Vector3.one);
            yield return new WaitForSeconds(_dangerTextDelayBetweenLetters);
            UpScale(_cLetter, upScale, Vector3.one);
            yield return new WaitForSeconds(_dangerTextDelayBetweenLetters);
            UpScale(_oLetter, upScale, Vector3.one);
            yield return new WaitForSeconds(_dangerTextDelayBetweenLetters);
            UpScale(_mLetter, upScale, Vector3.one);
            yield return new WaitForSeconds(_dangerTextDelayBetweenLetters);
            UpScale(_i2Letter, upScale, Vector3.one);
            yield return new WaitForSeconds(_dangerTextDelayBetweenLetters);
            UpScale(_n2Letter, upScale, Vector3.one);
            yield return new WaitForSeconds(_dangerTextDelayBetweenLetters);
            UpScale(_gLetter, upScale, Vector3.one);
        }

        private void UpScale(RectTransform letter, Vector3 upScale, Vector3 downScale)
        {
            letter.DOScale(upScale, _dangerTextUpScaleDuration).SetEase(Ease.InOutQuad).OnComplete(() => DownScale(letter, downScale));
        }

        private void DownScale(RectTransform letter, Vector3 downScale)
        {
            letter.DOScale(downScale, _dangerTextDownScaleDuration).SetEase(Ease.InOutQuad);
        }

        private void Update()
        {
            if (!_isEnabled)
            {
                return;
            }

            float move = _topAndBottomTextMovementSpeed * Time.deltaTime;
            _topText.anchoredPosition += new Vector2(move, NumberConstants.Zero);
            _bottomText.anchoredPosition -= new Vector2(move, NumberConstants.Zero);
        }
    }
}