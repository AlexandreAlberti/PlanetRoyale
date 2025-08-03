using System.Collections;
using Game.Enabler;
using Game.PlayerUnit;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class XpBar : EnablerMonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private Image _imageFiller;
        [SerializeField] private float _fillDuration;

        private Coroutine _coroutineFillEffect;

        public void Initialize()
        {
            _imageFiller.fillAmount = 1;
            _levelText.text = "1";
            Player player = PlayerManager.Instance.GetHumanPlayer();
            player.OnXpGained += Player_OnXpGained;
            player.OnLevelUp += Player_OnLevelUp;
        }

        private void Player_OnLevelUp(PlayerLevel.OnLevelUpEventArgs args)
        {
            _levelText.text = (1 + args.CurrentLevel).ToString();
            UpdateBar(args.CurrentLevelXp, args.NextLevelThreshold);
        }

        private void Player_OnXpGained(PlayerLevel.OnXpGainedEventArgs args, int playerIndex)
        {
            UpdateBar(args.CurrentLevelXp, args.NextLevelThreshold);
        }

        private void UpdateBar(float currentValue, float maxValue)
        {
            if (_coroutineFillEffect != null)
            {
                StopCoroutine(_coroutineFillEffect);
            }

            _coroutineFillEffect = StartCoroutine(FillEffect(currentValue, maxValue));
        }

        private IEnumerator FillEffect(float currentValue, float maxValue)
        {
            float targetFillAmount = currentValue / maxValue;
            float startingValue = _imageFiller.fillAmount;
            float currentFillTime = 0.0f;

            if (targetFillAmount < startingValue)
            {
                targetFillAmount++;
            }

            while (_imageFiller.fillAmount < targetFillAmount)
            {
                currentFillTime += Time.deltaTime;
                float imageFillerFillAmount = Mathf.Lerp(startingValue, targetFillAmount, currentFillTime / _fillDuration);
                _imageFiller.fillAmount = imageFillerFillAmount >= 1.0f ? imageFillerFillAmount - 1.0f : imageFillerFillAmount;
                yield return null;
            }

            _imageFiller.fillAmount = targetFillAmount;
            _coroutineFillEffect = null;
        }
    }
}