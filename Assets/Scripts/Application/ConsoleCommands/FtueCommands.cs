using Application.Data;
using Application.Data.Currency;
using Application.Data.Equipment;
using Application.Data.Progression;
using Application.Data.Talent;
using Game.Ftue;
using MobileConsole;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Application.ConsoleCommands
{
    public class FtueCommands : MonoBehaviour
    {
        [ExecutableCommand(name = "FTUE/0 - Reset Game")]
        public class ResetGameCommand : Command
        {
            public override void Execute()
            {
                GameDataManager.Instance.Clear();
                EquipmentDataManager.Instance.Clear();
                CurrencyDataManager.Instance.Clear();
                TalentsDataManager.Instance.Clear();
                ProgressionDataManager.Instance.Clear(true);
                EnergyDataManager.Instance.Clear();
                SceneManager.LoadScene((int) SceneEnum.SplashScreen);
            }
        }

        [ExecutableCommand(name = "FTUE/100 - Skip Controls Tutorial")]
        public class SkipControlsTutorialCommand : Command
        {
            public override void Execute()
            {
                GameDataManager.Instance.SaveFtueStep(FtueManager.FinishedControlsTutorialStep);
                SceneManager.LoadScene((int) SceneEnum.Game);
            }
        }
        
        [ExecutableCommand(name = "FTUE/200 - Skip Match Tutorial")]
        public class SkipMatchTutorialCommand : Command
        {
            public override void Execute()
            {
                GameDataManager.Instance.SaveFtueStep(FtueManager.FinishedMatchTutorialStep);
                SceneManager.LoadScene((int) SceneEnum.MainMenu);
            }
        }

        [ExecutableCommand(name = "FTUE/201 - Skip First Real Match Step")]
        public class SkipFirstRealMatchStepCommand : Command
        {
            public override void Execute()
            {
                GameDataManager.Instance.SaveFtueStep(FtueManager.FinishedFirstRealGameStep);
                SceneManager.LoadScene((int) SceneEnum.MainMenu);
            }
        }

        [ExecutableCommand(name = "FTUE/203 - Skip League One Up Step")]
        public class SkipLeagueOneUpStepCommand : Command
        {
            public override void Execute()
            {
                GameDataManager.Instance.SaveFtueStep(FtueManager.FinishedLeagueOneUpStep);
                SceneManager.LoadScene((int) SceneEnum.MainMenu);
            }
        }
        
        [ExecutableCommand(name = "FTUE/300 - Skip Talents Step")]
        public class SkipTalentsStepCommand : Command
        {
            public override void Execute()
            {
                GameDataManager.Instance.SaveFtueStep(FtueManager.FinishedTalentsTutorialStep);
                SceneManager.LoadScene((int) SceneEnum.MainMenu);
            }
        }

        [ExecutableCommand(name = "FTUE/301 - Skip Second Real Match Step")]
        public class SkipSecondRealMatchStepCommand : Command
        {
            public override void Execute()
            {
                GameDataManager.Instance.SaveFtueStep(FtueManager.FinishedSecondRealGameStep);
                SceneManager.LoadScene((int) SceneEnum.MainMenu);
            }
        }

        [ExecutableCommand(name = "FTUE/400 - Skip League Two Up Step")]
        public class SkipLeagueTwoUpStepCommand : Command
        {
            public override void Execute()
            {
                GameDataManager.Instance.SaveFtueStep(FtueManager.FinishedLeagueTwoUpStep);
                SceneManager.LoadScene((int) SceneEnum.MainMenu);
            }
        }

        [ExecutableCommand(name = "FTUE/500 - Skip Equipment Step")]
        public class SkipEquipmentStepCommand : Command
        {
            public override void Execute()
            {
                GameDataManager.Instance.SaveFtueStep(FtueManager.FinishedEquipmentTutorialStep);
                SceneManager.LoadScene((int) SceneEnum.MainMenu);
            }
        }

        [ExecutableCommand(name = "FTUE/600 - Skip Gold Dungeon Step")]
        public class SkipGoldDungeonStepCommand : Command
        {
            public override void Execute()
            {
                GameDataManager.Instance.SaveFtueStep(FtueManager.FinishedGoldDungeonStep);
                SceneManager.LoadScene((int) SceneEnum.MainMenu);
            }
        }
        
        [ExecutableCommand(name = "FTUE/700 - Skip Merge Step")]
        public class SkipMergeStepCommand : Command
        {
            public override void Execute()
            {
                GameDataManager.Instance.SaveFtueStep(FtueManager.FinishedMergeTutorialStep);
                SceneManager.LoadScene((int) SceneEnum.MainMenu);
            }
        }
        
        [ExecutableCommand(name = "FTUE/701 - Skip Third Real Match Step")]
        public class SkipThirdRealMatchStepCommand : Command
        {
            public override void Execute()
            {
                GameDataManager.Instance.SaveFtueStep(FtueManager.FinishedThirdRealGameStep);
                SceneManager.LoadScene((int) SceneEnum.MainMenu);
            }
        }
        
        [ExecutableCommand(name = "FTUE/800 - Skip Scout Step")]
        public class SkipScoutStepCommand : Command
        {
            public override void Execute()
            {
                GameDataManager.Instance.SaveFtueStep(FtueManager.FinishedScoutTutorialStep);
                SceneManager.LoadScene((int) SceneEnum.MainMenu);
            }
        }
        
        [ExecutableCommand(name = "FTUE/900 - Skip Change Name Step")]
        public class SkipChangeNameStepCommand : Command
        {
            public override void Execute()
            {
                GameDataManager.Instance.SaveFtueStep(FtueManager.FinishedChangeNameTutorialStep);
                SceneManager.LoadScene((int) SceneEnum.MainMenu);
            }
        }
        
        
        [ExecutableCommand(name = "FTUE/901 - Skip Fourth Real Match Step")]
        public class SkipFourthRealMatchStepCommand : Command
        {
            public override void Execute()
            {
                GameDataManager.Instance.SaveFtueStep(FtueManager.FinishedFourthRealGameStep);
                SceneManager.LoadScene((int) SceneEnum.MainMenu);
            }
        }
        
        [ExecutableCommand(name = "FTUE/999 - Skip All FTUE")]
        public class SkipFtueCommand : Command
        {
            public override void Execute()
            {
                GameDataManager.Instance.SaveFtueStep(FtueManager.FinalStep);
                SceneManager.LoadScene((int) SceneEnum.MainMenu);
            }
        }
    }
}