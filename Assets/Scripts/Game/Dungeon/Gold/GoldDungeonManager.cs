using Game.PlayerUnit;
using UnityEngine;

namespace Game.Dungeon.Gold
{
    public class GoldDungeonManager : MonoBehaviour
    {
        public static GoldDungeonManager Instance { get; private set; }
        
        private void Awake()
        {
            Instance = this;
        }

        public void InitializePlayerForGoldDungeon(int freeSkillsAmount)
        {
            Player humanPlayer = PlayerManager.Instance.GetHumanPlayer();
            humanPlayer.GiveInitialSkills(freeSkillsAmount);
            humanPlayer.DisableSkillSelection();
            humanPlayer.DisableXpEffects();
        }
    }
}
