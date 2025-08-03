using Game.Drop;
using Game.PlayerUnit;
using Game.Reward;
using Game.UI;
using UnityEngine;

namespace Game.Dungeon.Gold
{
    [RequireComponent(typeof(GoldDungeonEnemyManager))]
    [RequireComponent(typeof(GoldDropManager))]
    [RequireComponent(typeof(GoldDungeonManager))]
    public class GoldDungeonGameManager : GameManager
    {
        [Range(0, 20)]
        [SerializeField] private int _freeInitialSkillsAmount;
        
        protected override void InitializeDrops()
        {
            GoldDropManager.Instance.Initialize();
        }

        protected override void DisableDrops()
        {
            GoldDropManager.Instance.Disable();
        }

        public override void InitializeGame()
        {
            base.InitializeGame();
            GoldDungeonManager.Instance.InitializePlayerForGoldDungeon(_freeInitialSkillsAmount);
            UIManager.Instance.HideRanksPanels();
            UIManager.Instance.HideXpBar();
            UIManager.Instance.InitializeGameGoldCounter();
        }
        
        protected override void GiveGameRewards(int playerRank)
        {
            int collectedGoldAmount = PlayerManager.Instance.GetHumanPlayer().GetCollectedGoldAmount();
            RewardManager.Instance.ReceiveGoldDungeonReward(collectedGoldAmount);
        }
        
        protected override void ShowMatchResultsScreen(bool isHumanPlayerDead)
        {
            UIManager.Instance.ShowGoldDungeonMatchResultsScreen(isHumanPlayerDead);
        }
    }
}