using Application.Data;
using Application.Data.Rewards;
using MobileConsole;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Application.ConsoleCommands
{
    public class ForcePlanetCommands : MonoBehaviour
    {
        [ExecutableCommand(name = "ForcePlanet/Force Green Planet")]
        public class ForceGreenPlanetCommand : Command
        {
            public override void Execute()
            {
                GameDataManager.Instance.SaveIsGreenPlanetForcedGame(true);
            }
        }
        
        [ExecutableCommand(name = "ForcePlanet/Force Ice Planet")]
        public class ForceIcePlanetCommand : Command
        {
            public override void Execute()
            {
                GameDataManager.Instance.SaveIsIcePlanetForcedGame(true);
            }
        }
        
        [ExecutableCommand(name = "ForcePlanet/Force Fire Planet")]
        public class ForceFirePlanetCommand : Command
        {
            public override void Execute()
            {
                GameDataManager.Instance.SaveIsFirePlanetForcedGame(true);
            }
        }
    }
}