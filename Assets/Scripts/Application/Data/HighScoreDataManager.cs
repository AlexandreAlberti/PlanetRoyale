using Application.Utils;
using UnityEngine;

namespace Application.Data
{
    public class HighScoreDataManager : MonoBehaviour
    {
        private const string RaceTime = "RaceTime";

        public static HighScoreDataManager Instance;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
        }

        public void UpdateBestTime(float newTime)
        {
            float currentBestTime = CurrentBestTime();
            
            if (newTime >= currentBestTime && currentBestTime != 0.0f)
            {
                return;
            }
            
            PlayerPrefs.SetFloat(RaceTime, newTime);
        }

        public void ResetBestTime()
        {
            PlayerPrefs.SetFloat(RaceTime, NumberConstants.Zero);
        }

        public float CurrentBestTime()
        {
            return PlayerPrefs.GetFloat(RaceTime);
        }
    }
}