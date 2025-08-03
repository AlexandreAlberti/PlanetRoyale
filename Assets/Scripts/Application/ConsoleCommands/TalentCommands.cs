using Application.Data.Talent;
using MobileConsole;
using UnityEngine;

namespace Application.ConsoleCommands
{
    public class TalentCommands : MonoBehaviour
    {
        [ExecutableCommand(name = "Talents/Reset All Talents")]
        public class AllTalentsToLevel0Command : Command
        {
            public override void Execute()
            {
                TalentsDataManager.Instance.SetAllTalentsToLevel(0);
            }
        }
        
        [ExecutableCommand(name = "Talents/Maxed Out All Talents")]
        public class AllTalentsToLevel20Command : Command
        {
            public override void Execute()
            {
                TalentsDataManager.Instance.SetAllTalentsToLevel(20);
            }
        }
    }
}