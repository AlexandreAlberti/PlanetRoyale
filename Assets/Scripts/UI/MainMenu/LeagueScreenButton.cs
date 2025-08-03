using System;
using System.Collections;
using Application.Data.Currency;
using Application.Data.Progression;
using TMPro;
using UI.Button;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.UI;

namespace UI.MainMenu
{
    public class LeagueScreenButton : InteractableButton
    {
        [SerializeField] private TextMeshProUGUI _arena;
        [SerializeField] private TextMeshProUGUI _trophiesObtainedText;
        [SerializeField] private Image _trophyCompletionBar;
        [SerializeField] private float _trophyCompletionBarFillDuration;
        [SerializeField] private LeagueRewards _leagueRewards;

        public static Action OnTrophiesWonChoreographyEnded;
        
        public void Initialize()
        {
            PerformTrophiesWonChoreography();
        }
        
        public void PerformTrophiesWonChoreography()
        {
            int currentTrophies = CurrencyDataManager.Instance.CurrentTrophies();
            int trophiesWonAmount = RewardUIParticleManager.Instance.TrophiesWonAmount();
            int previousTrophies = currentTrophies - trophiesWonAmount;
            int currentLeagueIndex = LeaguesManager.Instance.GetCurrentLeagueIndex();
            LeagueData leagueData = LeaguesManager.Instance.GetLeagueInfo(currentLeagueIndex);
            bool leagueHasChanged = previousTrophies < leagueData.MinTrophiesInclusive || previousTrophies >= leagueData.MaxTrophiesExclusive;
            float currentBarProgress = (float)(currentTrophies - leagueData.MinTrophiesInclusive) / (leagueData.MaxTrophiesExclusive - leagueData.MinTrophiesInclusive);
            
            if (trophiesWonAmount < 0)
            {
                int nextLeagueIndex = currentLeagueIndex + 1;
                LeagueData nextLeagueData = leagueHasChanged ? LeaguesManager.Instance.GetLeagueInfo(nextLeagueIndex) : leagueData;
                _arena.text = nextLeagueData.LeagueTitle;
                _trophiesObtainedText.text = $"{currentTrophies}/{nextLeagueData.MaxTrophiesExclusive}";
                _trophyCompletionBar.fillAmount = (float)(previousTrophies - nextLeagueData.MinTrophiesInclusive) / (nextLeagueData.MaxTrophiesExclusive - nextLeagueData.MinTrophiesInclusive);
                _leagueRewards.Initialize(nextLeagueData);
                StartCoroutine(SubtractFillEffect(currentBarProgress, leagueData, currentTrophies));
            }
            else if (trophiesWonAmount > 0)
            {
                int previousLeagueIndex = Math.Max(0, currentLeagueIndex - 1);
                LeagueData previousLeagueData = leagueHasChanged ? LeaguesManager.Instance.GetLeagueInfo(previousLeagueIndex) : leagueData;
                _arena.text = previousLeagueData.LeagueTitle;
                _trophiesObtainedText.text = $"{currentTrophies}/{previousLeagueData.MaxTrophiesExclusive}";
                _trophyCompletionBar.fillAmount = (float)(previousTrophies - previousLeagueData.MinTrophiesInclusive) / (previousLeagueData.MaxTrophiesExclusive - previousLeagueData.MinTrophiesInclusive);
                _leagueRewards.Initialize(previousLeagueData);
                StartCoroutine(AddFillEffect(currentBarProgress, leagueData, currentTrophies));
            }
            else
            {
                SetCurrentLeagueData(leagueData, currentTrophies);
                StartCoroutine(WaitAndThrowChoreographyEnded());
            }
        }

        private IEnumerator WaitAndThrowChoreographyEnded()
        {
            yield return new WaitForSeconds(MenuManager.CreateParticlesInitialDelay);
            OnTrophiesWonChoreographyEnded?.Invoke();
        }
        
        private void SetCurrentLeagueData(LeagueData leagueData, int currentTrophies)
        {
            _arena.text = leagueData.LeagueTitle;
            _trophiesObtainedText.text = $"{currentTrophies}/{leagueData.MaxTrophiesExclusive}";
            _trophyCompletionBar.fillAmount = (float)(currentTrophies - leagueData.MinTrophiesInclusive) / (leagueData.MaxTrophiesExclusive - leagueData.MinTrophiesInclusive);
            _leagueRewards.Initialize(leagueData);
        }
        
        private IEnumerator AddFillEffect(float currentValue, LeagueData leagueData, int currentTrophies)
        {
            yield return new WaitForSeconds(MenuManager.CreateParticlesInitialDelay + RewardUIParticleManager.Instance.TrophiesDelayBeforeReachingTarget());
            float targetFillAmount = currentValue;
            float startingValue = _trophyCompletionBar.fillAmount;
            float currentFillTime = 0.0f;

            if (targetFillAmount <= startingValue)
            {
                targetFillAmount++;
            }

            float imageFillerFillAmount = _trophyCompletionBar.fillAmount;
            bool changeLeagueTriggered = false;
            
            while (imageFillerFillAmount < targetFillAmount)
            {
                currentFillTime += Time.deltaTime;
                imageFillerFillAmount = Mathf.Lerp(startingValue, targetFillAmount, currentFillTime / _trophyCompletionBarFillDuration);
                _trophyCompletionBar.fillAmount = imageFillerFillAmount >= 1.0f ? imageFillerFillAmount - 1.0f : imageFillerFillAmount;

                if (!changeLeagueTriggered && imageFillerFillAmount >= 1.0f)
                {
                    changeLeagueTriggered = true;
                    SetCurrentLeagueData(leagueData, currentTrophies);
                }
                
                yield return null;
            }
            
            OnTrophiesWonChoreographyEnded?.Invoke();
        }

        private IEnumerator SubtractFillEffect(float currentValue, LeagueData leagueData, int currentTrophies)
        {
            yield return new WaitForSeconds(MenuManager.CreateParticlesInitialDelay + RewardUIParticleManager.Instance.TrophiesDelayBeforeReachingTarget());
            float targetFillAmount = currentValue;
            float startingValue = _trophyCompletionBar.fillAmount;
            float currentFillTime = 0.0f;

            if (targetFillAmount >= startingValue)
            {
                targetFillAmount--;
            }

            float imageFillerFillAmount = _trophyCompletionBar.fillAmount;
            bool changeLeagueTriggered = false;

            while (imageFillerFillAmount > targetFillAmount)
            {
                currentFillTime += Time.deltaTime;
                imageFillerFillAmount = Mathf.Lerp(startingValue, targetFillAmount, currentFillTime / _trophyCompletionBarFillDuration);
                _trophyCompletionBar.fillAmount = imageFillerFillAmount < 0.0f ? imageFillerFillAmount + 1.0f : imageFillerFillAmount;
                
                if (!changeLeagueTriggered && imageFillerFillAmount < 0.0f)
                {
                    changeLeagueTriggered = true;
                    SetCurrentLeagueData(leagueData, currentTrophies);
                }

                yield return null;
            }
            
            OnTrophiesWonChoreographyEnded?.Invoke();
        }
    }
}