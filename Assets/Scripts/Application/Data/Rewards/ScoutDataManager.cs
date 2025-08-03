using System;
using Application.Data.Progression;
using Application.Utils;
using UnityEngine;

namespace Application.Data.Rewards
{
    public class ScoutDataManager : MonoBehaviour
    {
        private const string ScoutTimeLastCheck = "ScoutTimeLastCheck";
        private const string QuickScoutTimeLastCheck = "QuickScoutTimeLastCheck";
        private const string QuickScoutRemainingClaims = "QuickScoutRemainingClaims";

        [SerializeField] private int _maxTimeInSeconds;
        [SerializeField] private int _claimCooldownInSeconds;
        [SerializeField] private int _quickScoutEnergyCost;
        [SerializeField] private int _quickScoutDailyClaims;
        [SerializeField] private int _quickScoutRewardValueInSeconds;
        [SerializeField] private int _quickScoutRefreshTimeInSeconds;
        
        private float _indicatorsCooldown;

        public Action OnScoutClaimed;
        public Action OnQuickScoutClaimed;
        public Action OnScoutClaimAvailable;
        public Action OnQuickScoutClaimAvailable;
        public Action OnScoutTimeMaxedOut;

        public static ScoutDataManager Instance { get; set; }

        private void Awake()
        {
            Instance = this;
            CheckScoutData();
        }
        
        private void Update()
        {
            _indicatorsCooldown += Time.deltaTime;

            if (_indicatorsCooldown >= NumberConstants.One)
            {
                _indicatorsCooldown = NumberConstants.Zero;
                
                if (GetNextQuickClaimTime() < 0)
                {
                    SetInitialQuickScoutDailyClaims();
                }

                if (GetQuickScoutDailyChances() > 1)
                {
                    OnQuickScoutClaimAvailable?.Invoke();
                }
                
                if (IsAtMaxScoutTime())
                {
                    OnScoutTimeMaxedOut?.Invoke();
                }
                
                if (GetNextClaimTime() >= _claimCooldownInSeconds)
                {
                    OnScoutClaimAvailable?.Invoke();
                }
            }
        }
        
        private void CheckScoutData()
        {
            if (!PlayerPrefs.HasKey(ScoutTimeLastCheck) || 1 > Convert.ToInt64(PlayerPrefs.GetString(ScoutTimeLastCheck)))
            {
                PlayerPrefs.SetString(ScoutTimeLastCheck, DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
                SetInitialQuickScoutDailyClaims();
                PlayerPrefs.SetString(QuickScoutTimeLastCheck, DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
            }
        }

        public void SetInitialQuickScoutDailyClaims()
        {
            PlayerPrefs.SetInt(QuickScoutRemainingClaims, _quickScoutDailyClaims);
        }

        public float GetScoutTime()
        {
            long now = DateTimeOffset.Now.ToUnixTimeSeconds();
            long then = Convert.ToInt64(PlayerPrefs.GetString(ScoutTimeLastCheck));
            long timePassedInSeconds = now - then;
            
            if (timePassedInSeconds > _maxTimeInSeconds)
            {
                return _maxTimeInSeconds;
            }
            
            return timePassedInSeconds;
        }

        public void ClaimScout()
        {
            long now = DateTimeOffset.Now.ToUnixTimeSeconds();
            PlayerPrefs.SetString(ScoutTimeLastCheck, now.ToString());

            OnScoutClaimed?.Invoke();
        }

        public bool CanClaim()
        {
            long now = DateTimeOffset.Now.ToUnixTimeSeconds();
            long then = Convert.ToInt64(PlayerPrefs.GetString(ScoutTimeLastCheck));
            long timePassedInSeconds = now - then;
            
            return timePassedInSeconds >= _claimCooldownInSeconds;
        }

        public void ForceScoutTime(int scoutTime)
        {
            long now = DateTimeOffset.Now.ToUnixTimeSeconds();
            long then = now - scoutTime;
            PlayerPrefs.SetString(ScoutTimeLastCheck, then.ToString());
        }

        public bool IsAtMaxScoutTime()
        {
            long now = DateTimeOffset.Now.ToUnixTimeSeconds();
            long then = Convert.ToInt64(PlayerPrefs.GetString(ScoutTimeLastCheck));
            long timePassedInSeconds = now - then;
            
            return timePassedInSeconds >= _maxTimeInSeconds;
        }

        public long GetNextClaimTime()
        {
            long now = DateTimeOffset.Now.ToUnixTimeSeconds();
            long then = Convert.ToInt64(PlayerPrefs.GetString(ScoutTimeLastCheck));
            
            return _claimCooldownInSeconds + then - now;
        }

        public long GetNextQuickClaimTime()
        {
            long now = DateTimeOffset.Now.ToUnixTimeSeconds();
            long then = Convert.ToInt64(PlayerPrefs.GetString(QuickScoutTimeLastCheck));
            
            return _quickScoutRefreshTimeInSeconds + then - now;
        }
        
        public int GetQuickScoutEnergyCost()
        {
            return _quickScoutEnergyCost;
        }

        public int GetQuickScoutDailyChances()
        {
            return PlayerPrefs.GetInt(QuickScoutRemainingClaims);
        }

        public int GetQuickScoutRewardValue()
        {
            return _quickScoutRewardValueInSeconds;
        }

        public void ClaimQuickScout()
        {
            long now = DateTimeOffset.Now.ToUnixTimeSeconds();
            int remainingClaims = PlayerPrefs.GetInt(QuickScoutRemainingClaims);

            if (remainingClaims == _quickScoutDailyClaims)
            {
                PlayerPrefs.SetString(QuickScoutTimeLastCheck, now.ToString());
            }

            PlayerPrefs.SetInt(QuickScoutRemainingClaims, Mathf.Max(0, remainingClaims - 1));
            
            OnQuickScoutClaimed?.Invoke();
        }
        
        public void ForceQuickScoutEnergyTime(int scoutTime)
        {
            long now = DateTimeOffset.Now.ToUnixTimeSeconds();
            long then = now - scoutTime;
            PlayerPrefs.SetString(QuickScoutTimeLastCheck, then.ToString());
        }

        public bool CanQuickClaim()
        {
            return GetQuickScoutDailyChances() > 0 && EnergyDataManager.Instance.GetEnergy() >= _quickScoutEnergyCost;
        }

        public int GetMaxScoutTime()
        {
            return _maxTimeInSeconds;
        }

        public int GetClaimCooldownTime()
        {
            return _claimCooldownInSeconds;
        }

        public int GetMaxQuickScoutTime()
        {
            return _quickScoutRefreshTimeInSeconds;
        }
    }
}