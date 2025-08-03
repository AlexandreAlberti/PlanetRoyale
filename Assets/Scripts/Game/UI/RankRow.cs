using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class RankRow : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _playerName;
        [SerializeField] private TextMeshProUGUI _playerScore;
        [SerializeField] private Image _playerIcon;
        [SerializeField] private GameObject _otherPlayerBar;
        [SerializeField] private GameObject _humanPlayerBar;
        [SerializeField] private GameObject _humanPlayerDeadBar;

        private const string DeadScore = "X";

        public void SetPlayerName(string playerName)
        {
            _playerName.text = playerName;
        }

        public void SetPlayerIcon(Sprite playerIcon)
        {
            _playerIcon.sprite = playerIcon;
        }

        public void SetPlayerScore(int playerScore)
        {
            _playerScore.text = playerScore == -1 ? DeadScore : playerScore.ToString();
        }

        public void SetOtherPlayerBackground()
        {
            _otherPlayerBar.SetActive(true);
            _humanPlayerBar.SetActive(false);
            _humanPlayerDeadBar.SetActive(false);
        }

        public void SetHumanPlayerBackground()
        {
            _otherPlayerBar.SetActive(false);
            _humanPlayerBar.SetActive(true);
            _humanPlayerDeadBar.SetActive(false);
        }

        public void SetHumanPlayerDeadBackground()
        {
            _otherPlayerBar.SetActive(false);
            _humanPlayerBar.SetActive(false);
            _humanPlayerDeadBar.SetActive(true);
        }
    }
}