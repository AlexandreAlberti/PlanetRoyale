using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Application.Data
{
    public class NameDataManager : MonoBehaviour
    {
        private const string PlayerName = "PlayerName";
        private const string DefaultPlayerName = "Player";

        private readonly string[] _playerNpcNames =
        {
            "ShadowViper", "DeadlockX", "BulletReign", "NovaStriker", "GhostEcho", "SilentK1ll", "ColdBreach", "TacticalFury", "ZeroScope", "ApexBlitz", 
            "ArcaneFlame", "DarkCentaur", "FrostbladeZ", "InfernoWitch", "StormCaller", "RuneKnight", "SpectralFang", "VoidPhoenix", "EmberSoul", 
            "ChaosWarden", "Teabaggins", "LagKing", "NoScopeMom", "iCamp4Fun", "0PingWarrior", "HeadshotHarry", "CtrlAltDefeat", "FeedLord", "BabyYodaSnipes", 
            "XboxFridge", "Zenith", "Kairo", "Drexen", "Stryfe", "Valen", "KyroX", "Nexo", "Lith", "Oziq", "Vorn", "ClutchG0d", "FaZeWannabe", "AimBot3000", 
            "360Deadzone", "ToxicRush", "VortexGG", "SnipeMachine", "Cr4nked", "KillStreakz", "OverPeak", "QuantumBanana", "CosmicYeet", "NullError", 
            "ToastGrenade", "BananaSn1per", "SchrödingersCat", "SpicyPacket", "LaughingPixel", "StaticWaffle", "EchoMango", "Z3R0_D3V", "N1ghtH4wk", 
            "L33TSh4d0w", "XxV1RuSxX", "Gl1tch_M4n", "D34thDr1v3", "Fr4gm4st3r", "C0D3R_X", "K1llSw1tch", "8itOverlord", "RabidWolf", "CrimsonRaven", 
            "PantherClaw", "ToxicCobra", "IronFalcon", "JetJaguar", "FrostHawk", "StealthLynx", "VenomMoose", "ApexTiger", "Skulled", "Flicked", "Breached", 
            "Snared", "Feinted", "Blazed", "Spammed", "Choked", "Pinged", "Zoned", "MinionPlz", "JungleTax", "GankedAgain", "RiftSweeper", "0CSMid", 
            "ReportTeemo", "FakerzCousin", "LaneAFK", "BotLaneDoom", "HextechDreams", "Sn1p3rX99", "Ghost_47", "D3athBr1ng3r", "K1llz0neX", "_N1ghtW0lf_", 
            "Cr4zY_Sh0tz", "Xx_Headbust3r_xX", "No0bS14yer", "Bl4d3Mast3r", "R34per_777", 
            "Sk1llz4Dayz", "iTz_V3n0m", "Qu1kSc0p3z", "TTV_Sh4d0w", "420_Kraken", "FaZe_G0dX", "Tr1pl3_Tap", "Dr3amKillz", "S1lent_T4ctix", "xx_Kn1f3r_xx", 
            "LagSw1tch420", "S0ulHunt3r", "0hm3ga_X", "L3gendK1ll", "Z3r0_Mercy", "P1ngPwner", "Killah_69", "S3r1alG4m3r", "H4xOr_1337", "N0Sc0p3_Gh0st", 
            "S4v4g3_Xx", "99_Deadly", "Kr4zyK1llz", "DeathGr1pX", "R0gueSn1p3z", "G4m3rT1me", "X_R3kt_X", "EzCl4p_08", "V1rus_B1t", "H34dHunter93", "2Fast_4U", 
            "Darkn3ss_X", "T3rr0rByte", "M1dN1ght_Xx", "Br34k3r69", "C0ld_S0ld13r", "Fr4gOut_21", "Xx_Crush07_xX", "1T4chi_Uchiha", "D1g1talD3mon", "TehN00b42", 
            "R4nk1ngUp", "SlayZz_99", "T0xic_Tom", "Kn1ght0fR4ge", "OofMaster7", "Flashbangz420", "H0pSh0t_Xx", "EzMode101", "Sh00tM3Plz", "x_DeadAim_x", 
            "S1mply_Clutch", "CtrlZ4Life", "3xecut0r99", "404GamerNotFound", "Campzilla88", "Tr1cksh0tM4n", "U_Lagged_1st", "N00bl3tXx", "P0tat0Aim", 
            "DadAim_69", "360_N0sc0pe", "ItsUrPingNotMe", "D3sp4ir_Bot", "C4mpKing", "HideAndPe4k", "R00kieTTV", "K1ttyK1ll3r", "B4byY0daSn1pes", "AimBotDad", 
            "WallBangz69", "1v9_SoloQ", "Fr4gNSt34l", "D34dByM0m", "LoL_F33d3r", "Respawn3r", "C4llMeSn4ck", "P34kAndDie", "2ManyM4cs", "S3nsei_404", "BladeX93", 
            "Nitro_101", "MaxR4ge", "ZoneSn1per", "HexKillz", "Recoil99", "Arctic0ne", "K3nshi_X", "FearMe88", "GhostXD23", "PixelPouncer", "TurboToad", 
            "GlitchSnitch", "LootLlama", "ButtonMasher", "CaffeinatedKoala", "SneakyPanda", "QuestQuokka", "N00bCatcher",
            "JumpingJelly", "BananaBlaster", "ChillChameleon", "RespawnRanger", "WaffleWarrior", "StealthSnacc", "MuffinMayhem", "CactusCombo", "DizzyDragon",
            "SnuggleSpartan", "XPWhisperer", "LaserLobster", "BoopMaster", "GameGoblin", "ManaMoose", "CritKitten", "GhostToast", "RubberNinja", "PotionPenguin",
            "TurboSloth", "FluffyFPS", "MarioSpeedwagon", "CtrlAltElite", "PewPewPotato", "Lagzilla", "WinDiesel", "BotanicalByte", "DonkeyPing", "EnderSnacc",
            "MineCrafty", "CouchCommander", "QuestKnight", "PotionMancer", "MageMango", "FireFoxling", "BladeBeetle", "TrollNugget", "GoblinSpark", "PixieTank",
            "ArcaneAce", "DragonDrop", "SnackAttack", "NoScopeNarwhal", "BigBrainy", "CtrlWarlord", "ChillByte", "PingPenguin", "RebootRaccoon", "FragFrog",
            "QueueKoala", "DuckAndCover", "MechaMuffin", "WarpWeasel", "PixelPilot", "GalaxySnacc", "SynthShark", "NeonNomad", "AstroNoob", "DroidDestroyer",
            "BeamBandit", "OrbitOtter", "JellyXP", "SweetLoot", "ToastyTofu", "BlipBloop", "FuzzyPixels", "DizzyDango", "LemonLancer", "NibbleKnight", "CozyCombo",
            "GummyGrenade", "PixelStorm", "CodeCrusher", "EpicEnder", "ByteKnight", "VictoryVibe", "ShadowCircuit", "GameGuppy", "BossBanana", "KnockKnockBoom",
            "FlashFennec", "SneakyFerret", "ArcticAxolotl", "BrawlingBadger", "LavaLynx", "KrakenKid", "JaguarJukes", "SilentSeal", "DireDuck", "EagleXP", "NinjaNewt"
        };
        
        

        public static NameDataManager Instance { get; set; }

        List<string> _generatedNpcNames;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
        }

        public string GetPlayerName()
        {
            return PlayerPrefs.GetString(PlayerName, DefaultPlayerName);
        }

        public void SavePlayerName(string newPlayerName)
        {
            PlayerPrefs.SetString(PlayerName, newPlayerName);
        }

        private void GenerateRandomPlayerNames(int amount)
        {
            _generatedNpcNames = new List<string>();
            List<string> playerNpcNameList = _playerNpcNames.ToList();

            for (int i = 0; i < amount; i++)
            {
                string playerName = playerNpcNameList[Random.Range(0, playerNpcNameList.Count)];
                playerNpcNameList.Remove(playerName);
                _generatedNpcNames.Add(playerName);
            }
        }

        public List<string> GetGeneratedPlayerNpcNames(int amount, bool forceRenew = false)
        {
            if (forceRenew || _generatedNpcNames == null || _generatedNpcNames.Count != amount)
            {
                GenerateRandomPlayerNames(amount);
            }

            return _generatedNpcNames;
        }
    }
}