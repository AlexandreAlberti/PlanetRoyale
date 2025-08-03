using System;
using System.Collections;
using System.Linq;
using Application.Data.Currency;
using Application.Data.Progression;
using Application.Utils;
using TMPro;
using UI.Button;
using UnityEngine;
using UnityEngine.UI;

namespace UI.MainMenu
{
    public class LeagueScreen : MonoBehaviour
    {
        [SerializeField] private InteractableButton _leagueScreenButton;
        [SerializeField] private InteractableButton _arrowUpButton;
        [SerializeField] private InteractableButton _arrowDownButton;
        [SerializeField] private AnimationCurve _returnToCenterCurve;
        [SerializeField] private Transform _leaguesInfoParent;
        [SerializeField] private LeagueScreenLeagueInfo _leaguesInfoPrefab;
        [SerializeField] private RectTransform _currentTrophiesArrow;
        [SerializeField] private TextMeshProUGUI _currentTrophiesAmount;
        [SerializeField] private ScrollRect _scroll;
        [SerializeField] private RectTransform _trophiesFullProgressBar;
        [SerializeField] private Image _trophiesFullProgressBarFiller;

        private const float BottomEmptySpaceHeight = 250;

        public Action OnOkButtonClicked;

        private float _centerPositionToScroll;

        public void Initialize()
        {
            LeagueData[] leaguesData = LeaguesManager.Instance.GetAllLeaguesInfo();
            UpdateLeaguesInfo(leaguesData);

            int currentTrophies = CurrencyDataManager.Instance.CurrentTrophies();
            float trophies = currentTrophies;
            Vector3 arrowOriginalPosition = _currentTrophiesArrow.localPosition;
            Vector3 trophyProgressBarOriginalPosition = _trophiesFullProgressBar.localPosition;
            float spacing = _leaguesInfoParent.GetComponent<VerticalLayoutGroup>().spacing;
            float infoItemHeight = _leaguesInfoPrefab.GetComponent<RectTransform>().rect.height;
            float itemHeight = spacing + infoItemHeight;
            float startingPosition = BottomEmptySpaceHeight + spacing + infoItemHeight * NumberConstants.Half;
            float trophiesArrowPosition = startingPosition;

            _currentTrophiesAmount.text = currentTrophies.ToString();
            _trophiesFullProgressBar.localPosition = new Vector3(trophyProgressBarOriginalPosition.x, trophiesArrowPosition, trophyProgressBarOriginalPosition.z);
            _trophiesFullProgressBar.sizeDelta = new Vector2(_trophiesFullProgressBar.sizeDelta.x, itemHeight * leaguesData.Length);
            float progressBarFillAmount = 0.0f;
            float progressBarFillAmountPerCompletedLeague = 1.0f / leaguesData.Length;

            
            foreach (LeagueData leagueData in leaguesData)
            {
                if (leagueData.MinTrophiesInclusive > currentTrophies)
                {
                    break;
                }

                if (leagueData.MaxTrophiesExclusive < currentTrophies)
                {
                    progressBarFillAmount += progressBarFillAmountPerCompletedLeague;
                    trophiesArrowPosition += itemHeight;
                    continue;
                }

                float percent = (trophies - leagueData.MinTrophiesInclusive) / (leagueData.MaxTrophiesExclusive - leagueData.MinTrophiesInclusive);
                trophiesArrowPosition += itemHeight * percent;
                progressBarFillAmount += progressBarFillAmountPerCompletedLeague * percent;
                break;
            }

            _trophiesFullProgressBarFiller.fillAmount = Mathf.Min(1.0f, progressBarFillAmount);
            _scroll.verticalNormalizedPosition = 0;
            _currentTrophiesArrow.localPosition = new Vector3(arrowOriginalPosition.x, trophiesArrowPosition, arrowOriginalPosition.z);
            ScrollViewPortCenterOnItem();
        }

        private void ScrollViewPortCenterOnItem()
        {
            RectTransform content = _scroll.content;
            RectTransform viewport = _scroll.viewport;
            float contentHeight = content.rect.height;
            float viewportHeight = viewport.rect.height;

            float itemPosY = _currentTrophiesArrow.localPosition.y;
            float itemCenterY = itemPosY + _currentTrophiesArrow.rect.height / 2f;
            float viewportCenterY = viewportHeight / 2f;
            float offset = itemCenterY - viewportCenterY;
            float scrollableHeight = contentHeight - viewportHeight;
            float normalizedPosition = Mathf.Clamp01(offset / scrollableHeight);
            _centerPositionToScroll = normalizedPosition;
        }


        public void OpenLeagueScreen(bool disableSubscriptions = false)
        {
            if (!disableSubscriptions)
            {
                _leagueScreenButton.OnClicked += LeagueScreenBackButton_OnClicked;
                _arrowUpButton.OnClicked += ReCenterButton_OnClicked;
                _arrowDownButton.OnClicked += ReCenterButton_OnClicked;
            }


            _scroll.verticalNormalizedPosition = _centerPositionToScroll;
            _scroll.onValueChanged.AddListener(RectScroll_OnValueChanged);
        }

        private void RectScroll_OnValueChanged(Vector2 value)
        {
            _arrowDownButton.gameObject.SetActive(_scroll.verticalNormalizedPosition - 0.1f > _centerPositionToScroll);
            _arrowUpButton.gameObject.SetActive(_scroll.verticalNormalizedPosition + 0.1f < _centerPositionToScroll);
        }

        private void LeagueScreenBackButton_OnClicked(InteractableButton obj)
        {
            OnOkButtonClicked?.Invoke();
        }

        private void ReCenterButton_OnClicked(InteractableButton obj)
        {
            StartCoroutine(TransitionToCenteredPosition());
        }

        private IEnumerator TransitionToCenteredPosition()
        {
            float init = _scroll.verticalNormalizedPosition;
            float end = _centerPositionToScroll;
            float amountToMove = end - init;
            float animationTimer = 0.0f;
            yield return new WaitForEndOfFrame();

            while (animationTimer <= 1.0f)
            {
                _scroll.verticalNormalizedPosition = init + amountToMove * _returnToCenterCurve.Evaluate(animationTimer);
                animationTimer += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            _scroll.verticalNormalizedPosition = _centerPositionToScroll;
        }

        private void UpdateLeaguesInfo(LeagueData[] leaguesData)
        {
            for (int i = 0; i < leaguesData.Length; i++)
            {
                LeagueData leagueData = leaguesData[i];
                LeagueScreenLeagueInfo info = Instantiate(_leaguesInfoPrefab, _leaguesInfoParent);
                info.Initialize(leagueData, i);
                // Need to Skip the trophies indicator and the top space
                info.transform.SetSiblingIndex(2);
            }
        }
    }
}