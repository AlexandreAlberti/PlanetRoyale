using Game.Enabler;
using Game.PlayerUnit;
using TMPro;
using UnityEngine;

namespace Game.UI
{
    public class GameGoldCounter : EnablerMonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _amountText;
        
        public void Initialize()
        {
            _amountText.text = "0";
            Player player = PlayerManager.Instance.GetHumanPlayer();
            player.OnGoldGained += Player_OnGoldGained;
        }
        
        private void Player_OnGoldGained(int newGoldAmount)
        {
            _amountText.text = $"{newGoldAmount}";
        }
    }
}