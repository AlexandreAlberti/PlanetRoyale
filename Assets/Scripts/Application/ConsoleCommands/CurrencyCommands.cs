using Application.Data.Currency;
using Application.Data.Progression;
using MobileConsole;
using UI.MainMenu;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Application.ConsoleCommands
{
    public class CurrencyCommands : MonoBehaviour
    {
        [ExecutableCommand(name = "Currency/Add 10000 Gold")]
        public class AddGold10000Command : Command
        {
            public override void Execute()
            {
                CurrencyDataManager.Instance.GainGold(10000);
            }
        }

        [ExecutableCommand(name = "Energy/Recover Full Energy")]
        public class RecoverFullEnergyCommand : Command
        {
            public override void Execute()
            {
                EnergyDataManager.Instance.RecoverFullEnergy();
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }

        [ExecutableCommand(name = "Energy/Empty Energy")]
        public class EmptyEnergyCommand : Command
        {
            public override void Execute()
            {
                EnergyDataManager.Instance.EmptyEnergy();
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
        
        [ExecutableCommand(name = "GoldDungeonTickets/Recover Full GoldDungeonTickets")]
        public class RecoverFullGoldDungeonTicketsCommand : Command
        {
            public override void Execute()
            {
                DungeonTicketsDataManager.Instance.RecoverFullGoldDungeonTickets();
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }

        [ExecutableCommand(name = "GoldDungeonTickets/Add 5 GoldDungeonTickets")]
        public class Add5GoldDungeonTicketsCommand : Command
        {
            public override void Execute()
            {
                DungeonTicketsDataManager.Instance.AddGoldDungeonTickets(5);
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }

        [ExecutableCommand(name = "GoldDungeonTickets/Empty GoldDungeonTickets")]
        public class EmptyGoldDungeonTicketsCommand : Command
        {
            public override void Execute()
            {
                DungeonTicketsDataManager.Instance.EmptyGoldDungeonTickets();
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
        
        [ExecutableCommand(name = "Currency/Give 30 Trophies")]
        public class Give30TrophiesCommand : Command
        {
            public override void Execute()
            {
                CurrencyDataManager.Instance.UpdateTrophies(30);
                RewardUIParticleManager.Instance.ForceTrophiesWonAmount(30);
                RewardUIParticleManager.Instance.ForceTrophiesParticles(6);
                
                LeagueScreenButton leagueScreenButton = FindObjectOfType<LeagueScreenButton>();

                if (leagueScreenButton)
                {
                    leagueScreenButton.PerformTrophiesWonChoreography();
                }
                
                MenuManager menuManager = FindObjectOfType<MenuManager>();

                if (menuManager)
                {
                    menuManager.UpdateUIWithGoldAndRewardedParticles();
                }
            }
        }
        
        [ExecutableCommand(name = "Currency/Give 15 Trophies")]
        public class Give15TrophiesCommand : Command
        {
            public override void Execute()
            {
                CurrencyDataManager.Instance.UpdateTrophies(15);
                RewardUIParticleManager.Instance.ForceTrophiesWonAmount(15);
                RewardUIParticleManager.Instance.ForceTrophiesParticles(3);
                LeagueScreenButton leagueScreenButton = FindObjectOfType<LeagueScreenButton>();

                if (leagueScreenButton)
                {
                    leagueScreenButton.PerformTrophiesWonChoreography();
                }
                
                MenuManager menuManager = FindObjectOfType<MenuManager>();

                if (menuManager)
                {
                    menuManager.UpdateUIWithGoldAndRewardedParticles();
                }
            }
        }
        
        [ExecutableCommand(name = "Currency/Remove 30 Trophies")]
        public class Remove30TrophiesCommand : Command
        {
            public override void Execute()
            {
                CurrencyDataManager.Instance.UpdateTrophies(-30);
                RewardUIParticleManager.Instance.ForceTrophiesWonAmount(-30);
                LeagueScreenButton leagueScreenButton = FindObjectOfType<LeagueScreenButton>();

                if (leagueScreenButton)
                {
                    leagueScreenButton.PerformTrophiesWonChoreography();
                }
                
                MenuManager menuManager = FindObjectOfType<MenuManager>();

                if (menuManager)
                {
                    menuManager.UpdateUIWithGoldAndRewardedParticles();
                }
            }
        }
    }
}