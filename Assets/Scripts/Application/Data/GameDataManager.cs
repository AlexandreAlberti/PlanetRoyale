using UnityEngine;

namespace Application.Data
{
    public class GameDataManager : MonoBehaviour
    {
        private const string FtueStep = "FtueStep";
        private const string GoldDungeonGame = "GoldDungeonGame";
        private const string FirePlanetForcedGame = "FirePlanetForcedGame";
        private const string IcePlanetForcedGame = "IcePlanetForcedGame";
        private const string GreenPlanetForcedGame = "GreenPlanetForcedGame";

        public static GameDataManager Instance { get; set; }

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

        public void SaveFtueStep(int step)
        {
            PlayerPrefs.SetInt(FtueStep, step);
        }

        public int GetFtueStep()
        {
            return PlayerPrefs.GetInt(FtueStep);
        }

        public void Clear()
        {
            SaveFtueStep(0);
        }

        public bool IsADungeonGame()
        {
            return PlayerPrefs.GetInt(GoldDungeonGame) == 1;
        }

        public void SaveIsAGoldDungeonGame(bool isADungeonGame)
        {
            PlayerPrefs.SetInt(GoldDungeonGame, isADungeonGame ? 1 : 0);
        }

        public bool IsGreenPlanetForcedGame()
        {
            return PlayerPrefs.GetInt(GreenPlanetForcedGame) == 1;
        }
        
        public void SaveIsGreenPlanetForcedGame(bool isGreenPlanetForcedGame)
        {
            PlayerPrefs.SetInt(GreenPlanetForcedGame, isGreenPlanetForcedGame ? 1 : 0);
        }
        
        public bool IsIcePlanetForcedGame()
        {
            return PlayerPrefs.GetInt(IcePlanetForcedGame) == 1;
        }
        
        public void SaveIsIcePlanetForcedGame(bool isIcePlanetForcedGame)
        {
            PlayerPrefs.SetInt(IcePlanetForcedGame, isIcePlanetForcedGame ? 1 : 0);
        }
        
        public bool IsFirePlanetForcedGame()
        {
            return PlayerPrefs.GetInt(FirePlanetForcedGame) == 1;
        }
        
        public void SaveIsFirePlanetForcedGame(bool isFirePlanetForcedGame)
        {
            PlayerPrefs.SetInt(FirePlanetForcedGame, isFirePlanetForcedGame ? 1 : 0);
        }
    }
}