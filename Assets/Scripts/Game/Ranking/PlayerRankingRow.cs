using Game.PlayerUnit;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Ranking
{
    public class PlayerRankingRow : MonoBehaviour
    {
        [SerializeField] private GameObject _deathOverlay;
        [SerializeField] private GameObject _playerBorder;
        [SerializeField] private Image _playerBorderImage;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private RankNumber _rankNumber;
        [SerializeField] private Color _localPlayerNameColor;
        [SerializeField] private Color _nonLocalPlayerNameColor;
        [SerializeField] private Color _nonLocalPlayerRankColor;

        private const int InitialScore = 0;
        private const string DisqualifiedScore = "";

        private bool _isDisqualified;

        public void UpdateScore(int playerIndex, string playerName, int rank, int newScore = InitialScore)
        {
            if (playerIndex == PlayerManager.Instance.GetHumanPlayer().GetPlayerIndex())
            {
                int numberOfActiveHeroes = PlayerManager.Instance.GetPlayersList().Count;
                _nameText.color = _localPlayerNameColor;
                transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
                _playerBorder.gameObject.SetActive(true);
                _playerBorderImage.color = _rankNumber.GetColorFromRankAndNumberOfPlayers(rank, numberOfActiveHeroes);
                _rankNumber.ChangeRank(rank, numberOfActiveHeroes);
            }
            else
            {
                _nameText.color = _nonLocalPlayerNameColor;
                transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                _playerBorder.gameObject.SetActive(false);
                _rankNumber.SetRank(rank);
                _rankNumber.SetColor(_nonLocalPlayerRankColor);
            }

            _isDisqualified = newScore < 0;
            _nameText.text = playerName;
            _scoreText.text = _isDisqualified ? DisqualifiedScore : newScore.ToString();
            _deathOverlay.SetActive(_isDisqualified);
        }

        public void Disqualify()
        {
            _isDisqualified = true;
            _scoreText.text = DisqualifiedScore;
            _deathOverlay.SetActive(true);
        }
    }
}