using System;
using System.Collections;
using Application.Data.Progression;
using Application.Utils;
using UnityEngine;

namespace UI.MainMenu
{
    public class LeagueUpCelebrationPanel : Popup
    {
        [SerializeField] private Animator _contentAnimator;
        [SerializeField] private LeagueRewards _currentLeagueRewards;
        [SerializeField] private LeagueRewards _nextLeagueRewards;
        
        private const float WaitBeforeTriggerAnimation = 1.5f;
    
        private static readonly int LeagueChange = Animator.StringToHash("LeagueChange");

        public Action OnLeagueUpCelebrationPanelPopupClosed;
    
        public void Initialize(LeagueData currentLeagueData, LeagueData nextLeagueData)
        {
            Show();
            StartCoroutine(WaitAFrameAandInitialize(currentLeagueData, nextLeagueData));
        }

        private IEnumerator WaitAFrameAandInitialize(LeagueData currentLeagueData, LeagueData nextLeagueData)
        {
            yield return new WaitForEndOfFrame();
            _currentLeagueRewards.Initialize(currentLeagueData);
            _nextLeagueRewards.Initialize(nextLeagueData);
            StartCoroutine(WaitAndTriggerCurrentReward());
            StartCoroutine(WaitAndTriggerChangeLeague());
        }

        private IEnumerator WaitAndTriggerCurrentReward()
        {
            yield return new WaitForSeconds(NumberConstants.Half);
            _currentLeagueRewards.Appear();
        }

        private IEnumerator WaitAndTriggerChangeLeague()
        {
            yield return new WaitForSeconds(WaitBeforeTriggerAnimation);
            _nextLeagueRewards.Appear();
            _contentAnimator.SetTrigger(LeagueChange);
        }

        protected override void UnsubscribeButtons()
        {
            base.UnsubscribeButtons();
            OnLeagueUpCelebrationPanelPopupClosed?.Invoke();
        }
    }
}
