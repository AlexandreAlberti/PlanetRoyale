using TMPro;
using UnityEngine;

namespace Game.Match
{
    public class HeroKilledBossStatusPanel : HeroStatusPanel
    {
        [SerializeField] private TextMeshProUGUI _bossName;
        
        public void Show(float duration, int playerIndex, string bossName)
        {
            base.Show(duration, playerIndex);
            _bossName.text = bossName;
        }
    }
}