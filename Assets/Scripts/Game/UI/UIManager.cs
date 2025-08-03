using System;
using System.Collections;
using System.Collections.Generic;
using Application.Data.Equipment;
using Game.Equipment;
using Game.Match;
using Game.PlayerUnit;
using Game.Ranking;
using Game.Skills;
using UI.Button;
using UI.Indicator;
using UI.MainMenu;
using UnityEngine;

namespace Game.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private RewardsPanel _rewardsPanel;
        [SerializeField] private GoldDungeonRewardsPanel _goldRewardsPanel;
        [SerializeField] private InteractableButton _optionsButton;
        [SerializeField] private Popup _optionsPopup;
        [SerializeField] private Popup _leaveConfirmationPopup;
        [SerializeField] private InteractableButton _leaveButton;
        [SerializeField] private InteractableButton _leaveConfirmationButton;
        [Range(0,1)]
        [SerializeField] private float _leaveConfirmationPopupDelay;
        [SerializeField] private XpBar _xpBar;
        [SerializeField] private GameGoldCounter _gameGoldCounter;
        [SerializeField] private CountDownPanel _countDownPanel;
        [SerializeField] private SkillSelectionPanel _skillSelectionPanel;
        [SerializeField] private HeroStatusPanel _playerKilledStatusPanel;
        [SerializeField] private HeroKilledBossStatusPanel _heroKilledBossStatusPanel;
        [SerializeField] private float _humanPlayerKilledStatusPanelDuration;
        [SerializeField] private float _npcPlayerKilledStatusPanelDuration;
        [SerializeField] private GameObject _timeIsUpOverlay;
        [SerializeField] private GameObject _defeatedOverlay;
        [SerializeField] private GameObject _survivorOverlay;
        [SerializeField] private BossAppearWarning _bossWarningOverlay;
        [SerializeField] private float _bossWarningOverlayDuration;
        [SerializeField] private AnimationClip _overlayAnimationClip;
        [SerializeField] private GameObject _startGamePanel;
        [SerializeField] private AnimationClip _startGamePanelAnimationClip;
        [SerializeField] private RankNumberPanel _rankNumberPanel;
        [SerializeField] private RankingPanel _rankingPanel;
        [SerializeField] private BossIndicator _miniBossIndicator;
        [SerializeField] private BossIndicator _bossIndicator;
        [SerializeField] private HeroPrefabManager _humanHeroPrefabManager;
        [SerializeField] private GameObject _heroCamera;

        private const float RegularAnimationSpeed = 1.0f;
        private const float PausedAnimationSpeed = 100000.0f;

        // public Action OnOptionsPopupOpened;
        // public Action OnOptionsPopupClosed;
        public Action OnLeaveButtonClicked;
        public Action<PlayerSkillData> OnLevelUpSkillSelected;
        public Action OnStartGameAnimationEnded;
        public Action OnAcceptedRewards;

        private bool _isSkillPanelOpened;

        public static UIManager Instance { get; set; }

        private void Awake()
        {
            Instance = this;
        }

        public void Initialize(Vector3 sphereCenter, float sphereRadius)
        {
            _optionsPopup.gameObject.SetActive(true);
            _optionsButton.OnClicked += OptionsButton_OnClicked;
            _leaveButton.OnClicked += LeaveButton_OnClicked;
            _leaveConfirmationPopup.gameObject.SetActive(true);
            _leaveConfirmationButton.OnClicked += LeaveConfirmationButton_OnClicked;
            _optionsPopup.OnClosed += OptionsPopup_OnClosed;
            _xpBar.Initialize();
            HideGameGoldCounter();
            _skillSelectionPanel.Hide();
            _isSkillPanelOpened = false;
            _rankNumberPanel.Initialize();
            _miniBossIndicator.Initialize(sphereCenter, sphereRadius);
            _bossIndicator.Initialize(sphereCenter, sphereRadius);
            _heroCamera.SetActive(false);

            WeaponType weaponType = EquipmentDataManager.Instance.GetEquippedWeaponType();
            GameObject playerPrefab = _humanHeroPrefabManager.GetHeroPrefabByWeaponType(weaponType);
            playerPrefab.SetActive(true);
        }

        public void UpdateCountDownPanel(float time)
        {
            _countDownPanel.UpdateTime(time);
        }
        
        public void HideCountDownPanel()
        {
            _countDownPanel.Hide();
        }
        
        public void HideRanksPanels()
        {
            _rankingPanel.Hide();
            _rankNumberPanel.Hide();
        }
        
        public void HideOptionsButton()
        {
            _optionsButton.gameObject.SetActive(false);
        }

        public void HideXpBar()
        {
            _xpBar.gameObject.SetActive(false);
        }
        
        private void HideGameGoldCounter()
        {
            _gameGoldCounter.gameObject.SetActive(false);
        }
        
        public void InitializeGameGoldCounter()
        {
            _gameGoldCounter.gameObject.SetActive(true);
            _gameGoldCounter.Initialize();
        }
        
        private void LeaveButton_OnClicked(InteractableButton button)
        {
            _leaveButton.OnClicked -= LeaveButton_OnClicked;
            _optionsPopup.ChangeAnimatorSpeed(RegularAnimationSpeed);
            _leaveConfirmationPopup.OnClosed += LeaveConfirmationButton_OnClosed;
            _leaveConfirmationPopup.ChangeAnimatorSpeed(PausedAnimationSpeed);
            _optionsPopup.Hide();

            StartCoroutine(WaitAndShowLeaveConfirmationPopupCoroutine());
            // OnOptionsPopupOpened?.Invoke();
        }

        private IEnumerator WaitAndShowLeaveConfirmationPopupCoroutine()
        {
            yield return new WaitForSeconds(_leaveConfirmationPopupDelay);
            _leaveConfirmationPopup.Show();
        }

        private void LeaveConfirmationButton_OnClicked(InteractableButton button)
        {
            _leaveConfirmationButton.OnClicked -= LeaveConfirmationButton_OnClicked;
            _leaveConfirmationPopup.ChangeAnimatorSpeed(RegularAnimationSpeed);

            OnLeaveButtonClicked?.Invoke();
        }

        private void OptionsPopup_OnClosed()
        {
            _optionsPopup.OnClosed -= OptionsPopup_OnClosed;
            _optionsButton.OnClicked += OptionsButton_OnClicked;
            _optionsPopup.ChangeAnimatorSpeed(RegularAnimationSpeed);

            // OnOptionsPopupClosed?.Invoke();
        }

        private void LeaveConfirmationButton_OnClosed()
        {
            _leaveConfirmationPopup.OnClosed -= LeaveConfirmationButton_OnClosed;
            _leaveButton.OnClicked += LeaveButton_OnClicked;
            _leaveConfirmationPopup.ChangeAnimatorSpeed(RegularAnimationSpeed);

            // OnOptionsPopupClosed?.Invoke();
        }

        private void OptionsButton_OnClicked(InteractableButton button)
        {
            _optionsButton.OnClicked -= OptionsButton_OnClicked;
            _optionsPopup.OnClosed += OptionsPopup_OnClosed;
            _optionsPopup.ChangeAnimatorSpeed(PausedAnimationSpeed);
            _optionsPopup.Show();

            // OnOptionsPopupOpened?.Invoke();
        }

        public void ShowMatchResultsScreen(bool isHumanPlayerDead, bool isMatchTutorial = false)
        {
            _heroCamera.SetActive(true);
            _rewardsPanel.OnAcceptedRewards += RewardsPanel_OnAcceptedRewards;
            _rewardsPanel.Initialize(isHumanPlayerDead, isMatchTutorial);
            _rewardsPanel.Show();
        }

        private void RewardsPanel_OnAcceptedRewards()
        {
            _rewardsPanel.OnAcceptedRewards -= RewardsPanel_OnAcceptedRewards;
            OnAcceptedRewards?.Invoke();
        }

        public void ShowGoldDungeonMatchResultsScreen(bool isHumanPlayerDead)
        {
            _goldRewardsPanel.OnAcceptedRewards += GoldDungeonRewardsPanel_OnAcceptedRewards;
            _goldRewardsPanel.Initialize(isHumanPlayerDead);
            _goldRewardsPanel.Show();
        }

        private void GoldDungeonRewardsPanel_OnAcceptedRewards()
        {
            _goldRewardsPanel.OnAcceptedRewards -= RewardsPanel_OnAcceptedRewards;
            OnAcceptedRewards?.Invoke();
        }

        public void HideInteractableUI()
        {
            _bossWarningOverlay.gameObject.SetActive(false);
            _skillSelectionPanel.Hide();
            _miniBossIndicator.Disable();
            _bossIndicator.Disable();
        }

        public void ShowSkillSelectionPanel(List<PlayerSkillData> showSkills)
        {
            if (_isSkillPanelOpened)
            {
                return;
            }

            _isSkillPanelOpened = true;
            
            _skillSelectionPanel.Initialize(showSkills);
            _skillSelectionPanel.OnSkillSelected += SkillSelectionPanel_OnSkillSelected;
        }

        private void SkillSelectionPanel_OnSkillSelected(PlayerSkillData skillData)
        {
            _isSkillPanelOpened = false;
            _skillSelectionPanel.OnSkillSelected -= SkillSelectionPanel_OnSkillSelected;
            OnLevelUpSkillSelected?.Invoke(skillData);
        }

        public bool IsSkillPanelOpened()
        {
            return _isSkillPanelOpened;
        }

        public float GetTimeBetweenSkillSelection()
        {
            return _skillSelectionPanel.TimeBetweenSkillSelection();
        }
        
        public int GetAmountOfSkillsToChoose()
        {
            return _skillSelectionPanel.GetAmountOfSkillsToChoose();
        }

        public void ShowStartGameAnimation()
        {
            _startGamePanel.SetActive(true);

            StartCoroutine(WaitAndEndStartGameAnimation());
        }

        private IEnumerator WaitAndEndStartGameAnimation()
        {
            yield return new WaitForSeconds(_startGamePanelAnimationClip.length);
            
            _startGamePanel.SetActive(false);
            OnStartGameAnimationEnded?.Invoke();
        }

        public void ShowPlayerKilledStatusPanel(int playerIndex)
        {
            _playerKilledStatusPanel.Show(playerIndex == Player.GetHumanPlayerIndex() ? _humanPlayerKilledStatusPanelDuration : _npcPlayerKilledStatusPanelDuration, playerIndex);
        }
        
        public void ShowPlayerKilledBossStatusPanel(int playerIndex, string bossName)
        {
            _heroKilledBossStatusPanel.Show(_humanPlayerKilledStatusPanelDuration, playerIndex, bossName);
        }

        public void ShowTimeIsUpOverlay()
        {
            _timeIsUpOverlay.SetActive(true);
            StartCoroutine(WaitAndDisableOverlay(_timeIsUpOverlay));
        }

        public void ShowDefeatedOverlay()
        {
            _defeatedOverlay.SetActive(true);
            StartCoroutine(WaitAndDisableOverlay(_defeatedOverlay));
        }

        public void ShowSurvivorOverlay()
        {
            _survivorOverlay.SetActive(true);
            StartCoroutine(WaitAndDisableOverlay(_survivorOverlay));
        }

        public void ShowBossWarningOverlay()
        {
            _bossWarningOverlay.gameObject.SetActive(true);
            _bossWarningOverlay.Initialize();
            StartCoroutine(WaitAndDisableOverlayForSeconds(_bossWarningOverlay.gameObject, _bossWarningOverlayDuration));
        }

        private IEnumerator WaitAndDisableOverlayForSeconds(GameObject overlay, float dangerOverlayTimer)
        {
            yield return new WaitForSeconds(dangerOverlayTimer);
            overlay.SetActive(false);
        }
        
        private IEnumerator WaitAndDisableOverlay(GameObject overlay)
        {
            yield return new WaitForSeconds(_overlayAnimationClip.length);
            overlay.SetActive(false);
        }
        
        public float GetOverlayDuration()
        {
            return _overlayAnimationClip.length;
        }

        public float GetPlayerKilledStatusPanelDuration(int playerIndex)
        {
            return playerIndex == Player.GetHumanPlayerIndex()
                ? _humanPlayerKilledStatusPanelDuration
                : _npcPlayerKilledStatusPanelDuration;
        }

        public void EnableMiniBossIndicator()
        {
            _miniBossIndicator.Enable();
        }

        public void DisableMiniBossIndicator()
        {
            _miniBossIndicator.Disable();
        }

        public void EnableBossIndicator()
        {
            _bossIndicator.Enable();
        }

        public void DisableBossIndicator()
        {
            _bossIndicator.Disable();
        }

        public float BossWarningOverlayDuration()
        {
            return _bossWarningOverlayDuration;
        }

        public void InitializeForMatchTutorial()
        {
            _rankingPanel.InitializeForMatchTutorial();
            RectTransform rankNumberRectTransform = _rankNumberPanel.GetComponent<RectTransform>();
            Vector3 rankNumberPosition = rankNumberRectTransform.localPosition;
            rankNumberRectTransform.localPosition = new Vector3(rankNumberPosition.x, rankNumberPosition.y + 50, rankNumberPosition.z);
            
            _optionsButton.OnClicked -= OptionsButton_OnClicked;
            _optionsButton.gameObject.SetActive(false);

            HideCountDownPanel();
        }
    }
}