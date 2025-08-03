using Game.PlayerUnit;
using TMPro;
using UnityEngine;

namespace Game.Match
{
    public class HeroStatusPanel : StatusPanel
    {
        [SerializeField] private TextMeshProUGUI _name;
        
        public void Show(float duration, int playerIndex)
        {
            base.Show(duration);
            
            Player player = PlayerManager.Instance.GetPlayer(playerIndex);
            _icon.sprite = player.GetSprite();
            _name.text = player.GetName();
        }
    }
}