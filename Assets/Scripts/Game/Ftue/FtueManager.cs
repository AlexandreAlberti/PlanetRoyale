using System.Collections;
using System.Collections.Generic;
using Application.Data;
using Application.Data.Currency;
using Application.Data.Equipment;
using Application.Data.Progression;
using Application.Data.Rewards;
using Application.Data.Talent;
using Application.Sound;
using Application.Telemetry;
using Game.Camera;
using Game.Drop;
using Game.EnemyUnit;
using Game.Equipment;
using Game.Input;
using Game.Map;
using Game.Match;
using Game.PlayerUnit;
using Game.UI;
using TMPro;
using UI.Button;
using UI.MainMenu;
using UI.ScreenFader;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game.Ftue
{
    public class FtueManager : MonoBehaviour
    {
        [Header("Skip FTUE")]
        [SerializeField] private bool _skipFtue;

        [Header("Menu")]
        [SerializeField] private TalentsData _talentsData;
        [SerializeField] private LeagueData _firstLeagueData;
        [SerializeField] private LeagueData _secondLeagueData;
        [SerializeField] private LeagueData _thirdLeagueData;
        [SerializeField] private EquipmentPieceData _equipmentPieceData;
        [SerializeField] private InteractableButton _playButton;
        [SerializeField] private InteractableButton _talentsButton;
        [SerializeField] private InteractableButton _upgradeTalentsButton;
        [SerializeField] private TalentsScreen _talentsScreen;
        [SerializeField] private InteractableButton _battleButton;
        [SerializeField] private InteractableButton _leagueUpCloseButton;
        [SerializeField] private InteractableButton _leagueUpBackgroundButton;
        [SerializeField] private InteractableButton _equipmentButton;
        [SerializeField] private EquipmentScreen _equipmentScreen;
        [SerializeField] private InteractableButton _equipmentPieceEquipButton;
        [SerializeField] private InteractableButton _weaponSlotButton;
        [SerializeField] private InteractableButton _equipmentPieceUpgradeButton;
        [SerializeField] private EquipmentPieceDetail _equipmentDetailPopup;
        [SerializeField] private InteractableButton _equipmentMergeButton;
        [SerializeField] private EquipmentMergeScreen _equipmentMergeScreen;
        [SerializeField] private InteractableButton _equipmentConfirmMergeButton;
        [SerializeField] private InteractableButton _equipmentMergeBackButton;
        [SerializeField] private MergeAnimationScreen _mergeAnimationScreen;
        [SerializeField] private InteractableButton _changeNameButton;
        [SerializeField] private Popup _changeNamePopup;
        [SerializeField] private InteractableButton _dungeonButton;
        [SerializeField] private InteractableButton _goldDungeonButton;
        [SerializeField] private InteractableButton _startDungeonButton;
        [SerializeField] private InteractableButton _scoutButton;
        [SerializeField] private InteractableButton _scoutClaimButton;
        [SerializeField] private InteractableButton _scoutQuickButton;
        [SerializeField] private InteractableButton _scoutQuickEnergyButton;
        [SerializeField] private ScoutRewardsPanel _scoutRewardsPanel;
        [SerializeField] private QuickScoutPanel _quickScoutPanel;
        [SerializeField] private ScoutPanel _scoutPanel;

        [Header("Menu Pointer Masks")]
        [SerializeField] private Animator _pointerMaskBigAnimator;
        [SerializeField] private Animator _pointerMaskWideAnimator;
        [SerializeField] private Animator _pointerMaskSquareAnimator;
        [SerializeField] private Animator _pointerMaskMidAnimator;
        [SerializeField] private Animator _pointerMaskBigHorizontalAnimator;
        [SerializeField] private GameObject _pointerMaskBig;
        [SerializeField] private GameObject _pointerMaskWide;
        [SerializeField] private GameObject _pointerMaskSquare;
        [SerializeField] private GameObject _pointerMaskMid;
        [SerializeField] private GameObject _pointerMaskBigHorizontal;

        [Header("Menu Pointer Indicators")]
        [SerializeField] private Animator _pointerDownIndicatorAnimator;
        [SerializeField] private Animator _pointerUpIndicatorAnimator;
        [SerializeField] private GameObject _pointerDownIndicator;
        [SerializeField] private GameObject _pointerUpIndicator;

        [Header("Tap Blocker")]
        [SerializeField] private GameObject _tapBlocker;

        [Header("Game")] 
        [SerializeField] private GameObject _canvas;
        [SerializeField] private GameObject _background;
        [SerializeField] private GameObject _character;
        [SerializeField] private GameObject _tapToContinue;
        [SerializeField] private InteractableButton _clickableArea;
        [SerializeField] private Animator _backgroundAnimator;
        [SerializeField] private Animator _characterAnimator;
        [SerializeField] private Animator _bubbleTextAnimator;
        [SerializeField] private Animator _textAnimator;
        [SerializeField] private Animator _tapToContinueAnimator;
        [SerializeField] private TextMeshProUGUI _bubbleText;
        [SerializeField] private Animator _tutorialBottomTextAnimator;
        [SerializeField] private TextMeshProUGUI _tutorialBottomText;
        [SerializeField] private Animator _tutorialTopTextAnimator;
        [SerializeField] private TextMeshProUGUI _tutorialTopText;
        [SerializeField] private Animator _tutorialMoveIn8Animator;
        [SerializeField] private Animator _tutorialStopToMoveAnimator;
        [SerializeField] private Image _tutorialStopToMoveRedCross;
        [SerializeField] private GameObject _rankingTarget;
        [SerializeField] private InteractableButton _firstSkillTargetButton;

        private static readonly int AppearTrigger = Animator.StringToHash("Appear");
        private static readonly int HideTrigger = Animator.StringToHash("Hide");
        private static readonly int TalkTrigger = Animator.StringToHash("Talk");

        private const string WelcomeText = "Welcome to Planet Royale";
        private const string ControlsTutorialDragToMoveText = "Drag To Move";
        private const string ControlsTutorialStopToAttackText = "Stop to attack";
        private const string ControlsTutorialPointsIntroductionText = "Score points defeating enemies";
        private const string ControlsTutorialPointsXpDropsText = "Collect Gems to score more points!!!";
        private const string ControlsTutorialPointsXpDropsTextHint = "Collect the Gem";
        private const string ControlsTutorialLevelUpText = "Select \"Attack Range\"!";
        private const string ControlsTutorialFinalWaveText = "Finish them off!!!";
        private const string ControlsTutorialWellDoneText = "Well done!";
        private const string MatchTutorialScorePointsText = "Score the most points to win!";
        private const string MatchTutorialCollectGemsText = "Collect Gems to earn more points!";
        private const string MatchTutorialBossComingText = "A Boss comes to break the planet!";
        private const string MatchTutorialSlayerPowerText = "Boss defeated!<br><br>Enjoy the Slayer Power!";
        private static readonly string MatchTutorialFinishThemText = $"Kill {TargetEnemyKills} enemies!";
        private const string MatchTutorialYouDidItText = "You did it! Look at that score!";
        private const string NewPlanetText = "Nice! You unlocked a new Planet!";
        private const string PutGoldToUseText = "Also, let's put that Gold to use!";
        private const string PowerfulBoonText = "A powerful boon!";
        private const string NewWeaponText = "You unlocked a new weapon! Let's equip it!";
        private const string LooksMenacingText = "WOW! Let's try it collecting Gold!";
        private const string FoundSwordsText = "I've found these 2 rusty swords!";
        private const string ImproveSwordText = "Let's use them to improve your current sword!";
        private const string WayBetterText = "That looks way better!";
        private const string ScoutText = "Scout gives you rewards over time!";
        private const string ScoutEnergyText = "You can also spend energy to get rewards!";
        private const string ChangeNameInitialText = "You are a rising star! How should legends name you?";
        private const string ChangeNameFinalText = "Keep conquering world after world and become a Planet Royale legend!";

        private const float UpgradePieceYOffset = 50.0f;
        private const float ScoutButtonYOffset = 90.0f;
        private const float ShortStepTime = 0.25f;
        private const float MediumStepTime = 0.5f;
        private const float LongStepTime = 1.0f;
        private const float MinTimeToMoveToCompleteStep3 = 1.3f;

        private const int SplashScreenStep = 0;
        public const int FinishedControlsTutorialStep = 100;
        public const int FinishedMatchTutorialStep = 200;
        public const int FinishedFirstRealGameStep = 201;
        public const int FinishedLeagueOneUpStep = 203;
        public const int FinishedTalentsTutorialStep = 300;
        public const int FinishedSecondRealGameStep = 301;
        public const int FinishedLeagueTwoUpStep = 400;
        public const int FinishedEquipmentTutorialStep = 500;
        public const int FinishedGoldDungeonStep = 600;
        public const int FinishedMergeTutorialStep = 700;
        public const int FinishedThirdRealGameStep = 701;
        public const int FinishedScoutTutorialStep = 800;
        public const int FinishedChangeNameTutorialStep = 900;
        public const int FinishedFourthRealGameStep = 901;
        public const int FinalStep = 999;
        private const int TargetEnemyKills = 5;

        private int _currentStep;
        private int _stepToComplete;
        private float _controlTutorialMoveAmount;
        private float _controlWaveRemainingDeads;
        private float _lastStepStartTime;
        private int _enemyKillCounter;

        public static FtueManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        public void Initialize()
        {
            _canvas.SetActive(false);

            if (_skipFtue)
            {
                GameDataManager.Instance.SaveFtueStep(FinalStep + 1);
            }

            _lastStepStartTime = Time.time;
            _currentStep = GameDataManager.Instance.GetFtueStep();
            _clickableArea.gameObject.SetActive(false);
        }

        public bool HasClearedControlsTutorialLevel()
        {
            return GameDataManager.Instance.GetFtueStep() >= FinishedControlsTutorialStep;
        }

        public bool HasClearedMatchTutorialLevel()
        {
            return GameDataManager.Instance.GetFtueStep() >= FinishedMatchTutorialStep;
        }

        public bool HasFinishedFirstRealGame()
        {
            return GameDataManager.Instance.GetFtueStep() >= FinishedFirstRealGameStep;
        }

        public bool HasFinishedSecondRealGame()
        {
            return GameDataManager.Instance.GetFtueStep() >= FinishedSecondRealGameStep;
        }

        public bool HasFinishedThirdRealGame()
        {
            return GameDataManager.Instance.GetFtueStep() >= FinishedThirdRealGameStep;
        }

        public bool HasFinishedFourthRealGame()
        {
            return GameDataManager.Instance.GetFtueStep() >= FinishedFourthRealGameStep;
        }

        public bool HasFinishedFtue()
        {
            return GameDataManager.Instance.GetFtueStep() >= FinalStep;
        }

        public void ShowNextStep()
        {
            Debug.Log($"ShowNextStep: {_currentStep}");
            float now = Time.time;
            float timeUsed = now - _lastStepStartTime;
            _lastStepStartTime = now;
            AmplitudeManager.Instance.TutorialStep(_currentStep > FinishedMergeTutorialStep, false, _currentStep, GetTutorialStepName(_currentStep), UnityEngine.Application.version, timeUsed);

            switch (_currentStep)
            {
                case SplashScreenStep:
                    StartCoroutine(FirstDialogChoreography(WelcomeText));
                    break;
                case 1:
                    StartCoroutine(NextDialogChoreography(ControlsTutorialDragToMoveText));
                    break;
                case 2:
                    StartCoroutine(EndDialogChoreography());
                    break;
                case 3:
                    ShowMoveSymbolChoreography();
                    break;
                case 4:
                    GameManager.Instance.StopGameplay();
                    StartCoroutine(FirstEnemySpawnChoreography());
                    break;
                case 5:
                    StartCoroutine(FirstDialogChoreography(ControlsTutorialStopToAttackText));
                    break;
                case 6:
                    StartCoroutine(EndDialogChoreography());
                    break;
                case 7:
                    GameManager.Instance.ResumeGameplay();
                    ShowStopToAttackChoreography();
                    break;
                case 8:
                    GameManager.Instance.StopGameplay();
                    StartCoroutine(FirstDialogChoreography(ControlsTutorialPointsIntroductionText));
                    break;
                case 9:
                    StartCoroutine(SetMaskOverXpItem());
                    break;
                case 10:
                    StartCoroutine(NextDialogChoreography(ControlsTutorialPointsXpDropsText));
                    break;
                case 11:
                    StartCoroutine(EndDialogChoreography());
                    break;
                case 12:
                    GameManager.Instance.ResumeGameplay();
                    CollectTagChoreography();
                    break;
                case 13:
                    GameManager.Instance.StopGameplay();
                    StartCoroutine(FirstDialogChoreography(ControlsTutorialLevelUpText));
                    break;
                case 14:
                    StartCoroutine(EndDialogChoreography());
                    break;
                case 15:
                    StartCoroutine(RangeUpSkillButtonChoreography());
                    break;
                case 16:
                    StartCoroutine(FirstDialogChoreography(ControlsTutorialFinalWaveText));
                    break;
                case 17:
                    StartCoroutine(EndDialogChoreography());
                    StartCoroutine(RemoveObstaclesChoreography());
                    break;
                case 18:
                    _canvas.SetActive(false);
                    GameManager.Instance.ResumeGameplay();
                    StartCoroutine(SecondEnemySpawnChoreography());
                    break;
                case 19:
                    GameManager.Instance.StopGameplay();
                    StartCoroutine(FirstDialogChoreography(ControlsTutorialWellDoneText));
                    break;
                case 20:
                    StartCoroutine(EndDialogChoreography());
                    break;
                case 21:
                    StartCoroutine(LoadMatchTutorial());
                    break;
                case FinishedControlsTutorialStep:
                    StartCoroutine(FirstDialogChoreography(MatchTutorialScorePointsText));
                    StartCoroutine(GameRankNumberChoreography());
                    break;
                case 101:
                    StartCoroutine(NextDialogChoreography(MatchTutorialCollectGemsText));
                    break;
                case 102:
                    StartCoroutine(EndDialogChoreography());
                    break;
                case 103:
                    StartCoroutine(StartGameChoreography());
                    break;
                case 104:
                    StartCoroutine(FirstDialogChoreography(MatchTutorialBossComingText));
                    break;
                case 105:
                    StartCoroutine(EndDialogChoreography());
                    break;
                case 106:
                    StartCoroutine(BossAppearChoreography());
                    break;
                case 107:
                    StartCoroutine(BossPowerChoreography());
                    break;
                case 108:
                    GameManager.Instance.StopGameplay();
                    StartCoroutine(FirstDialogChoreography(MatchTutorialSlayerPowerText));
                    break;
                case 109:
                    StartCoroutine(NextDialogChoreography(MatchTutorialFinishThemText));
                    break;
                case 110:
                    StartCoroutine(EndDialogChoreography());
                    break;
                case 111:
                    StartCoroutine(KillAllEnemiesChoreography());
                    break;
                case 112:
                    GameManager.Instance.StopGameplay();
                    StartCoroutine(FirstDialogChoreography(MatchTutorialYouDidItText));
                    StartCoroutine(GameRankNumberChoreography());
                    break;
                case 113:
                    StartCoroutine(EndDialogChoreography());
                    break;
                case 114:
                    StartCoroutine(TutorialRewardsChoreography(FinishedMatchTutorialStep));
                    break;
                case FinishedMatchTutorialStep:
                    StartCoroutine(MenuPlayButtonChoreography());
                    break;
                case FinishedFirstRealGameStep:
                    CurrencyDataManager.Instance.Clear();
                    CurrencyDataManager.Instance.GainGold(_talentsData.GoldCostPerUpgrade[0]);
                    CurrencyDataManager.Instance.ForceTrophies(_firstLeagueData.MinTrophiesInclusive);
                    CurrencyDataManager.Instance.UpdateTrophies(_firstLeagueData.MaxTrophiesExclusive - _firstLeagueData.MinTrophiesInclusive);
                    RewardUIParticleManager.Instance.ForceTrophiesWonAmount(_firstLeagueData.MaxTrophiesExclusive - _firstLeagueData.MinTrophiesInclusive);
                    RewardUIParticleManager.Instance.ForceTrophiesParticles(_firstLeagueData.MaxTrophiesExclusive - _firstLeagueData.MinTrophiesInclusive);
                    ProgressionDataManager.Instance.Clear();
                    _canvas.SetActive(true);
                    _tapBlocker.SetActive(true);
                    LeagueScreenButton.OnTrophiesWonChoreographyEnded += LeagueScreenButton_OnLeagueOneTrophiesWonChoreographyEnded;
                    break;
                case 202:
                    MenuLeagueOneClosePopupButtonChoreography();
                    break;
                case FinishedLeagueOneUpStep:
                    CurrencyDataManager.Instance.Clear();
                    CurrencyDataManager.Instance.GainGold(_talentsData.GoldCostPerUpgrade[0]);
                    CurrencyDataManager.Instance.ForceTrophies(_firstLeagueData.MaxTrophiesExclusive);
                    TalentsDataManager.Instance.Clear();
                    StartCoroutine(FirstDialogChoreography(NewPlanetText));
                    break;
                case 204:
                    StartCoroutine(NextDialogChoreography(PutGoldToUseText));
                    break;
                case 205:
                    StartCoroutine(EndDialogChoreography());
                    break;
                case 206:
                    StartCoroutine(MenuNavigationBarTalentsChoreography());
                    break;
                case 207:
                    StartCoroutine(MenuUpgradeTalentChoreography());
                    break;
                case 208:
                    StartCoroutine(FirstDialogChoreography(PowerfulBoonText));
                    break;
                case 209:
                    StartCoroutine(EndDialogChoreography());
                    break;
                case 210:
                    StartCoroutine(MenuNavigationBarBattleChoreography(FinishedTalentsTutorialStep));
                    break;
                case FinishedTalentsTutorialStep:
                    StartCoroutine(MenuPlayButtonChoreography());
                    break;
                case FinishedSecondRealGameStep:
                    CurrencyDataManager.Instance.Clear();
                    CurrencyDataManager.Instance.ForceTrophies(_secondLeagueData.MinTrophiesInclusive);
                    CurrencyDataManager.Instance.UpdateTrophies(_secondLeagueData.MaxTrophiesExclusive - _secondLeagueData.MinTrophiesInclusive);
                    RewardUIParticleManager.Instance.ForceTrophiesWonAmount(_secondLeagueData.MaxTrophiesExclusive - _secondLeagueData.MinTrophiesInclusive);
                    RewardUIParticleManager.Instance.ForceTrophiesParticles(_secondLeagueData.MaxTrophiesExclusive - _secondLeagueData.MinTrophiesInclusive);
                    ProgressionDataManager.Instance.Clear();
                    ProgressionDataManager.Instance.ReceiveLeagueReward(0);
                    _canvas.SetActive(true);
                    _tapBlocker.SetActive(true);
                    LeagueScreenButton.OnTrophiesWonChoreographyEnded += LeagueScreenButton_OnLeagueTwoTrophiesWonChoreographyEnded;
                    break;
                case 302:
                    MenuLeagueTwoClosePopupButtonChoreography();
                    break;
                case FinishedLeagueTwoUpStep:
                    CurrencyDataManager.Instance.Clear();
                    CurrencyDataManager.Instance.ForceTrophies(_secondLeagueData.MaxTrophiesExclusive);
                    EquipmentDataManager.Instance.Clear();
                    EquipmentDataManager.Instance.GiveInitialEquipment();
                    EquipmentDataManager.Instance.AddPieceToInventory(EquipmentPieceFactory.GenerateEquipmentPieceById(_equipmentPieceData.Id));
                    CurrencyDataManager.Instance.GainMaterials(_equipmentPieceData.EquipmentPieceType, EquipmentDataManager.Instance.GetEquipmentSlotData().MaterialUpgradeCostPerLevel[0]);
                    CurrencyDataManager.Instance.GainGold(EquipmentDataManager.Instance.GetEquipmentSlotData().GoldUpgradeCostPerLevel[0]);
                    StartCoroutine(FirstDialogChoreography(NewWeaponText));
                    break;
                case 401:
                    StartCoroutine(EndDialogChoreography());
                    break;
                case 402:
                    StartCoroutine(MenuNavigationBarEquipmentChoreography());
                    break;
                case 403:
                    StartCoroutine(MenuEquipmentPieceButtonChoreography());
                    break;
                case 404:
                    StartCoroutine(MenuEquipmentPieceEquipButtonChoreography());
                    break;
                case 405:
                    StartCoroutine(MenuEquipmentSlotButtonChoreography());
                    break;
                case 406:
                    StartCoroutine(MenuEquipmentSlotUpgradeButtonChoreography());
                    break;
                case 407:
                    StartCoroutine(MenuEquipmentDetailPopupCloseChoreography());
                    break;
                case 408:
                    StartCoroutine(FirstDialogChoreography(LooksMenacingText));
                    break;
                case 409:
                    StartCoroutine(EndDialogChoreography(FinishedEquipmentTutorialStep));
                    break;
                case FinishedEquipmentTutorialStep:
                    DungeonTicketsDataManager.Instance.EmptyGoldDungeonTickets();
                    DungeonTicketsDataManager.Instance.AddGoldDungeonTickets(1);
                    StartCoroutine(MenuNavigationBarDungeonChoreography());
                    break;
                case 501:
                    StartCoroutine(MenuGoldDungeonButtonChoreography());
                    break;
                case 502:
                    StartCoroutine(MenuStartGoldDungeonButtonChoreography());
                    break;
                case FinishedGoldDungeonStep:
                    EquipmentDataManager.Instance.Clear();
                    EquipmentDataManager.Instance.GiveInitialEquipment();
                    EquipmentDataManager.Instance.LevelUpSlot(EquipmentPieceType.Weapon);
                    EquipmentDataManager.Instance.AddPieceToInventory(EquipmentPieceFactory.GenerateEquipmentPieceById(_equipmentPieceData.Id));
                    EquipmentDataManager.Instance.AddPieceToInventory(EquipmentPieceFactory.GenerateEquipmentPieceById(_equipmentPieceData.Id));
                    EquipmentPieceInstance initialWeapon = EquipmentPieceFactory.GenerateEquipmentPieceById(_equipmentPieceData.Id);
                    EquipmentDataManager.Instance.AddPieceToInventory(initialWeapon);
                    EquipmentDataManager.Instance.EquipPiece(initialWeapon);
                    StartCoroutine(FirstDialogChoreography(FoundSwordsText));
                    break;
                case 601:
                    StartCoroutine(NextDialogChoreography(ImproveSwordText));
                    break;
                case 602:
                    StartCoroutine(EndDialogChoreography());
                    break;
                case 603:
                    StartCoroutine(MenuNavigationBarEquipmentChoreography());
                    break;
                case 604:
                    StartCoroutine(MenuEquipmentMergeButtonChoreography());
                    break;
                case 605:
                    StartCoroutine(MenuEquipmentPartButtonMergeChoreography(_equipmentMergeScreen.FirstMergePiece()));
                    break;
                case 606:
                    StartCoroutine(MenuEquipmentPartButtonMergeChoreography(_equipmentMergeScreen.SecondMergePiece()));
                    break;
                case 607:
                    StartCoroutine(MenuEquipmentPartButtonMergeChoreography(_equipmentMergeScreen.ThirdMergePiece()));
                    break;
                case 608:
                    StartCoroutine(MenuEquipmentConfirmMergeChoreography());
                    break;
                case 609:
                    StartCoroutine(FirstDialogChoreography(WayBetterText));
                    break;
                case 610:
                    StartCoroutine(EndDialogChoreography());
                    break;
                case 611:
                    StartCoroutine(MenuEquipmentMergeBackButtonChoreography());
                    break;
                case 612:
                    StartCoroutine(MenuNavigationBarBattleChoreography(FinishedMergeTutorialStep));
                    break;
                case FinishedMergeTutorialStep:
                    StartCoroutine(MenuPlayButtonChoreography());
                    break;
                case FinishedThirdRealGameStep:
                    ScoutDataManager.Instance.ForceScoutTime(7200);
                    ScoutDataManager.Instance.SetInitialQuickScoutDailyClaims();
                    StartCoroutine(MenuScoutButtonChoreography());
                    break;
                case 702:
                    StartCoroutine(FirstDialogChoreography(ScoutText));
                    break;
                case 703:
                    StartCoroutine(EndDialogChoreography());
                    break;
                case 704:
                    StartCoroutine(MenuScoutClaimButtonChoreography());
                    break;
                case 705:
                    StartCoroutine(FirstDialogChoreography(ScoutEnergyText));
                    break;
                case 706:
                    StartCoroutine(EndDialogChoreography());
                    break;
                case 707:
                    StartCoroutine(MenuScoutQuickButtonChoreography());
                    break;
                case 708:
                    StartCoroutine(MenuScoutQuickEnergyButtonChoreography());
                    break;
                case FinishedScoutTutorialStep:
                    StartCoroutine(FirstDialogChoreography(ChangeNameInitialText));
                    break;
                case 801:
                    StartCoroutine(EndDialogChoreography());
                    break;
                case 802:
                    StartCoroutine(MenuChangeNameButtonChoreography());
                    break;
                case 803:
                    StartCoroutine(FirstDialogChoreography(ChangeNameFinalText));
                    break;
                case 804:
                    StartCoroutine(EndDialogChoreography(FinishedChangeNameTutorialStep));
                    break;
                case FinishedChangeNameTutorialStep:
                    StartCoroutine(MenuPlayButtonChoreography());
                    break;
                case FinishedFourthRealGameStep:
                    ScoutDataManager.Instance.SetInitialQuickScoutDailyClaims();
                    DungeonTicketsDataManager.Instance.EmptyGoldDungeonTickets();
                    DungeonTicketsDataManager.Instance.AddGoldDungeonTickets(1);
                    CurrencyDataManager.Instance.ForceTrophies(_thirdLeagueData.MinTrophiesInclusive);
                    CurrencyDataManager.Instance.UpdateTrophies(_thirdLeagueData.MaxTrophiesExclusive - _thirdLeagueData.MinTrophiesInclusive);
                    RewardUIParticleManager.Instance.ForceTrophiesWonAmount(_thirdLeagueData.MaxTrophiesExclusive - _thirdLeagueData.MinTrophiesInclusive);
                    RewardUIParticleManager.Instance.ForceTrophiesParticles(_thirdLeagueData.MaxTrophiesExclusive - _thirdLeagueData.MinTrophiesInclusive);
                    GameDataManager.Instance.SaveFtueStep(FinalStep);
                    break;
                case FinalStep:
                default:
                    DisableAllVisuals();
                    return;
            }
        }

        private void LeagueScreenButton_OnLeagueOneTrophiesWonChoreographyEnded()
        {
            LeagueScreenButton.OnTrophiesWonChoreographyEnded -= LeagueScreenButton_OnLeagueOneTrophiesWonChoreographyEnded;
            GameDataManager.Instance.SaveFtueStep(FinishedLeagueOneUpStep);
            _tapBlocker.SetActive(false);
            _canvas.SetActive(false);

            StartCoroutine(WaitAndMoveCurrentStep(0.5f));
        }

        private void LeagueScreenButton_OnLeagueTwoTrophiesWonChoreographyEnded()
        {
            LeagueScreenButton.OnTrophiesWonChoreographyEnded -= LeagueScreenButton_OnLeagueTwoTrophiesWonChoreographyEnded;
            GameDataManager.Instance.SaveFtueStep(FinishedLeagueTwoUpStep);
            _tapBlocker.SetActive(false);
            _canvas.SetActive(false);

            StartCoroutine(WaitAndMoveCurrentStep(0.5f));
        }

        private IEnumerator WaitAndMoveCurrentStep(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            _currentStep++;
            ShowNextStep();
        }

        private string GetTutorialStepName(int currentStep)
        {
            switch (currentStep)
            {
                // case SplashScreenStep:
                //     return "Welcome";
                // case 1:
                // case 2:
                // case 3:
                // case 4:
                // case 5:
                // case 6:
                // case 7:
                // case 8:
                // case 9:
                // case 10:
                // case 11:
                // case 12:
                // case 13:
                // case 14:
                // case 15:
                // case 16:
                // case 17:
                // case 18:
                // case 19:
                // case 20:
                // case 21:
                //     return $"ControlTutorial - {currentStep}";
                // case FinishedControlsTutorialStep:
                // case 101:
                // case 102:
                // case 103:
                // case 104:
                // case 105:
                // case 106:
                // case 107:
                // case 108:
                // case 109:
                // case 110:
                // case 111:
                // case 112:
                // case 113:
                // case 114:
                //     return $"MatchTutorial - {currentStep - FinishedControlsTutorialStep}";
                // case FinishedMatchTutorialStep:
                //     return "First Real Match";
                // case FinishedFirstRealGameStep:
                // case 202:
                // case 203:
                // case 204:
                // case 205:
                // case 206:
                // case 207:
                //     return $"Talents Tutorial - {currentStep - FinishedMatchTutorialStep}";
                // case FinishedTalentsTutorialStep:
                //     return "Second Real Match";
                // case FinishedSecondRealGameStep:
                // case 302:
                //     return $"Leagues Tutorial - {currentStep - FinishedTalentsTutorialStep}";
                // case FinishedLeagueUpStep:
                // case 401:
                // case 402:
                // case 403:
                // case 404:
                // case 405:
                // case 406:
                // case 407:
                // case 408:
                // case 409:
                // case 410:
                //     return $"Equipment Tutorial - {currentStep - FinishedLeagueUpStep}";
                // case FinishedEquipmentTutorial:
                //     return "Third Real Match";
                // case FinishedThirdRealGameStep:
                // case 502:
                // case 503:
                // case 504:
                // case 505:
                // case 506:
                // case 507:
                // case 508:
                // case 509:
                // case 510:
                // case 511:
                // case 512:
                // case 513:
                //     return $"Merge Tutorial - {currentStep - FinishedThirdRealGameStep}";
                // case FinishedMergeTutorial:
                //     return "Fourth Real Match";
                // case ChangeNameTutorial:
                // case 701:
                // case 702:
                // case 703:
                // case 704:
                //     return $"Change Name Tutorial - {currentStep - ChangeNameTutorial}";
                // case FinalStep:
                default:
                    return "Tutorial Ended";
            }
        }

        private IEnumerator KillAllEnemiesChoreography()
        {
            yield return new WaitForSeconds(ShortStepTime);
            DisableAllVisuals();
            UIManager.Instance.DisableBossIndicator();
            GameManager.Instance.ResumeGameplay();
            EnemyManager.Instance.OnEnemyDead += EnemyManager_OnEnemyDead;
            _enemyKillCounter = 0;
            EnemyManager.Instance.ForceSpawn();
            yield return new WaitForSeconds(LongStepTime);
            EnemyManager.Instance.ForceSpawn();
            yield return new WaitForSeconds(LongStepTime);
            EnemyManager.Instance.ForceSpawn();
        }

        private void EnemyManager_OnEnemyDead(Enemy enemy)
        {
            _enemyKillCounter++;

            if (_enemyKillCounter >= TargetEnemyKills)
            {
                EnemyManager.Instance.OnEnemyDead -= EnemyManager_OnEnemyDead;
                _currentStep++;
                ShowNextStep();
            }
        }

        private IEnumerator BossPowerChoreography()
        {
            PlayerManager.Instance.GetHumanPlayer().ActivateBossPower(true);
            yield return new WaitForSeconds(MatchManager.Instance.GetBossPowerInvulnerabilityDuration());
            _currentStep++;
            ShowNextStep();
        }

        private IEnumerator BossAppearChoreography()
        {
            yield return new WaitForSeconds(ShortStepTime);
            DisableAllVisuals();
            MatchManager.Instance.ForceStage2();
            GameManager.Instance.ResumeGameplay();
            UIManager.Instance.ShowBossWarningOverlay();
            yield return new WaitForSeconds(UIManager.Instance.BossWarningOverlayDuration());
            EnemyManager.Instance.SpawnBoss(true);
            EnemyManager.Instance.OnBossDead += EnemyManager_OnBossDead;
            UIManager.Instance.EnableBossIndicator();
        }

        private void EnemyManager_OnBossDead()
        {
            EnemyManager.Instance.OnBossDead -= EnemyManager_OnBossDead;
            _currentStep++;
            ShowNextStep();
        }

        private void HumanPlayer_OnLevelUp(PlayerLevel.OnLevelUpEventArgs args)
        {
            if (args.CurrentLevel < 2)
            {
                return;
            }

            PlayerManager.Instance.GetHumanPlayer().OnLevelUp -= HumanPlayer_OnLevelUp;
            GameManager.Instance.StopGameplay();
            _currentStep++;
            ShowNextStep();
        }

        private IEnumerator StartGameChoreography()
        {
            yield return new WaitForSeconds(ShortStepTime);
            DisableAllVisuals();
            PlayerManager.Instance.GetPlayer(0).MakePlayerNonKillable();
            PlayerManager.Instance.GetPlayer(1).MakePlayerNonKillable();
            UIManager.Instance.ShowStartGameAnimation();
            PlayerManager.Instance.GetHumanPlayer().OnLevelUp += HumanPlayer_OnLevelUp;
        }

        private void ShowMoveSymbolChoreography()
        {
            _tutorialTopText.text = ControlsTutorialDragToMoveText;
            _tutorialTopTextAnimator.SetTrigger(AppearTrigger);
            _tutorialMoveIn8Animator.SetTrigger(AppearTrigger);
            GameManager.Instance.GameMatchStart();
            PlayerManager.Instance.GetHumanPlayer().AttackOnlyDisable();
            PlayerManager.Instance.GetHumanPlayer().MakePlayerNonKillable();
            _controlTutorialMoveAmount = 0;
            InputManager.Instance.OnDragVector += InputManager_OnDragVector;
        }

        private void InputManager_OnDragVector(Vector2 inputVector)
        {
            if (inputVector == Vector2.zero)
            {
                return;
            }

            _controlTutorialMoveAmount += Time.deltaTime;

            if (_controlTutorialMoveAmount > MinTimeToMoveToCompleteStep3)
            {
                InputManager.Instance.OnDragVector -= InputManager_OnDragVector;
                _tutorialTopTextAnimator.SetTrigger(HideTrigger);
                _tutorialMoveIn8Animator.SetTrigger(HideTrigger);
                _currentStep++;
                ShowNextStep();
            }
        }

        private IEnumerator FirstEnemySpawnChoreography()
        {
            yield return ControlsTutorialEnemySpawner.Instance.SpawnWave1();
            _currentStep++;
            ShowNextStep();
        }

        private void ShowStopToAttackChoreography()
        {
            _tutorialTopText.text = ControlsTutorialStopToAttackText;
            _tutorialTopTextAnimator.SetTrigger(AppearTrigger);
            _tutorialMoveIn8Animator.SetTrigger(AppearTrigger);
            _tutorialStopToMoveAnimator.SetTrigger(AppearTrigger);
            _tutorialStopToMoveRedCross.fillAmount = 0;
            StartCoroutine(MakeRedCrossFill());
            PlayerManager.Instance.GetHumanPlayer().AttackOnlyEnable();
            List<Enemy> enemiesWave1 = ControlsTutorialEnemySpawner.Instance.GetEnemiesWave1();
            _controlWaveRemainingDeads = enemiesWave1.Count;

            foreach (Enemy enemy in enemiesWave1)
            {
                enemy.OnDead += Wave1Enemy_OnDead;
            }

            EnemyManager.Instance.AddEnemiesFromTutorial(enemiesWave1);
        }

        private IEnumerator MakeRedCrossFill()
        {
            float currentWhiteFillTime = 0.0f;

            while (currentWhiteFillTime < LongStepTime)
            {
                _tutorialStopToMoveRedCross.fillAmount = currentWhiteFillTime / LongStepTime;
                currentWhiteFillTime += Time.deltaTime;
                yield return null;
            }

            _tutorialStopToMoveRedCross.fillAmount = 1f;
        }

        private void Wave1Enemy_OnDead(Enemy enemy, int arg2)
        {
            enemy.OnDead -= Wave1Enemy_OnDead;
            ControlsTutorialEnemySpawner.Instance.RemoveEnemyFromWave1(enemy);

            _controlWaveRemainingDeads--;

            if (_controlWaveRemainingDeads <= 0)
            {
                PlayerManager.Instance.GetHumanPlayer().AttackOnlyDisable();
                _tutorialTopTextAnimator.SetTrigger(HideTrigger);
                _tutorialMoveIn8Animator.SetTrigger(HideTrigger);
                _tutorialStopToMoveAnimator.SetTrigger(HideTrigger);
                _currentStep++;
                ShowNextStep();
            }
        }

        private IEnumerator SetMaskOverXpItem()
        {
            XpDrop[] xpDropsOnPlanet = FindObjectsOfType<XpDrop>();

            UnityEngine.Camera mainCamera = UnityEngine.Camera.main;

            if (xpDropsOnPlanet.Length == 0 || !mainCamera)
            {
                _currentStep++;
                ShowNextStep();
                yield break;
            }

            RectTransform backgroundMaskRect = _pointerMaskSquare.transform.Find("Iris Shot").Find("Unmask").GetComponent<RectTransform>();
            Vector3 screenPoint = mainCamera.WorldToScreenPoint(xpDropsOnPlanet[0].transform.Find("Visuals").Find("Item_position").transform.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.GetComponent<RectTransform>(), screenPoint, null, out Vector2 localPoint);

            _canvas.SetActive(true);
            _pointerMaskSquare.SetActive(true);
            _pointerMaskSquareAnimator.SetTrigger(AppearTrigger);
            _backgroundAnimator.SetTrigger(HideTrigger);

            backgroundMaskRect.anchoredPosition = localPoint;
            yield return new WaitForSeconds(LongStepTime);
            _currentStep++;
            ShowNextStep();
        }

        private void CollectTagChoreography()
        {
            XpDrop[] xpDropsOnPlanet = FindObjectsOfType<XpDrop>();
            _canvas.SetActive(false);
            _pointerMaskSquare.SetActive(false);
            _pointerMaskSquareAnimator.SetTrigger(HideTrigger);
            _backgroundAnimator.ResetTrigger(HideTrigger);

            if (xpDropsOnPlanet.Length == 0)
            {
                _currentStep++;
                ShowNextStep();
                return;
            }

            _tutorialTopText.text = ControlsTutorialPointsXpDropsTextHint;
            _tutorialTopTextAnimator.SetTrigger(AppearTrigger);
            PlayerManager.Instance.GetHumanPlayer().EnableDirectionalArrow(xpDropsOnPlanet[0].transform.position);
            PlayerManager.Instance.GetHumanPlayer().MagnetColliderEnable();
            // Restoring original values
            PlayerManager.Instance.GetHumanPlayer().SetFactorOfPlayerRadiusToSee(0.8f);
            CameraManager.Instance.RecalculateMagnitudeConstantMultiplier();
            PlayerManager.Instance.GetHumanPlayer().SetAttackRange(6.0f);
            PlayerManager.Instance.GetHumanPlayer().OnXpGained += Player_OnXpGained;
        }

        private void Player_OnXpGained(PlayerLevel.OnXpGainedEventArgs arg1, int arg2)
        {
            PlayerManager.Instance.GetHumanPlayer().OnXpGained -= Player_OnXpGained;
            _tutorialTopTextAnimator.SetTrigger(HideTrigger);
            PlayerManager.Instance.GetHumanPlayer().DisableDirectionalArrow();
            _tapBlocker.SetActive(true);
            _currentStep++;
            ShowNextStep();
        }

        private IEnumerator RangeUpSkillButtonChoreography()
        {
            DisableAllVisuals();
            _canvas.SetActive(true);
            _pointerMaskSquare.SetActive(true);
            _pointerDownIndicator.SetActive(true);
            MoveMaskAndPointerToButton(_pointerMaskSquare, _pointerDownIndicator, _firstSkillTargetButton, offsetY: 20);

            yield return new WaitForSeconds(ShortStepTime);

            _pointerMaskSquareAnimator.SetTrigger(AppearTrigger);
            _pointerDownIndicatorAnimator.SetTrigger(AppearTrigger);
            _firstSkillTargetButton.OnClicked += SkillButton_OnClicked;
            _tapBlocker.SetActive(false);
        }

        private void SkillButton_OnClicked(InteractableButton obj)
        {
            _firstSkillTargetButton.OnClicked -= SkillButton_OnClicked;

            _canvas.SetActive(false);
            _pointerMaskSquare.SetActive(false);
            _pointerDownIndicator.SetActive(false);
            _pointerMaskSquareAnimator.SetTrigger(HideTrigger);
            _pointerDownIndicatorAnimator.SetTrigger(HideTrigger);

            _currentStep++;
            ShowNextStep();
        }

        private IEnumerator RemoveObstaclesChoreography()
        {
            foreach (TutorialObstacleRemover obstacleRemover in FindObjectsOfType<TutorialObstacleRemover>())
            {
                obstacleRemover.gameObject.SetActive(false);
            }

            yield return null;
        }

        private IEnumerator SecondEnemySpawnChoreography()
        {
            yield return ControlsTutorialEnemySpawner.Instance.SpawnWave2();
            List<Enemy> enemiesWave2 = ControlsTutorialEnemySpawner.Instance.GetEnemiesWave2();
            _controlWaveRemainingDeads = enemiesWave2.Count;
            Debug.Log($"Enemies wave 2: {_controlWaveRemainingDeads}");

            foreach (Enemy enemy in enemiesWave2)
            {
                enemy.OnDead += Wave2Enemy_OnDead;
            }

            EnemyManager.Instance.AddEnemiesFromTutorial(enemiesWave2);
            PlayerManager.Instance.GetHumanPlayer().AttackOnlyEnable();
        }

        private void Wave2Enemy_OnDead(Enemy enemy, int arg2)
        {
            enemy.OnDead -= Wave2Enemy_OnDead;
            ControlsTutorialEnemySpawner.Instance.RemoveEnemyFromWave2(enemy);

            _controlWaveRemainingDeads--;
            Debug.Log($"Remaining Enemies wave 2: {_controlWaveRemainingDeads}");

            if (_controlWaveRemainingDeads <= 0)
            {
                _currentStep++;
                ShowNextStep();
            }
        }

        private IEnumerator LoadMatchTutorial()
        {
            GameManager.Instance.StopGameplay();
            ScreenFader.Instance.FadeIn(true);
            MusicManager.Instance.FadeOutMusic();
            GameDataManager.Instance.SaveFtueStep(FinishedControlsTutorialStep);
            yield return new WaitForSeconds(LongStepTime);
            SceneManager.LoadScene((int)SceneEnum.Game);
        }
        
        private void WaitForTap()
        {
            _clickableArea.OnClicked += ClickableArea_OnClicked;
            _clickableArea.gameObject.SetActive(true);
            _clickableArea.Enable();
            _tapToContinueAnimator.SetTrigger(AppearTrigger);
        }

        private void ClickableArea_OnClicked(InteractableButton button)
        {
            _clickableArea.OnClicked -= ClickableArea_OnClicked;
            _currentStep++;
            ShowNextStep();
        }

        private IEnumerator FirstDialogChoreography(string text)
        {
            _canvas.SetActive(true);
            _background.SetActive(true);
            _tapToContinue.SetActive(true);
            _character.SetActive(true);

            _clickableArea.gameObject.SetActive(false);

            yield return new WaitForSeconds(MediumStepTime);

            _backgroundAnimator.SetTrigger(AppearTrigger);

            yield return new WaitForSeconds(MediumStepTime);

            _characterAnimator.SetTrigger(AppearTrigger);

            yield return new WaitForSeconds(MediumStepTime);

            _characterAnimator.SetTrigger(TalkTrigger);

            yield return new WaitForSeconds(ShortStepTime);

            _bubbleTextAnimator.SetTrigger(AppearTrigger);

            yield return new WaitForSeconds(ShortStepTime);

            _bubbleText.text = text;
            _textAnimator.SetTrigger(AppearTrigger);

            yield return new WaitForSeconds(ShortStepTime);

            WaitForTap();
        }

        private IEnumerator NextDialogChoreography(string text)
        {
            _clickableArea.gameObject.SetActive(false);

            _tapToContinueAnimator.SetTrigger(HideTrigger);
            _textAnimator.SetTrigger(HideTrigger);
            _bubbleTextAnimator.SetTrigger(HideTrigger);
            _characterAnimator.SetTrigger(TalkTrigger);

            yield return new WaitForSeconds(ShortStepTime);

            _bubbleText.text = text;
            _bubbleTextAnimator.SetTrigger(AppearTrigger);

            yield return new WaitForSeconds(ShortStepTime);

            _textAnimator.SetTrigger(AppearTrigger);

            yield return new WaitForSeconds(ShortStepTime);

            WaitForTap();
        }

        private IEnumerator EndDialogChoreography(int stepToComplete = -1)
        {
            _clickableArea.gameObject.SetActive(false);

            _tapToContinueAnimator.SetTrigger(HideTrigger);
            _textAnimator.SetTrigger(HideTrigger);
            _bubbleTextAnimator.SetTrigger(HideTrigger);

            yield return new WaitForSeconds(ShortStepTime);

            _characterAnimator.SetTrigger(HideTrigger);

            yield return new WaitForSeconds(ShortStepTime);

            _backgroundAnimator.SetTrigger(HideTrigger);

            yield return new WaitForSeconds(ShortStepTime);

            if (stepToComplete != -1)
            {
                GameDataManager.Instance.SaveFtueStep(stepToComplete);
                _currentStep = stepToComplete;
            }
            else
            {
                _currentStep++;
            }

            ShowNextStep();
        }

        private IEnumerator TutorialRewardsChoreography(int step)
        {
            _clickableArea.gameObject.SetActive(false);

            _tapToContinueAnimator.SetTrigger(HideTrigger);
            _textAnimator.SetTrigger(HideTrigger);
            _bubbleTextAnimator.SetTrigger(HideTrigger);

            yield return new WaitForSeconds(ShortStepTime);

            _characterAnimator.SetTrigger(HideTrigger);

            yield return new WaitForSeconds(ShortStepTime);

            _backgroundAnimator.SetTrigger(HideTrigger);

            yield return new WaitForSeconds(ShortStepTime);

            UIManager.Instance.ShowMatchResultsScreen(false, true);
            _canvas.gameObject.SetActive(false);

            GameDataManager.Instance.SaveFtueStep(step);
        }

        private void DisableAllVisuals()
        {
            _background.SetActive(false);
            _clickableArea.gameObject.SetActive(false);
            _tapToContinue.SetActive(false);
            _character.SetActive(false);

            _pointerMaskBig.SetActive(false);
            _pointerMaskWide.SetActive(false);
            _pointerMaskSquare.SetActive(false);
            _pointerMaskMid.SetActive(false);
            _pointerMaskBigHorizontal.SetActive(false);

            _pointerDownIndicator.SetActive(false);
            _pointerUpIndicator.SetActive(false);
        }

        private IEnumerator MenuPlayButtonChoreography()
        {
            DisableAllVisuals();
            _canvas.SetActive(true);

            _pointerMaskBig.SetActive(true);
            _pointerDownIndicator.SetActive(true);

            yield return new WaitForSeconds(ShortStepTime);

            MoveMaskAndPointerToButton(_pointerMaskBig, _pointerDownIndicator, _playButton);

            _pointerMaskBigAnimator.SetTrigger(AppearTrigger);
            _pointerDownIndicatorAnimator.SetTrigger(AppearTrigger);
            _playButton.OnClicked += PlayButton_OnClicked;
        }

        private void PlayButton_OnClicked(InteractableButton button)
        {
            _playButton.OnClicked -= PlayButton_OnClicked;
            _pointerMaskBigAnimator.SetTrigger(HideTrigger);
            _pointerDownIndicatorAnimator.SetTrigger(HideTrigger);

            if (GameDataManager.Instance.GetFtueStep() < FinishedFirstRealGameStep)
            {
                GameDataManager.Instance.SaveFtueStep(FinishedFirstRealGameStep);
            }
            else if (GameDataManager.Instance.GetFtueStep() < FinishedSecondRealGameStep)
            {
                GameDataManager.Instance.SaveFtueStep(FinishedSecondRealGameStep);
            }
            else if (GameDataManager.Instance.GetFtueStep() < FinishedThirdRealGameStep)
            {
                GameDataManager.Instance.SaveFtueStep(FinishedThirdRealGameStep);
            }
            else if (GameDataManager.Instance.GetFtueStep() < FinishedFourthRealGameStep)
            {
                GameDataManager.Instance.SaveFtueStep(FinishedFourthRealGameStep);
            }
        }

        private void MoveMaskAndPointerToButton(GameObject mask, GameObject pointer, InteractableButton button, float offsetX = 0, float offsetY = 0, bool hasToScaleMask = false)
        {
            RectTransform buttonRect = button.GetComponent<RectTransform>();
            MoveMaskAndPointerToRectTransform(mask, pointer, buttonRect, offsetX, offsetY, hasToScaleMask);
        }

        private void MoveMaskAndPointerToRectTransform(GameObject mask, GameObject pointer, RectTransform target, float offsetX = 0, float offsetY = 0, bool hasToScaleMask = false)
        {
            Vector3 worldPosition = target.position;

            RectTransform backgroundMaskRect = mask.transform.Find("Iris Shot").Find("Unmask").GetComponent<RectTransform>();
            Vector3 localPosition = backgroundMaskRect.parent.InverseTransformPoint(worldPosition);

            backgroundMaskRect.localPosition = localPosition + new Vector3(hasToScaleMask ? offsetX * _equipmentScreen.GetGridScaleFactor() : offsetX, offsetY);

            RectTransform pointerRect = pointer.GetComponent<RectTransform>();
            localPosition = pointerRect.parent.InverseTransformPoint(worldPosition);

            pointerRect.localPosition = localPosition;

            if (hasToScaleMask)
            {
                backgroundMaskRect.sizeDelta *= _equipmentScreen.GetGridScaleFactor();
            }
        }

        private void MoveMaskToTransform(GameObject mask, Transform target, float offsetX = 0, float offsetY = 0, bool hasToScaleMask = false)
        {
            RectTransform targetRect = target.GetComponent<RectTransform>();
            Vector3 worldPosition = targetRect.position;

            RectTransform backgroundMaskRect = mask.transform.Find("Iris Shot").Find("Unmask").GetComponent<RectTransform>();
            Vector3 localPosition = backgroundMaskRect.parent.InverseTransformPoint(worldPosition);

            backgroundMaskRect.localPosition = localPosition + new Vector3(offsetX, offsetY);
        }

        private IEnumerator MenuNavigationBarTalentsChoreography()
        {
            DisableAllVisuals();
            _canvas.SetActive(true);

            _pointerMaskSquare.SetActive(true);
            _pointerDownIndicator.SetActive(true);

            yield return new WaitForSeconds(ShortStepTime);

            MoveMaskAndPointerToButton(_pointerMaskSquare, _pointerDownIndicator, _talentsButton);

            _pointerMaskSquareAnimator.SetTrigger(AppearTrigger);
            _pointerDownIndicatorAnimator.SetTrigger(AppearTrigger);

            _talentsButton.OnClicked += TalentsButton_OnClicked;
        }

        private void TalentsButton_OnClicked(InteractableButton button)
        {
            _talentsButton.OnClicked -= TalentsButton_OnClicked;
            _pointerMaskSquareAnimator.SetTrigger(HideTrigger);
            _pointerDownIndicatorAnimator.SetTrigger(HideTrigger);

            _currentStep++;
            ShowNextStep();
        }

        private IEnumerator MenuUpgradeTalentChoreography()
        {
            DisableAllVisuals();
            _canvas.SetActive(true);

            _pointerMaskBig.SetActive(true);
            _pointerDownIndicator.SetActive(true);

            _talentsScreen.OnTalentUpgradeStarted += TalentsScreen_OnTalentUpgradeStarted;
            _talentsScreen.OnTalentUpgradeAnimationEnded += TalentsScreen_OnTalentUpgradeAnimationEnded;
            _talentsScreen.OnTalentUpgradeEnded += TalentsScreen_OnTalentUpgradeEnded;

            yield return new WaitForSeconds(ShortStepTime);

            MoveMaskAndPointerToButton(_pointerMaskBig, _pointerDownIndicator, _upgradeTalentsButton);

            _pointerMaskBigAnimator.SetTrigger(AppearTrigger);
            _pointerDownIndicatorAnimator.SetTrigger(AppearTrigger);
        }

        private void TalentsScreen_OnTalentUpgradeStarted()
        {
            _talentsScreen.OnTalentUpgradeStarted -= TalentsScreen_OnTalentUpgradeStarted;

            _pointerMaskBigAnimator.SetTrigger(HideTrigger);
            _pointerDownIndicatorAnimator.SetTrigger(HideTrigger);

            StartCoroutine(WaitAndDisablePointerMask(_pointerMaskBig));
        }

        private void TalentsScreen_OnTalentUpgradeAnimationEnded()
        {
            _talentsScreen.OnTalentUpgradeAnimationEnded -= TalentsScreen_OnTalentUpgradeAnimationEnded;

            DisableAllVisuals();
        }

        private void TalentsScreen_OnTalentUpgradeEnded()
        {
            _talentsScreen.OnTalentUpgradeEnded -= TalentsScreen_OnTalentUpgradeEnded;

            _currentStep++;
            ShowNextStep();
        }

        private IEnumerator MenuNavigationBarBattleChoreography(int stepToComplete = -1)
        {
            DisableAllVisuals();
            _canvas.SetActive(true);

            _pointerMaskSquare.SetActive(true);
            _pointerDownIndicator.SetActive(true);

            yield return new WaitForSeconds(ShortStepTime);

            MoveMaskAndPointerToButton(_pointerMaskSquare, _pointerDownIndicator, _battleButton);

            _pointerMaskSquareAnimator.SetTrigger(AppearTrigger);
            _pointerDownIndicatorAnimator.SetTrigger(AppearTrigger);
            _battleButton.OnClicked += BattleButton_OnClicked;
            _stepToComplete = stepToComplete;
        }

        private void BattleButton_OnClicked(InteractableButton button)
        {
            _battleButton.OnClicked -= BattleButton_OnClicked;
            _pointerMaskSquareAnimator.SetTrigger(HideTrigger);
            _pointerDownIndicatorAnimator.SetTrigger(HideTrigger);

            if (_stepToComplete != -1)
            {
                GameDataManager.Instance.SaveFtueStep(_stepToComplete);
                _currentStep = _stepToComplete;
                ShowNextStep();
            }
            else if (_currentStep < FinalStep)
            {
                _currentStep++;
                ShowNextStep();
            }
            else
            {
                DisableAllVisuals();
            }
        }

        private void MenuLeagueOneClosePopupButtonChoreography()
        {
            StartCoroutine(MenuLeagueClosePopupButtonChoreography());
            _leagueUpBackgroundButton.OnClicked += LeagueOneUpClose_OnClicked;
        }
        
        private void LeagueOneUpClose_OnClicked(InteractableButton button)
        {
            _leagueUpBackgroundButton.OnClicked -= LeagueOneUpClose_OnClicked;
            _pointerMaskSquareAnimator.SetTrigger(HideTrigger);
            _pointerUpIndicatorAnimator.SetTrigger(HideTrigger);

            GameDataManager.Instance.SaveFtueStep(FinishedLeagueOneUpStep);
            _currentStep = FinishedLeagueOneUpStep;
            ShowNextStep();
        }

        private void MenuLeagueTwoClosePopupButtonChoreography()
        {
            StartCoroutine(MenuLeagueClosePopupButtonChoreography());
            _leagueUpBackgroundButton.OnClicked += LeagueTwoUpClose_OnClicked;
        }

        private void LeagueTwoUpClose_OnClicked(InteractableButton button)
        {
            _leagueUpBackgroundButton.OnClicked -= LeagueTwoUpClose_OnClicked;
            _pointerMaskSquareAnimator.SetTrigger(HideTrigger);
            _pointerUpIndicatorAnimator.SetTrigger(HideTrigger);

            GameDataManager.Instance.SaveFtueStep(FinishedLeagueTwoUpStep);
            _currentStep = FinishedLeagueTwoUpStep;
            ShowNextStep();
        }

        private IEnumerator MenuLeagueClosePopupButtonChoreography()
        {
            DisableAllVisuals();
            _canvas.SetActive(true);
            _pointerMaskSquare.SetActive(true);
            _pointerUpIndicator.SetActive(true);

            yield return new WaitForSeconds(MediumStepTime);
            yield return new WaitForSeconds(2.0f);

            MoveMaskAndPointerToButton(_pointerMaskSquare, _pointerUpIndicator, _leagueUpCloseButton);

            _pointerMaskSquareAnimator.SetTrigger(AppearTrigger);
            _pointerUpIndicatorAnimator.SetTrigger(AppearTrigger);
        }

        private IEnumerator MenuNavigationBarEquipmentChoreography()
        {
            DisableAllVisuals();
            _canvas.SetActive(true);

            _pointerMaskSquare.SetActive(true);
            _pointerDownIndicator.SetActive(true);

            yield return new WaitForSeconds(ShortStepTime);

            MoveMaskAndPointerToButton(_pointerMaskSquare, _pointerDownIndicator, _equipmentButton);

            _pointerMaskSquareAnimator.SetTrigger(AppearTrigger);
            _pointerDownIndicatorAnimator.SetTrigger(AppearTrigger);
            _equipmentButton.OnClicked += EquipmentButton_OnClicked;
        }

        private void EquipmentButton_OnClicked(InteractableButton button)
        {
            _equipmentButton.OnClicked -= EquipmentButton_OnClicked;
            _pointerMaskSquareAnimator.SetTrigger(HideTrigger);
            _pointerDownIndicatorAnimator.SetTrigger(HideTrigger);

            _currentStep++;
            ShowNextStep();
        }

        private IEnumerator MenuEquipmentPieceButtonChoreography()
        {
            DisableAllVisuals();
            _canvas.SetActive(true);

            _pointerMaskSquare.SetActive(true);
            _pointerDownIndicator.SetActive(true);

            yield return new WaitForSeconds(ShortStepTime);

            MoveMaskAndPointerToButton(_pointerMaskSquare, _pointerDownIndicator, _equipmentScreen.FirstEquipmentPiece(), hasToScaleMask: true);

            _pointerMaskSquareAnimator.SetTrigger(AppearTrigger);
            _pointerDownIndicatorAnimator.SetTrigger(AppearTrigger);
            _equipmentScreen.OnEquipmentPieceButtonClicked += EquipmentScreen_OnEquipmentPieceButtonClicked;
        }

        private void EquipmentScreen_OnEquipmentPieceButtonClicked()
        {
            _equipmentScreen.OnEquipmentPieceButtonClicked -= EquipmentScreen_OnEquipmentPieceButtonClicked;
            _pointerMaskSquareAnimator.SetTrigger(HideTrigger);
            _pointerDownIndicatorAnimator.SetTrigger(HideTrigger);

            _currentStep++;
            ShowNextStep();
        }

        private IEnumerator MenuEquipmentPieceEquipButtonChoreography()
        {
            DisableAllVisuals();
            _canvas.SetActive(true);
            _pointerMaskMid.SetActive(true);
            _pointerDownIndicator.SetActive(true);

            yield return new WaitForSeconds(ShortStepTime);

            MoveMaskAndPointerToButton(_pointerMaskMid, _pointerDownIndicator, _equipmentPieceEquipButton);

            _pointerMaskMidAnimator.SetTrigger(AppearTrigger);
            _pointerDownIndicatorAnimator.SetTrigger(AppearTrigger);
            _equipmentPieceEquipButton.OnClicked += EquipmentPartEquipButton_OnClicked;
        }

        private void EquipmentPartEquipButton_OnClicked(InteractableButton button)
        {
            _equipmentPieceEquipButton.OnClicked -= EquipmentPartEquipButton_OnClicked;
            _pointerMaskMidAnimator.SetTrigger(HideTrigger);
            _pointerDownIndicatorAnimator.SetTrigger(HideTrigger);

            _currentStep++;
            ShowNextStep();
        }

        private IEnumerator MenuEquipmentSlotButtonChoreography()
        {
            DisableAllVisuals();
            _canvas.SetActive(true);

            _pointerMaskSquare.SetActive(true);
            _pointerDownIndicator.SetActive(true);

            yield return new WaitForSeconds(ShortStepTime);

            MoveMaskAndPointerToButton(_pointerMaskSquare, _pointerDownIndicator, _weaponSlotButton, hasToScaleMask: true);

            _pointerMaskSquareAnimator.SetTrigger(AppearTrigger);
            _pointerDownIndicatorAnimator.SetTrigger(AppearTrigger);
            _weaponSlotButton.OnClicked += WeaponSlot_OnClicked;
        }

        private void WeaponSlot_OnClicked(InteractableButton button)
        {
            _weaponSlotButton.OnClicked -= WeaponSlot_OnClicked;
            _pointerMaskSquareAnimator.SetTrigger(HideTrigger);
            _pointerDownIndicatorAnimator.SetTrigger(HideTrigger);

            _currentStep++;
            ShowNextStep();
        }

        private IEnumerator MenuEquipmentSlotUpgradeButtonChoreography()
        {
            DisableAllVisuals();
            _canvas.SetActive(true);
            _pointerMaskMid.SetActive(true);
            _pointerUpIndicator.SetActive(true);

            yield return new WaitForSeconds(ShortStepTime);

            MoveMaskAndPointerToButton(_pointerMaskMid, _pointerUpIndicator, _equipmentPieceUpgradeButton, offsetY: UpgradePieceYOffset);

            _pointerMaskMidAnimator.SetTrigger(AppearTrigger);
            _pointerUpIndicatorAnimator.SetTrigger(AppearTrigger);
            _equipmentPieceUpgradeButton.OnClicked += EquipmentPartUpgradeButton_OnClicked;
        }

        private void EquipmentPartUpgradeButton_OnClicked(InteractableButton button)
        {
            _equipmentPieceUpgradeButton.OnClicked -= EquipmentPartUpgradeButton_OnClicked;
            _pointerMaskMidAnimator.SetTrigger(HideTrigger);
            _pointerUpIndicatorAnimator.SetTrigger(HideTrigger);

            _currentStep++;
            ShowNextStep();
        }

        private IEnumerator MenuEquipmentDetailPopupCloseChoreography()
        {
            yield return new WaitForSeconds(1.0f);
            _equipmentDetailPopup.Hide();

            _currentStep++;
            ShowNextStep();
        }

        private IEnumerator MenuEquipmentMergeButtonChoreography()
        {
            DisableAllVisuals();
            _canvas.SetActive(true);
            _pointerMaskMid.SetActive(true);
            _pointerUpIndicator.SetActive(true);

            yield return new WaitForSeconds(ShortStepTime);

            MoveMaskAndPointerToButton(_pointerMaskMid, _pointerUpIndicator, _equipmentMergeButton);

            _pointerMaskMidAnimator.SetTrigger(AppearTrigger);
            _pointerUpIndicatorAnimator.SetTrigger(AppearTrigger);
            _equipmentMergeButton.OnClicked += EquipmentMergeButton_OnClicked;
        }

        private void EquipmentMergeButton_OnClicked(InteractableButton button)
        {
            _equipmentMergeButton.OnClicked -= EquipmentMergeButton_OnClicked;
            _pointerMaskMidAnimator.SetTrigger(HideTrigger);
            _pointerUpIndicatorAnimator.SetTrigger(HideTrigger);

            _currentStep++;
            ShowNextStep();
        }

        private IEnumerator MenuEquipmentPartButtonMergeChoreography(InteractableButton button, float offsetX = 0)
        {
            DisableAllVisuals();
            _canvas.SetActive(true);
            _pointerMaskSquare.SetActive(true);
            _pointerDownIndicator.SetActive(true);

            yield return new WaitForSeconds(ShortStepTime);

            MoveMaskAndPointerToButton(_pointerMaskSquare, _pointerDownIndicator, button, offsetX, hasToScaleMask: true);

            _pointerMaskSquareAnimator.SetTrigger(AppearTrigger);
            _pointerDownIndicatorAnimator.SetTrigger(AppearTrigger);
            _equipmentMergeScreen.OnEquipmentPieceButtonClicked += EquipmentMergeScreen_OnEquipmentPieceButtonClicked;
        }

        private void EquipmentMergeScreen_OnEquipmentPieceButtonClicked()
        {
            _equipmentMergeScreen.OnEquipmentPieceButtonClicked -= EquipmentMergeScreen_OnEquipmentPieceButtonClicked;
            _pointerMaskSquareAnimator.SetTrigger(HideTrigger);
            _pointerDownIndicatorAnimator.SetTrigger(HideTrigger);

            _currentStep++;
            ShowNextStep();
        }

        private IEnumerator MenuEquipmentConfirmMergeChoreography()
        {
            DisableAllVisuals();
            _canvas.SetActive(true);
            _pointerMaskMid.SetActive(true);
            _pointerDownIndicator.SetActive(true);

            yield return new WaitForSeconds(ShortStepTime);

            MoveMaskAndPointerToButton(_pointerMaskMid, _pointerDownIndicator, _equipmentConfirmMergeButton);

            _pointerMaskMidAnimator.SetTrigger(AppearTrigger);
            _pointerDownIndicatorAnimator.SetTrigger(AppearTrigger);
            _equipmentConfirmMergeButton.OnClicked += EquipmentConfirmMergeButton_OnClicked;
            _mergeAnimationScreen.OnClosed += MergeAnimationScreen_OnClosed;
        }

        private void EquipmentConfirmMergeButton_OnClicked(InteractableButton button)
        {
            _equipmentConfirmMergeButton.OnClicked -= EquipmentConfirmMergeButton_OnClicked;
            _pointerMaskMidAnimator.SetTrigger(HideTrigger);
            _pointerDownIndicatorAnimator.SetTrigger(HideTrigger);

            GameDataManager.Instance.SaveFtueStep(FinishedMergeTutorialStep);

            StartCoroutine(WaitAndDisablePointerMask(_pointerMaskMid));
        }

        private IEnumerator WaitAndDisablePointerMask(GameObject pointerMask)
        {
            yield return new WaitForSeconds(ShortStepTime);
            pointerMask.SetActive(false);
        }

        private void MergeAnimationScreen_OnClosed()
        {
            _mergeAnimationScreen.OnClosed -= MergeAnimationScreen_OnClosed;

            _currentStep++;
            ShowNextStep();
        }

        private IEnumerator MenuEquipmentMergeBackButtonChoreography()
        {
            DisableAllVisuals();
            _canvas.SetActive(true);
            _pointerMaskSquare.SetActive(true);
            _pointerDownIndicator.SetActive(true);

            yield return new WaitForSeconds(ShortStepTime);

            MoveMaskAndPointerToButton(_pointerMaskSquare, _pointerDownIndicator, _equipmentMergeBackButton);

            _pointerMaskSquareAnimator.SetTrigger(AppearTrigger);
            _pointerDownIndicatorAnimator.SetTrigger(AppearTrigger);
            _equipmentMergeBackButton.OnClicked += EquipmentMergeBackButton_OnClicked;
        }

        private void EquipmentMergeBackButton_OnClicked(InteractableButton button)
        {
            _equipmentMergeBackButton.OnClicked -= EquipmentMergeBackButton_OnClicked;
            _pointerMaskSquareAnimator.SetTrigger(HideTrigger);
            _pointerDownIndicatorAnimator.SetTrigger(HideTrigger);

            _currentStep++;
            ShowNextStep();
        }

        private IEnumerator GameRankNumberChoreography()
        {
            _background.SetActive(false);
            _pointerMaskMid.SetActive(true);

            yield return new WaitForSeconds(ShortStepTime);

            MoveMaskToTransform(_pointerMaskMid, _rankingTarget.transform);

            _pointerMaskMidAnimator.SetTrigger(AppearTrigger);

            WaitForTapOnTransformMask();
        }

        private void WaitForTapOnTransformMask()
        {
            _clickableArea.OnClicked += MaskClickableArea_OnClicked;
        }

        private void MaskClickableArea_OnClicked(InteractableButton button)
        {
            _clickableArea.OnClicked -= MaskClickableArea_OnClicked;
            _background.SetActive(true);
            _backgroundAnimator.SetTrigger(AppearTrigger);
            _pointerMaskMidAnimator.SetTrigger(HideTrigger);
        }

        private IEnumerator MenuChangeNameButtonChoreography()
        {
            DisableAllVisuals();
            _canvas.SetActive(true);

            _pointerMaskWide.SetActive(true);
            _pointerUpIndicator.SetActive(true);

            yield return new WaitForSeconds(ShortStepTime);

            MoveMaskAndPointerToButton(_pointerMaskWide, _pointerUpIndicator, _changeNameButton);

            _pointerMaskWideAnimator.SetTrigger(AppearTrigger);
            _pointerUpIndicatorAnimator.SetTrigger(AppearTrigger);
            _changeNameButton.OnClicked += ChangeNameButton_OnClicked;
            _changeNamePopup.OnClosed += ChangeNamePopup_OnClosed;
        }

        private void ChangeNameButton_OnClicked(InteractableButton button)
        {
            _changeNameButton.OnClicked -= ChangeNameButton_OnClicked;
            _canvas.SetActive(false);
            _pointerMaskWide.SetActive(false);
            _pointerUpIndicator.SetActive(false);
            _pointerMaskWideAnimator.SetTrigger(HideTrigger);
            _pointerUpIndicatorAnimator.SetTrigger(HideTrigger);
        }

        private void ChangeNamePopup_OnClosed()
        {
            _changeNamePopup.OnClosed -= ChangeNamePopup_OnClosed;
            _currentStep++;
            ShowNextStep();
        }
        
        private IEnumerator MenuNavigationBarDungeonChoreography(int stepToComplete = -1)
        {
            DisableAllVisuals();
            _canvas.SetActive(true);

            _pointerMaskSquare.SetActive(true);
            _pointerDownIndicator.SetActive(true);

            yield return new WaitForSeconds(ShortStepTime);

            MoveMaskAndPointerToButton(_pointerMaskSquare, _pointerDownIndicator, _dungeonButton);

            _pointerMaskSquareAnimator.SetTrigger(AppearTrigger);
            _pointerDownIndicatorAnimator.SetTrigger(AppearTrigger);
            _dungeonButton.OnClicked += ExpeditionsButton_OnClicked;
            _stepToComplete = stepToComplete;
        }
        
        private void ExpeditionsButton_OnClicked(InteractableButton button)
        {
            _dungeonButton.OnClicked -= ExpeditionsButton_OnClicked;
            _pointerMaskSquareAnimator.SetTrigger(HideTrigger);
            _pointerDownIndicatorAnimator.SetTrigger(HideTrigger);

            if (_stepToComplete != -1)
            {
                GameDataManager.Instance.SaveFtueStep(_stepToComplete);
                _currentStep = _stepToComplete;
                ShowNextStep();
            }
            else if (_currentStep < FinalStep)
            {
                _currentStep++;
                ShowNextStep();
            }
            else
            {
                DisableAllVisuals();
            }
        }
        
        private IEnumerator MenuGoldDungeonButtonChoreography()
        {
            DisableAllVisuals();
            _canvas.SetActive(true);
            _pointerMaskBigHorizontal.SetActive(true);
            _pointerDownIndicator.SetActive(true);

            yield return new WaitForSeconds(ShortStepTime);

            MoveMaskAndPointerToButton(_pointerMaskBigHorizontal, _pointerDownIndicator, _goldDungeonButton);

            _pointerMaskBigHorizontalAnimator.SetTrigger(AppearTrigger);
            _pointerDownIndicatorAnimator.SetTrigger(AppearTrigger);
            _goldDungeonButton.OnClicked += GoldDungeonButton_OnClicked;
        }

        private void GoldDungeonButton_OnClicked(InteractableButton button)
        {
            _goldDungeonButton.OnClicked -= GoldDungeonButton_OnClicked;
            _pointerMaskBigHorizontalAnimator.SetTrigger(HideTrigger);
            _pointerDownIndicatorAnimator.SetTrigger(HideTrigger);

            _currentStep++;
            ShowNextStep();
        }

        private IEnumerator MenuStartGoldDungeonButtonChoreography()
        {
            DisableAllVisuals();
            _canvas.SetActive(true);
            _pointerMaskBig.SetActive(true);
            _pointerDownIndicator.SetActive(true);

            yield return new WaitForSeconds(ShortStepTime);

            MoveMaskAndPointerToButton(_pointerMaskBig, _pointerDownIndicator, _startDungeonButton);

            _pointerMaskBigAnimator.SetTrigger(AppearTrigger);
            _pointerDownIndicatorAnimator.SetTrigger(AppearTrigger);
            _startDungeonButton.OnClicked += StartGoldDungeonButton_OnClicked;
        }

        private void StartGoldDungeonButton_OnClicked(InteractableButton button)
        {
            _startDungeonButton.OnClicked -= StartGoldDungeonButton_OnClicked;
            _pointerMaskBigAnimator.SetTrigger(HideTrigger);
            _pointerDownIndicatorAnimator.SetTrigger(HideTrigger);
            GameDataManager.Instance.SaveFtueStep(FinishedGoldDungeonStep);
        }
        
        private IEnumerator MenuScoutButtonChoreography()
        {
            DisableAllVisuals();
            _canvas.SetActive(true);
            _pointerMaskSquare.SetActive(true);
            _pointerDownIndicator.SetActive(true);

            yield return new WaitForSeconds(ShortStepTime);

            MoveMaskAndPointerToButton(_pointerMaskSquare, _pointerDownIndicator, _scoutButton, offsetY: ScoutButtonYOffset);

            _pointerMaskSquareAnimator.SetTrigger(AppearTrigger);
            _pointerDownIndicatorAnimator.SetTrigger(AppearTrigger);
            _scoutButton.OnClicked += ScoutButton_OnClicked;
        }

        private void ScoutButton_OnClicked(InteractableButton button)
        {
            _scoutButton.OnClicked -= ScoutButton_OnClicked;
            _pointerMaskSquareAnimator.SetTrigger(HideTrigger);
            _pointerDownIndicatorAnimator.SetTrigger(HideTrigger);

            _currentStep++;
            ShowNextStep();
        }
        
        private IEnumerator MenuScoutClaimButtonChoreography()
        {
            DisableAllVisuals();
            _canvas.SetActive(true);
            _pointerMaskMid.SetActive(true);
            _pointerDownIndicator.SetActive(true);

            yield return new WaitForSeconds(ShortStepTime);

            MoveMaskAndPointerToButton(_pointerMaskMid, _pointerDownIndicator, _scoutClaimButton);

            _pointerMaskMidAnimator.SetTrigger(AppearTrigger);
            _pointerDownIndicatorAnimator.SetTrigger(AppearTrigger);
            _scoutClaimButton.OnClicked += ScoutClaimButton_OnClicked;
        }

        private void ScoutClaimButton_OnClicked(InteractableButton button)
        {
            DisableAllVisuals();
            _scoutClaimButton.OnClicked -= ScoutClaimButton_OnClicked;
            _pointerMaskMidAnimator.SetTrigger(HideTrigger);
            _pointerDownIndicatorAnimator.SetTrigger(HideTrigger);
            _scoutRewardsPanel.OnClosed += ScoutRewardsPanel_OnClosed;
        }

        private void ScoutRewardsPanel_OnClosed()
        {
            _scoutRewardsPanel.OnClosed -= ScoutRewardsPanel_OnClosed;
            
            _currentStep++;
            ShowNextStep();
        }

        private IEnumerator MenuScoutQuickButtonChoreography()
        {
            DisableAllVisuals();
            _canvas.SetActive(true);
            _pointerMaskMid.SetActive(true);
            _pointerDownIndicator.SetActive(true);

            yield return new WaitForSeconds(ShortStepTime);

            MoveMaskAndPointerToButton(_pointerMaskMid, _pointerDownIndicator, _scoutQuickButton);

            _pointerMaskMidAnimator.SetTrigger(AppearTrigger);
            _pointerDownIndicatorAnimator.SetTrigger(AppearTrigger);
            _scoutQuickButton.OnClicked += ScoutQuickButton_OnClicked;
        }

        private void ScoutQuickButton_OnClicked(InteractableButton button)
        {
            _scoutQuickButton.OnClicked -= ScoutQuickButton_OnClicked;
            _pointerMaskMidAnimator.SetTrigger(HideTrigger);
            _pointerDownIndicatorAnimator.SetTrigger(HideTrigger);

            _currentStep++;
            ShowNextStep();
        }
        
        private IEnumerator MenuScoutQuickEnergyButtonChoreography()
        {
            DisableAllVisuals();
            _canvas.SetActive(true);
            _pointerMaskBig.SetActive(true);
            _pointerDownIndicator.SetActive(true);

            yield return new WaitForSeconds(ShortStepTime);

            MoveMaskAndPointerToButton(_pointerMaskBig, _pointerDownIndicator, _scoutQuickEnergyButton);

            _pointerMaskBigAnimator.SetTrigger(AppearTrigger);
            _pointerDownIndicatorAnimator.SetTrigger(AppearTrigger);
            _scoutQuickEnergyButton.OnClicked += ScoutQuickEnergyButton_OnClicked;
        }

        private void ScoutQuickEnergyButton_OnClicked(InteractableButton button)
        {
            DisableAllVisuals();
            _scoutQuickEnergyButton.OnClicked -= ScoutQuickEnergyButton_OnClicked;
            _pointerMaskBigAnimator.SetTrigger(HideTrigger);
            _pointerDownIndicatorAnimator.SetTrigger(HideTrigger);
            _scoutRewardsPanel.OnClosed += QuickScoutRewardsPanel_OnClosed;
            
            GameDataManager.Instance.SaveFtueStep(FinishedScoutTutorialStep);
        }

        private void QuickScoutRewardsPanel_OnClosed()
        {
            _scoutRewardsPanel.OnClosed -= QuickScoutRewardsPanel_OnClosed;
            _quickScoutPanel.Hide();
            _scoutPanel.Hide();
            
            _currentStep = FinishedScoutTutorialStep;
            ShowNextStep();
        }
    }
}