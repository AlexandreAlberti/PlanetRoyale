using System;
using TMPro;
using UnityEngine;

namespace Game.UI
{
    public class CountDownPanel : Panel
    {
        [SerializeField] private TextMeshProUGUI _roundTimeLeft;
        [SerializeField] private Animator _timerAnimator;
        [SerializeField] private int _timerStartBeepingOnRemainingSeconds;
        [SerializeField] private int _timerBeepCooldown;
        [SerializeField] private int _timerStartAlarmingOnRemainingSeconds;
        [SerializeField] private int _timerAlarmCooldown;

        private static readonly int Beep = Animator.StringToHash("Beep");
        private static readonly int Alarm = Animator.StringToHash("Alarm");

        private int _lastSecondBeep;

        private void Awake()
        {
            _lastSecondBeep = Mathf.CeilToInt(_timerStartBeepingOnRemainingSeconds) + 1;
        }

        public void UpdateTime(float time)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(Mathf.CeilToInt(time));
            _roundTimeLeft.text = $"{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";

            if (time < _timerStartAlarmingOnRemainingSeconds)
            {
                AlarmAction(Mathf.FloorToInt(time));
            }
            else if (time < _timerStartBeepingOnRemainingSeconds)
            {
                BeepAction(Mathf.FloorToInt(time));
            }
        }

        private void BeepAction(int beepSecond)
        {
            if (beepSecond % _timerBeepCooldown != 0 || beepSecond == _lastSecondBeep)
            {
                return;
            }

            _lastSecondBeep = beepSecond;
            _timerAnimator.SetTrigger(Beep);
        }

        private void AlarmAction(int beepSecond)
        {
            if (beepSecond % _timerAlarmCooldown != 0 || beepSecond == _lastSecondBeep)
            {
                return;
            }

            _lastSecondBeep = beepSecond;
            _timerAnimator.SetTrigger(Alarm);
        }
    }
}