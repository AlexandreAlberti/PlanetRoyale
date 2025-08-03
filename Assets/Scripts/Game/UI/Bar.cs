using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class Bar : MonoBehaviour
    {
        [SerializeField] protected GameObject _bar;
        [SerializeField] protected Image _barFill;
        [SerializeField] protected Image _whiteFill;
        [SerializeField] protected ColorPercentage[] _barColors;

        private const float FullFillAmount = 1.0f;
        private const float WhiteFillDuration = 0.5f;

        private float _whiteFillAmount;
        private float _currentWhiteFillTime;
        private float _targetWhiteFillAmount;

        protected void Initialize()
        {
            _barFill.fillAmount = FullFillAmount;
            _whiteFill.fillAmount = FullFillAmount;
            SetHealthBarColor(FullFillAmount);
            _bar.SetActive(true);
        }
        
        protected void FillBarPercentage(float barPercentage)
        {
            SetHealthBarColor(barPercentage);
            _barFill.fillAmount = barPercentage;
        }

        protected void EmptyBarPercentage(float healthPercentage)
        {
            SetHealthBarColor(healthPercentage);

            _whiteFill.fillAmount = _barFill.fillAmount;
            _barFill.fillAmount = healthPercentage;

            if (!isActiveAndEnabled)
            {
                _whiteFill.fillAmount = healthPercentage;
                return;
            }
            
            StartCoroutine(WhiteFillEffect());
        }

        private void SetHealthBarColor(float healthPercentage)
        {
            foreach (ColorPercentage colorPercentage in _barColors)
            {
                if (healthPercentage >= colorPercentage.Threshold)
                {
                    _barFill.color = colorPercentage.Color;
                    break;
                }
            }
        }

        private IEnumerator WhiteFillEffect()
        {
            yield return new WaitForSeconds(WhiteFillDuration);

            _targetWhiteFillAmount = _barFill.fillAmount;
            float startingValue = _whiteFill.fillAmount;
            _currentWhiteFillTime = 0.0f;

            while (_whiteFill.fillAmount > _targetWhiteFillAmount)
            {
                _currentWhiteFillTime += Time.deltaTime;
                _whiteFill.fillAmount = Mathf.Lerp(startingValue, _targetWhiteFillAmount,
                    _currentWhiteFillTime / WhiteFillDuration);
                yield return null;
            }

            _whiteFill.fillAmount = _targetWhiteFillAmount;
        }
    }
}
