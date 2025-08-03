using System.Linq;
using Application.Data.Currency;
using UnityEngine;

namespace Application.Data.Progression
{
    public class LeaguesManager : MonoBehaviour
    {
        [SerializeField] private LeagueData[] _leaguesData;
        public static LeaguesManager Instance { get; private set; }

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

        public LeagueData GetCurrentLeagueData()
        {
            int trophies = CurrencyDataManager.Instance.CurrentTrophies();

            foreach (LeagueData leagueData in _leaguesData)
            {
                if (trophies >= leagueData.MinTrophiesInclusive && trophies < leagueData.MaxTrophiesExclusive)
                {
                    return leagueData;
                }
            }

            Debug.LogWarning($"No league found for trophies amount {trophies}, using last in list");
            return _leaguesData.Last();
        }

        public int GetCurrentLeagueIndex()
        {
            int trophies = CurrencyDataManager.Instance.CurrentTrophies();

            for( int i = 0; i < _leaguesData.Length ; i++)
            {
                LeagueData leagueData = _leaguesData[i];
                
                if (trophies >= leagueData.MinTrophiesInclusive && trophies < leagueData.MaxTrophiesExclusive)
                {
                    return i;
                }
            }

            Debug.LogWarning($"No league found for trophies amount {trophies}, using last in list");
            return _leaguesData.Length - 1;
        }

        public LeagueData[] GetAllLeaguesInfo()
        {
            return _leaguesData;
        }

        public LeagueData GetLeagueInfo(int league)
        {
            if (league < 0)
            {
                return _leaguesData[0];
            }

            if (league < _leaguesData.Length)
            {
                return _leaguesData[league];
            }

            return _leaguesData.Last();
        }
    }
}