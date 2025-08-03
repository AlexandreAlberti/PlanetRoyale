using System;
using Application.Utils;
using UnityEngine;

namespace Application.Data.Progression
{
    public class DungeonTicketsDataManager : MonoBehaviour
    {
        private const string GoldDungeonTickets = "GoldDungeonTickets";
        private const string GoldDungeonTicketsLastCheck = "GoldDungeonTicketsLastCheck";

        [SerializeField] private float _startingGoldDungeonTickets;
        [SerializeField] private float _maxGoldDungeonTickets;
        [SerializeField] private float _goldDungeonTicketsRecoverPerHour;

        public static DungeonTicketsDataManager Instance { get; set; }

        public Action<int> OnGoldDungeonTicketsSpent { get; set; }

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

        public float GetGoldDungeonTickets()
        {
            CheckGoldDungeonTicketsSystemIsStarted();
            float goldDungeonTickets = Mathf.Max(0.0f, PlayerPrefs.GetFloat(GoldDungeonTickets));
            float maxGoldDungeonTickets = GetMaxGoldDungeonTickets();

            if (goldDungeonTickets > maxGoldDungeonTickets)
            {
                return goldDungeonTickets;
            }
            
            float recoveryPerHour = GetGoldDungeonTicketsRecoveryRatePerHour();
            long now = DateTimeOffset.Now.ToUnixTimeSeconds();
            long then = Convert.ToInt64(PlayerPrefs.GetString(GoldDungeonTicketsLastCheck));
            long timePassedInSeconds = now - then;
            float goldDungeonTicketsRecovered = timePassedInSeconds * recoveryPerHour / TimeUtils.SecondsInHour;
            goldDungeonTickets = MathF.Min(goldDungeonTickets + goldDungeonTicketsRecovered, maxGoldDungeonTickets);
            PlayerPrefs.SetFloat(GoldDungeonTickets, goldDungeonTickets);
            PlayerPrefs.SetString(GoldDungeonTicketsLastCheck, now.ToString());
            return goldDungeonTickets;
        }

        public float GetMaxGoldDungeonTickets()
        {
            return _maxGoldDungeonTickets;
        }

        public float GetGoldDungeonTicketsRecoveryRatePerHour()
        {
            return _goldDungeonTicketsRecoverPerHour;
        }

        public bool SpendGoldDungeonTickets(float amount)
        {
            float goldDungeonTickets = GetGoldDungeonTickets() - amount;
            bool enoughGoldDungeonTickets = goldDungeonTickets >= 0.0f;

            if (enoughGoldDungeonTickets)
            {
                PlayerPrefs.SetFloat(GoldDungeonTickets, goldDungeonTickets);
                OnGoldDungeonTicketsSpent?.Invoke(Mathf.FloorToInt(amount));
            }

            return enoughGoldDungeonTickets;
        }

        public void AddGoldDungeonTickets(float amount)
        {
            float goldDungeonTickets = GetGoldDungeonTickets() + amount;

            if (goldDungeonTickets > _maxGoldDungeonTickets)
            {
                goldDungeonTickets = Mathf.FloorToInt(goldDungeonTickets);
            }

            PlayerPrefs.SetFloat(GoldDungeonTickets, goldDungeonTickets);
        }

        private void CheckGoldDungeonTicketsSystemIsStarted()
        {
            if (!PlayerPrefs.HasKey(GoldDungeonTicketsLastCheck) || 1 > Convert.ToInt64(PlayerPrefs.GetString(GoldDungeonTicketsLastCheck)))
            {
                PlayerPrefs.SetFloat(GoldDungeonTickets, _startingGoldDungeonTickets);
                PlayerPrefs.SetString(GoldDungeonTicketsLastCheck, DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
            }
        }

        public void RecoverFullGoldDungeonTickets()
        {
            PlayerPrefs.SetFloat(GoldDungeonTickets, _maxGoldDungeonTickets);
            PlayerPrefs.SetString(GoldDungeonTicketsLastCheck, DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
        }
        
        public void EmptyGoldDungeonTickets()
        {
            PlayerPrefs.SetFloat(GoldDungeonTickets, 0);
            PlayerPrefs.SetString(GoldDungeonTicketsLastCheck, DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
        }

        public void Clear()
        {
            PlayerPrefs.SetFloat(GoldDungeonTickets, _startingGoldDungeonTickets);
            PlayerPrefs.SetString(GoldDungeonTicketsLastCheck, DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
        }
    }
}