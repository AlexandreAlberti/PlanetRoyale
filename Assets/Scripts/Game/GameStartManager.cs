using Application.Data;
using Application.Data.Progression;
using Application.Utils;
using Game.Ftue;
using UnityEngine;

namespace Game
{
    public class GameStartManager : MonoBehaviour
    {
        [SerializeField] private GameManager _gameManager;
        [SerializeField] private GameManager _iceHazardGameManager;
        [SerializeField] private GameManager _fireHazardGameManager;
        [SerializeField] private GameManager _dungeonGameManager;
        [SerializeField] private ControlsTutorialGameManager _controlsTutorialGameManager;
        [SerializeField] private MatchTutorialGameManager _matchTutorialGameManager;
        [SerializeField] private FirstMatchGameManager _firstMatchGameManager;
        [SerializeField] private SecondMatchGameManager _secondMatchGameManager;
        [SerializeField] private ThirdMatchGameManager _thirdMatchGameManager;
        [SerializeField] private FourthMatchGameManager _fourthMatchGameManager;

        private void Start()
        {
            QualitySettings.vSyncCount = 0;
            UnityEngine.Application.targetFrameRate = 60;
            FtueManager.Instance.Initialize();
            
            if (GameDataManager.Instance.IsGreenPlanetForcedGame())
            {
                GameDataManager.Instance.SaveIsGreenPlanetForcedGame(false);
                _gameManager.gameObject.SetActive(true);
                _gameManager.InitializeGame();
                return;
            }

            if (GameDataManager.Instance.IsIcePlanetForcedGame())
            {
                GameDataManager.Instance.SaveIsIcePlanetForcedGame(false);
                _iceHazardGameManager.gameObject.SetActive(true);
                _iceHazardGameManager.InitializeGame();
                return;
            }

            if (GameDataManager.Instance.IsFirePlanetForcedGame())
            {
                GameDataManager.Instance.SaveIsFirePlanetForcedGame(false);
                _fireHazardGameManager.gameObject.SetActive(true);
                _fireHazardGameManager.InitializeGame();
                return;
            }

            if (GameDataManager.Instance.IsADungeonGame())
            {
                GameDataManager.Instance.SaveIsAGoldDungeonGame(false);
                _dungeonGameManager.gameObject.SetActive(true);
                _dungeonGameManager.InitializeGame();
                return;
            }

            if (!FtueManager.Instance.HasClearedControlsTutorialLevel())
            {
                _controlsTutorialGameManager.gameObject.SetActive(true);
                _controlsTutorialGameManager.InitializeGame();
                return;
            }

            if (!FtueManager.Instance.HasClearedMatchTutorialLevel())
            {
                _matchTutorialGameManager.gameObject.SetActive(true);
                _matchTutorialGameManager.InitializeGame();
                return;
            }

            if (FtueManager.Instance.HasFinishedFirstRealGame()
                && !FtueManager.Instance.HasFinishedSecondRealGame())
            {
                _firstMatchGameManager.gameObject.SetActive(true);
                _firstMatchGameManager.InitializeGame();
                return;
            }

            if (FtueManager.Instance.HasFinishedFirstRealGame()
                && FtueManager.Instance.HasFinishedSecondRealGame()
                && !FtueManager.Instance.HasFinishedThirdRealGame())
            {
                _secondMatchGameManager.gameObject.SetActive(true);
                _secondMatchGameManager.InitializeGame();
                return;
            }

            if (FtueManager.Instance.HasFinishedFirstRealGame()
                && FtueManager.Instance.HasFinishedSecondRealGame()
                && FtueManager.Instance.HasFinishedThirdRealGame()
                && !FtueManager.Instance.HasFinishedFourthRealGame())
            {
                _thirdMatchGameManager.gameObject.SetActive(true);
                _thirdMatchGameManager.InitializeGame();
                return;
            }

            if (FtueManager.Instance.HasFinishedFirstRealGame()
                && FtueManager.Instance.HasFinishedSecondRealGame()
                && FtueManager.Instance.HasFinishedThirdRealGame()
                && FtueManager.Instance.HasFinishedFourthRealGame()
                && !FtueManager.Instance.HasFinishedFtue())
            {
                _fourthMatchGameManager.gameObject.SetActive(true);
                _fourthMatchGameManager.InitializeGame();
                return;
            }

            switch (Random.Range(NumberConstants.Zero, NumberConstants.Three))
            {
                case NumberConstants.Zero:
                    _iceHazardGameManager.gameObject.SetActive(true);
                    _iceHazardGameManager.InitializeGame();
                    break;
                case NumberConstants.One:
                    _fireHazardGameManager.gameObject.SetActive(true);
                    _fireHazardGameManager.InitializeGame();
                    break;
                default:
                    _gameManager.gameObject.SetActive(true);
                    _gameManager.InitializeGame();
                    break;
            }
        }
    }
}