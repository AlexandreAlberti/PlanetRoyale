using Game.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Ranking
{
    public class RankingPanel : Panel
    {
        [SerializeField] private PlayerRankingRow[] _heroScoreRows;
        [SerializeField] private VerticalLayoutGroup _verticalLayoutGroup;
        
        public static RankingPanel Instance { get; set; }
        
        private void Awake()
        {
            Instance = this;
        }

        public void InitializeForMatchTutorial()
        {
            PlayerRankingRow[] newRows = new PlayerRankingRow[2];
            newRows[0] = _heroScoreRows[0];
            newRows[1] = _heroScoreRows[1];
            _heroScoreRows[2].gameObject.SetActive(false);
            _heroScoreRows[3].gameObject.SetActive(false);
            _heroScoreRows = newRows;
            _verticalLayoutGroup.spacing = -90;
        }
        
        public void UpdateRankingRows(string[] names, int[] scores, int[] playerIndexes)
        {
            for (int i = 0; i < names.Length; i++)
            {
                _heroScoreRows[i].UpdateScore(playerIndexes[i], names[i], i, scores[i]);
            }
        }
    }
}