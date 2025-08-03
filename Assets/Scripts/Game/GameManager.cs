using System.Collections;
using System.Collections.Generic;
using Application.Data;
using Application.Data.Equipment;
using Application.Data.Progression;
using Application.Sound;
using Application.Telemetry;
using Game.Camera;
using Game.Drop;
using Game.EnemyUnit;
using Game.Input;
using Game.Map;
using Game.Match;
using Game.PlayerUnit;
using Game.Pool;
using Game.Ranking;
using Game.Reward;
using Game.UI;
using UI.ScreenFader;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private Planet _planetPrefab;
        [SerializeField] private GameObject _planetHaloPrefab;
        
        protected const float LoadDuration = 0.5f;
        private const float ZoomIntroPercentage = 0.8f;
        private const float TimeBeforeZoomOut = 0.2f;
        private const float TimeBeforeStartAnimation = 0.5f;
        private const float TimeBeforeOverlay = 1.0f;
        private const float TimeBeforeEndAnimation = 1.0f;

        public static GameManager Instance { get; private set; }
        
        private Dictionary<EquipmentPieceType, int> _equipmentMaterialsRewards;
        protected Vector3 _sphereCenter;
        protected float _sphereRadius;

        private void Awake()
        {
            Instance = this;
        }

        public virtual void InitializeGame()
        {
            ObjectPool.Instance.Initialize();
            RewardManager.Instance.Initialize();
            Joystick.Instance.Initialize();
            InputManager.Instance.Initialize();
            Planet planet = Instantiate(_planetPrefab);
            Instantiate(_planetHaloPrefab);
            _sphereCenter = planet.transform.position;
            _sphereRadius = planet.GetPlanetRadius();
            MatchManager.Instance.OnCountDownEnded += MatchManager_OnCountDownEnded;
            EnemyManager.Instance.Initialize(_sphereCenter, _sphereRadius);
            PlayerManager.Instance.Initialize(_sphereCenter, _sphereRadius);
            PlayerManager.Instance.OnPlayerDead += PlayerManager_OnPlayerDead;
            CameraManager.Instance.Initialize();
            UIManager.Instance.Initialize(_sphereCenter, _sphereRadius);
            UIManager.Instance.OnLeaveButtonClicked += UIManager_OnLeaveButtonClicked;
            UIManager.Instance.OnAcceptedRewards += UIManager_OnAcceptedRewards;
            InitializeDrops();
            EnvironmentManager.Instance.Initialize();

            RankingManager.Instance.Initialize(PlayerManager.Instance.GetPlayersList());
            RankingManager.Instance.Enable();

            int leagueIndex = LeaguesManager.Instance.GetCurrentLeagueIndex();
            AmplitudeManager.Instance.GameStart(ProgressionDataManager.Instance.GetLeagueAttempts(leagueIndex), _planetPrefab.gameObject.name);
            
            StartCoroutine(StartGame());
        }

        protected virtual void InitializeDrops()
        {
            DropManager.Instance.Initialize();
        }
        
        private void PlayerManager_OnPlayerDead(int playerIndex)
        {
            if (playerIndex == Player.GetHumanPlayerIndex())
            {
                StartCoroutine(EndGameByHumanPlayerKilled(playerIndex));
                return;
            }

            if (PlayerManager.Instance.GetAmountOfAlivePlayers() == 1)
            {
                StartCoroutine(EndGameByLastPlayerStanding(playerIndex));
                return;
            }

            UIManager.Instance.ShowPlayerKilledStatusPanel(playerIndex);
        }

        private IEnumerator EndGameByLastPlayerStanding(int playerIndex)
        {
            EndGame(LevelResult.LastPlayerStanding);
            UIManager.Instance.ShowPlayerKilledStatusPanel(playerIndex);
            yield return new WaitForSeconds(UIManager.Instance.GetPlayerKilledStatusPanelDuration(playerIndex) + TimeBeforeOverlay);
            UIManager.Instance.ShowSurvivorOverlay();
            yield return new WaitForSeconds(UIManager.Instance.GetOverlayDuration() + TimeBeforeEndAnimation);
            ShowMatchResultsScreen(false);
        }

        private IEnumerator EndGameByHumanPlayerKilled(int playerIndex)
        {
            yield return new WaitForEndOfFrame();
            EndGame(LevelResult.Death);
            UIManager.Instance.ShowPlayerKilledStatusPanel(playerIndex);
            yield return new WaitForSeconds(UIManager.Instance.GetPlayerKilledStatusPanelDuration(playerIndex) + TimeBeforeOverlay);
            UIManager.Instance.ShowDefeatedOverlay();
            yield return new WaitForSeconds(UIManager.Instance.GetOverlayDuration() + TimeBeforeEndAnimation);
            ShowMatchResultsScreen(true);
        }

        protected virtual void  ShowMatchResultsScreen(bool isHumanPlayerDead)
        {
            UIManager.Instance.ShowMatchResultsScreen(isHumanPlayerDead);
        }
        
        private void MatchManager_OnCountDownEnded()
        {
            StartCoroutine(EndGameByTimer());
        }

        private IEnumerator EndGameByTimer()
        {
            EndGame(LevelResult.TimeUp);
            UIManager.Instance.ShowTimeIsUpOverlay();
            yield return new WaitForSeconds(UIManager.Instance.GetOverlayDuration() + TimeBeforeEndAnimation);
            ShowMatchResultsScreen(false);
        }

        private void EndGame(LevelResult levelResult)
        {
            int playerRank = RankingManager.Instance.GetHumanPlayerRank();
            InputManager.Instance.Disable();
            Joystick.Instance.Disable();
            EnemyManager.Instance.Disable();
            PlayerManager.Instance.Disable();
            RankingManager.Instance.Disable();
            UIManager.Instance.HideInteractableUI();
            MatchManager.Instance.EndGame();
            GiveGameRewards(playerRank);
            SendAmplitudeGameEndEvent(levelResult, playerRank);
            StartCoroutine(WaitAndUpdateRankingPanel());
        }

        protected virtual void GiveGameRewards(int playerRank)
        {
            RewardManager.Instance.ReceiveGameRewards(playerRank);
        }

        private IEnumerator WaitAndUpdateRankingPanel()
        {
            yield return new WaitForEndOfFrame();
            RankingManager.Instance.UpdateRankingPanel();
        }
        
        private IEnumerator StartGame()
        {
            StartCoroutine(InitialZoomOut());
            yield return new WaitForSeconds(TimeBeforeStartAnimation);
            ShowStartAnimation();
        }

        protected virtual IEnumerator InitialZoomOut()
        {
            float enemyDetectionDistance = PlayerManager.Instance.GetHumanPlayer().GetEnemyDetectionDistance();
            CameraManager.Instance.ForceOffset(enemyDetectionDistance * ZoomIntroPercentage);
            ScreenFader.Instance.FadeOut();
            yield return new WaitForSeconds(TimeBeforeZoomOut);
            CameraManager.Instance.UpdateOffset(enemyDetectionDistance);
        }

        protected virtual void ShowStartAnimation()
        {
            UIManager.Instance.ShowStartGameAnimation();
            UIManager.Instance.OnStartGameAnimationEnded += UIManager_OnStartGameAnimationEnded;
        }

        protected void UIManager_OnStartGameAnimationEnded()
        {
            GameMatchStart();
        }

        public void GameMatchStart()
        {
            MatchManager.Instance.Initialize();
            MatchManager.Instance.StartGame();
            MusicManager.Instance.PlayGameMusic();
            InputManager.Instance.Enable();
            Joystick.Instance.Enable();
            EnemyManager.Instance.Enable();
            EnemyManager.Instance.SpawnInitialBatch();
            PlayerManager.Instance.Enable();
        }

        private void UIManager_OnLeaveButtonClicked()
        {
            UIManager.Instance.OnLeaveButtonClicked -= UIManager_OnLeaveButtonClicked;
            Joystick.Instance.Disable();
            int playerRankIsLast = PlayerManager.Instance.GetPlayersList().Count - 1;
            SendAmplitudeGameEndEvent(LevelResult.Exit, playerRankIsLast);
            RewardManager.Instance.ReceiveGameRewards(playerRankIsLast);
            StartCoroutine(LoadNextScene());
        }

        private void UIManager_OnAcceptedRewards()
        {
            UIManager.Instance.OnAcceptedRewards -= UIManager_OnAcceptedRewards;
            Joystick.Instance.Disable();
            StartCoroutine(LoadNextScene());
        }

        protected virtual IEnumerator LoadNextScene()
        {
            DisableDrops();
            EnvironmentManager.Instance.ResetEnvironment();
            ScreenFader.Instance.FadeIn(true);
            MusicManager.Instance.FadeOutMusic();
            yield return new WaitForSeconds(LoadDuration);
            SceneManager.LoadScene((int) SceneEnum.MainMenu);
        }
        
        protected virtual void DisableDrops()
        {
            DropManager.Instance.Disable();
        }
        
        private void SendAmplitudeGameEndEvent(LevelResult levelResult, int rank)
        {
            int leagueIndex = LeaguesManager.Instance.GetCurrentLeagueIndex();
            int gamesPlayed = ProgressionDataManager.Instance.IncrementLeagueAttempts(leagueIndex);
            int secondsPassed = MatchManager.Instance.GetSecondsPassed();

            Player player = PlayerManager.Instance.GetHumanPlayer();
            int kills = player.Kills();
            int playerHealth = player.CurrentHealth();
            int playerScore = RankingManager.Instance.GetPlayerScore(rank);
            int bossKills = player.BossKills();
            
            AmplitudeManager.Instance.GameEnd(_planetPrefab.gameObject.name, leagueIndex, levelResult.ToString(), rank,  gamesPlayed, secondsPassed, kills, playerHealth, playerScore, bossKills);
        }

        public void StopGameplay()
        {
            InputManager.Instance.Disable();
            Joystick.Instance.Disable();
            EnemyManager.Instance.Disable();
            PlayerManager.Instance.Disable();
            RankingManager.Instance.Disable();
        }

        public void ResumeGameplay()
        {
            InputManager.Instance.Enable();
            Joystick.Instance.Enable();
            EnemyManager.Instance.Enable();
            PlayerManager.Instance.Enable();
            RankingManager.Instance.Enable();
        }
    }
}