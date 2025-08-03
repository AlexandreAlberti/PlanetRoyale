using Application.Data.Progression;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.MainMenu
{
    public class LeagueScreenLeagueInfo : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _arena;
        [SerializeField] private TextMeshProUGUI _requiredMinTrophies;
        [SerializeField] private Image _obtainedMark;
        [SerializeField] private LeagueRewards _leagueRewards;
        
        public void Initialize(LeagueData leagueData, int leagueIndex)
        {
            _arena.text = leagueData.TrophyRoadTitle;
            _requiredMinTrophies.text = $"{leagueData.MaxTrophiesExclusive}";
            _obtainedMark.enabled = ProgressionDataManager.Instance.HasReceivedLeagueReward(leagueIndex);
            _leagueRewards.Initialize(leagueData);
        }
    }
}