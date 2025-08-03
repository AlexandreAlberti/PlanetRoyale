using System;
using UnityEngine;

namespace Application.Utils
{
    public class CountDown : MonoBehaviour
    {
        private const float Second = 1.0f;
        
        private bool _isEnabled;
        private float _remainingTime;
        private float _totalTime;

        public Action OnSecondPassed;
        public Action OnCountDownEnded;

        public void SetTotalTime(float totalTime)
        {
            _totalTime = totalTime;
            _remainingTime = totalTime;
        }

        public void StartCounting()
        {
            _isEnabled = true;
        }

        public void StopCounting()
        {
            _isEnabled = false;
        }
        
        public float GetRemainingTime()
        {
            return _remainingTime;
        }

        private void Update()
        {
            if (!_isEnabled)
            {
                return;
            }
            
            _remainingTime -= Time.deltaTime;

            if (_totalTime - _remainingTime >= Second)
            {
                _totalTime -= Second;
                OnSecondPassed?.Invoke();
            }
            
            if (_remainingTime <= NumberConstants.Zero)
            {
                _isEnabled = false;
                OnCountDownEnded?.Invoke();
            }
        }
    }
}