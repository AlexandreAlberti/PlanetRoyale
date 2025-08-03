using System;
using System.Collections.Generic;
using System.Linq;
using Game.Enabler;
using Game.Ftue;
using Game.PlayerUnit;
using UnityEngine;

namespace Game.Ranking
{
    public class RankingManager : EnablerMonoBehaviour
    {
        private List<Player> _players;
        private Dictionary<int, float> _playerRankings;

        public static RankingManager Instance { get; private set; }

        public Action OnScoreChanged { get; internal set; }
        
        private void Awake()
        {
            Instance = this;
        }

        public override void Disable()
        {
            base.Disable();
            
            foreach (Player player in _players)
            {
                if (player)
                {
                    player.OnXpGained -= Player_OnXpGained;
                    player.OnDead -= Player_OnDead;
                }
            }
        }

        public void Initialize(List<Player> players)
        {
            _players = players;
            _playerRankings = new Dictionary<int, float>();

            foreach (Player player in _players)
            {
                if (player)
                {
                    player.OnXpGained += Player_OnXpGained;
                    player.OnDead += Player_OnDead;
                    _playerRankings.Add(player.GetPlayerIndex(), 0);
                }
            }

            UpdateRankingPanel();
        }

        private void Player_OnDead(Player player)
        {
            _playerRankings[player.GetPlayerIndex()] = -1;
            UpdateRankingPanel();
            OnScoreChanged?.Invoke();
        }

        public override void Enable()
        {
            base.Enable();
            UpdateRankingPanel();
            
            foreach (Player player in _players)
            {
                if (player)
                {
                    player.OnXpGained += Player_OnXpGained;
                    player.OnDead += Player_OnDead;
                }
            }
        }

        public void Reset()
        {
            foreach ((int playerIndex, float playerRanking) in _playerRankings)
            {
                _playerRankings[playerIndex] = 0;
            }
        }

        private void Player_OnXpGained(PlayerLevel.OnXpGainedEventArgs onXpGainedEventArgs, int playerIndex)
        {
            if (playerIndex != Player.GetHumanPlayerIndex() && PlayerManager.Instance.GetHumanPlayer().GetTotalXp() <= onXpGainedEventArgs.TotalXp && FtueManager.Instance.HasClearedControlsTutorialLevel() && !FtueManager.Instance.HasClearedMatchTutorialLevel())
            {
                _playerRankings[playerIndex] = Mathf.Max(PlayerManager.Instance.GetHumanPlayer().GetTotalXp() - 1, 0);
            }
            else
            {
                _playerRankings[playerIndex] = onXpGainedEventArgs.TotalXp;
            }
            
            UpdateRankingPanel();
            OnScoreChanged?.Invoke();
        }

        public void UpdateRankingPanel()
        {
            string[] names = new string[_players.Count];
            int[] scores = new int[_players.Count];
            int[] playerIndexes = new int[_players.Count];
            int index = 0;

            foreach (KeyValuePair<int, float> rankingValue in _playerRankings.ToList().OrderByDescending(playerRanking => playerRanking.Value))
            {
                names[index] = _players[rankingValue.Key].GetName();
                playerIndexes[index] = rankingValue.Key;
                scores[index] = Mathf.FloorToInt(rankingValue.Value);
                index++;
            }

            RankingPanel.Instance.UpdateRankingRows(names, scores, playerIndexes);
        }

        public int GetPlayerScore(int playerIndex)
        {
            return Mathf.FloorToInt(_playerRankings[playerIndex]);
        }
        
        public int GetRankByPlayerIndex(int playerIndex)
        {
            int rank = 0;
            
            foreach (KeyValuePair<int, float> rankingValue in _playerRankings.ToList().OrderByDescending(playerRanking => playerRanking.Value))
            {
                if (rankingValue.Key == playerIndex)
                {
                    return rank;
                }
                
                rank++;
            }

            return rank;
        }

        public int GetHumanPlayerRank()
        {
            return GetRankByPlayerIndex(Player.GetHumanPlayerIndex());
        }
    }
}