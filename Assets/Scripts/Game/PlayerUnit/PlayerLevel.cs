using System;
using System.Linq;
using Application.Data;
using Game.Enabler;

namespace Game.PlayerUnit
{
    public class PlayerLevel : EnablerMonoBehaviour
    {
        private PlayerLevelData _playerLevelData;
        private int _currentLevel;
        private float _currentXp;
        private float _totalXp;

        public Action<OnLevelUpEventArgs> OnLevelUp { get; internal set; }
        public Action<OnXpGainedEventArgs> OnXpGained { get; internal set; }

        public class OnLevelUpEventArgs : EventArgs
        {
            public float CurrentLevelXp { get; internal set; }
            public int NextLevelThreshold { get; internal set; }
            public int CurrentLevel { get; internal set; }
        }

        public class OnXpGainedEventArgs : EventArgs
        {
            public float GainedXp { get; internal set; }
            public int NextLevelThreshold { get; internal set; }
            public float CurrentLevelXp { get; internal set; }
            public float TotalXp { get; internal set; }
        }

        public void Initialize(PlayerLevelData playerLevelData)
        {
            _playerLevelData = playerLevelData;
            _currentLevel = 0;
            _currentXp = 0;
            _totalXp = 0;
            Enable();
        }

        public void GainXp(float amount)
        {
            if (!_isEnabled)
            {
                return;
            }

            _currentXp += amount;
            _totalXp += amount;

            int nextLevelThreshold = NextLevelThreshold();
            
            OnXpGained?.Invoke(new OnXpGainedEventArgs
            {
                GainedXp = amount,
                CurrentLevelXp = _currentXp,
                TotalXp = _totalXp,
                NextLevelThreshold = nextLevelThreshold
            });

            while (_currentXp >= nextLevelThreshold)
            {
                _currentXp -= nextLevelThreshold;
                _currentLevel++;

                OnLevelUp?.Invoke(new OnLevelUpEventArgs
                {
                    CurrentLevel = _currentLevel,
                    CurrentLevelXp = _currentXp,
                    NextLevelThreshold = NextLevelThreshold()
                });
            }
        }

        private int NextLevelThreshold()
        {
            return _playerLevelData.XpThresholdPerLevel.Length > _currentLevel ? _playerLevelData.XpThresholdPerLevel[_currentLevel] : _playerLevelData.XpThresholdPerLevel.Last();
        }

        public float GetTotalXp()
        {
            return _totalXp;
        }
    }
}