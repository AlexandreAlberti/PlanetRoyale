using UnityEngine;

namespace Application.Data.Progression
{
    public class ProgressionDataManager : MonoBehaviour
    {
        private const string LeagueAttempts = "LeagueAttempts";
        private const string ReceivedLeagueReward = "ReceivedLeagueReward";
        private const string RegisterDoneAmplitude = "RegisterDoneAmplitude";
        private const string RegisterDoneAdjust = "RegisterDoneAdjust";
        private const string UserId = "UserId";

        public static ProgressionDataManager Instance { get; set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
        }
        
        public int GetLeagueAttempts(int leagueIndex)
        {
            string key = LeagueAttempts + leagueIndex;
            return PlayerPrefs.GetInt(key, 0);
        }

        public int IncrementLeagueAttempts(int leagueIndex)
        {
            string key = LeagueAttempts + leagueIndex;
            int leagueAttempts = PlayerPrefs.GetInt(key, 0) + 1;
            PlayerPrefs.SetInt(key, leagueAttempts);
            return leagueAttempts;
        }
        
        public void Clear(bool removeUserGeneratedData = false)
        {
            for (int leagueIndex = 0; leagueIndex < LeaguesManager.Instance.GetAllLeaguesInfo().Length; leagueIndex++)
            {
                PlayerPrefs.SetInt(LeagueAttempts + leagueIndex, 0);
                PlayerPrefs.SetInt(ReceivedLeagueReward + leagueIndex, 0);
            }

            if (removeUserGeneratedData)
            {
                PlayerPrefs.SetInt(RegisterDoneAmplitude, 0);
                PlayerPrefs.SetInt(RegisterDoneAdjust, 0);
                PlayerPrefs.SetString(UserId, "");
            }
        }
        
        public bool HasReceivedLeagueReward(int leagueIndex)
        {
            return PlayerPrefs.GetInt(ReceivedLeagueReward + leagueIndex) > 0;
        }

        public void ReceiveLeagueReward(int leagueIndex)
        {
            PlayerPrefs.SetInt(ReceivedLeagueReward + leagueIndex, 1);
        }

        public bool IsRegisterDoneAmplitude()
        {
            return PlayerPrefs.GetInt(RegisterDoneAmplitude) > 0;
        }

        public void SetRegisterDoneAmplitude()
        {
            PlayerPrefs.SetInt(RegisterDoneAmplitude, 1);
        }

        public bool IsRegisterDoneAdjust()
        {
            return PlayerPrefs.GetInt(RegisterDoneAdjust) > 0;
        }

        public void SetRegisterDoneAdjust()
        {
            PlayerPrefs.SetInt(RegisterDoneAdjust, 1);
        }

        public string GetUserId()
        {
            string userId = PlayerPrefs.GetString(UserId, "");

            if (string.IsNullOrEmpty(userId))
            {
                userId = System.Guid.NewGuid().ToString();
                PlayerPrefs.SetString(UserId, userId);
            }
            
            return userId;
        }
    }
}