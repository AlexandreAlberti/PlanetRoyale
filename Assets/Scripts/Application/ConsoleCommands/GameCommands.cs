using Game.Damage;
using Game.Match;
using Game.PlayerUnit;
using MobileConsole;
using UnityEngine;

namespace Application.ConsoleCommands
{
    public class GameCommands : MonoBehaviour
    {
        [ExecutableCommand(name = "Game/Damage 25")]
        public class DamagePlayerCommand : Command
        {
            public override void Execute()
            {
                PlayerManager.Instance.GetHumanPlayer().UnavoidableDamage(25, DamageType.Melee, DamageEffect.None);
            }
        }

        [ExecutableCommand(name = "Game/Damage 100")]
        public class FullDamagePlayerCommand : Command
        {
            public override void Execute()
            {
                PlayerManager.Instance.GetHumanPlayer().UnavoidableDamage(100, DamageType.Melee, DamageEffect.None);
            }
        }

        [ExecutableCommand(name = "Game/Kill Player")]
        public class KilPlayerCommand : Command
        {
            public override void Execute()
            {
                PlayerManager.Instance.GetHumanPlayer().UnavoidableDamage(PlayerManager.Instance.GetHumanPlayer().CurrentHealth(), DamageType.Melee, DamageEffect.None);
            }
        }

        [ExecutableCommand(name = "Game/Damage 100 Npcs")]
        public class FullDamageNpcPlayersCommand : Command
        {
            public override void Execute()
            {
                foreach (Player player in PlayerManager.Instance.GetPlayersList())
                {
                    if (player.GetPlayerIndex() != Player.GetHumanPlayerIndex())
                    {
                        player.UnavoidableDamage(100, DamageType.Melee, DamageEffect.None);
                    }
                }
            }
        }

        [ExecutableCommand(name = "Game/Kill 1 Npc")]
        public class Kill1NpcPlayersCommand : Command
        {
            public override void Execute()
            {
                foreach (Player player in PlayerManager.Instance.GetPlayersList())
                {
                    if (player.GetPlayerIndex() != Player.GetHumanPlayerIndex() && player.CurrentHealth() > 10)
                    {
                        player.UnavoidableDamage(player.CurrentHealth(), DamageType.Melee, DamageEffect.None);
                        break;
                    }
                }
            }
        }

        [ExecutableCommand(name = "Game/Heal 25%")]
        public class HealPlayerCommand : Command
        {
            public override void Execute()
            {
                PlayerManager.Instance.GetHumanPlayer().HealPercentage(0.25f);
            }
        }

        [ExecutableCommand(name = "Game/Gain 500 XP")]
        public class Gain500XpCommand : Command
        {
            public override void Execute()
            {
                PlayerManager.Instance.GetHumanPlayer().GainXp(500);
            }
        }

        [ExecutableCommand(name = "Game/Gain 5000 XP")]
        public class Gain5000XpCommand : Command
        {
            public override void Execute()
            {
                PlayerManager.Instance.GetHumanPlayer().GainXp(5000);
            }
        }

        [ExecutableCommand(name = "Game/End Timer")]
        public class EndTimerCommand : Command
        {
            public override void Execute()
            {
                MatchManager.Instance.ForceEndGameByTimer();
            }
        }
    }
}