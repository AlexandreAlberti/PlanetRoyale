using Application.Data;
using Application.Data.Rewards;
using MobileConsole;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Application.ConsoleCommands
{
    public class ScoutCommands : MonoBehaviour
    {
        [ExecutableCommand(name = "Scout/Scout Max Rewards")]
        public class ScoutMaxRewardsCommand : Command
        {
            public override void Execute()
            {
                ScoutDataManager.Instance.ForceScoutTime(ScoutDataManager.Instance.GetMaxScoutTime());
                SceneManager.LoadScene((int) SceneEnum.MainMenu);
            }
        }
        
        [ExecutableCommand(name = "Scout/Scout Reward Available")]
        public class ScoutRewardAvailableCommand : Command
        {
            public override void Execute()
            {
                ScoutDataManager.Instance.ForceScoutTime(ScoutDataManager.Instance.GetClaimCooldownTime());
                SceneManager.LoadScene((int) SceneEnum.MainMenu);
            }
        }
        
        [ExecutableCommand(name = "Scout/Quick Scout Max Rewards")]
        public class QuickScoutMaxRewardsCommand : Command
        {
            public override void Execute()
            {
                ScoutDataManager.Instance.ForceQuickScoutEnergyTime(ScoutDataManager.Instance.GetMaxQuickScoutTime());
                SceneManager.LoadScene((int) SceneEnum.MainMenu);
            }
        }
    }
}