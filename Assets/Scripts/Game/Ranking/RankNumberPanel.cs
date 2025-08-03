using Game.PlayerUnit;
using Game.UI;
using UnityEngine;

namespace Game.Ranking
{
    public class RankNumberPanel : Panel
    {
        [SerializeField] private RankNumber _rankNumber;
    
        private int _oldRank;

        public void Initialize()
        {
            RankingManager.Instance.OnScoreChanged += RankingManager_OnScoreChanged;
            _oldRank = PlayerManager.Instance.GetHumanPlayer().GetPlayerIndex();
            _rankNumber.ChangeRank(_oldRank, PlayerManager.Instance.GetPlayersList().Count);
            _rankNumber.OnRankChanged += RankNumber_OnRankChanged;
        }

        private void RankingManager_OnScoreChanged()
        {
            int newRank = RankingManager.Instance.GetRankByPlayerIndex(PlayerManager.Instance.GetHumanPlayer().GetPlayerIndex());
        
            if (_oldRank == newRank)
            {
                return;
            }
        
            _oldRank = newRank;
            RankingManager.Instance.OnScoreChanged -= RankingManager_OnScoreChanged;
            _rankNumber.ChangeRankAnimation(newRank, PlayerManager.Instance.GetPlayersList().Count);
        }

        private void RankNumber_OnRankChanged()
        {
            RankingManager.Instance.OnScoreChanged += RankingManager_OnScoreChanged;
        }
    }
}
