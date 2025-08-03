using System;
using System.Collections;
using System.Collections.Generic;
using Application.Data;
using Application.Data.Currency;
using Application.Data.Equipment;
using Application.Data.Progression;
using Application.Data.Rewards;
using Application.Sound;
using Application.Telemetry;
using Application.Utils;
using DG.Tweening;
using Game.Ftue;
using Game.Pool;
using Game.UI;
using TMPro;
using UI.Button;
using UI.Counter;
using UnityEngine;
using MaterialReward = Application.Data.Rewards.MaterialReward;
using Random = UnityEngine.Random;

namespace UI.MainMenu
{
    public class MenuManager : MonoBehaviour
    {
        [SerializeField] private NavigationBar _navigationBar;
        [SerializeField] private InteractableButton _optionsButton;
        [SerializeField] private Popup _optionsPopup;
        [SerializeField] private GoldCounter _goldCounter;
        [SerializeField] private GameObject _screenInputBlocker;
        [SerializeField] private GameObject _equipmentDestination;
        [SerializeField] private GameObject _goldDestination;
        [SerializeField] private GameObject _trophiesDestination;
        [SerializeField] private Animator _trophiesPlayerAnimator;
        [SerializeField] private Animator _trophiesPlayerAnimatorSubtractAmount;
        [SerializeField] private TextMeshProUGUI _trophiesPlayerAnimatorSubtractAmountText;
        [SerializeField] private GameObject _shopItemsDestination;
        [SerializeField] private GameObject _particlesOrigin;
        [SerializeField] private Animator _goldAnimator;
        [SerializeField] private Animator _shopItemsAnimator;
        [SerializeField] private Animator _equipmentAnimator;
        [SerializeField] private LeagueUpCelebrationPanel _leagueUpCelebrationPanel;
        [SerializeField] private GameObject _leagueUpCelebrationPanelParticles;
        [SerializeField] private LeagueScreen _leagueScreen;
        [SerializeField] private LeagueScreenButton _leagueScreenButton;
        [SerializeField] private float _leagueScreenButtonTransitionDuration;
        [SerializeField] private TextMeshProUGUI _playerNameText;
        [SerializeField] private TMP_InputField _newPlayerNameText;
        [SerializeField] private InteractableButton _playerNameButton;
        [SerializeField] private InteractableButton _playerNameChangeButton;
        [SerializeField] private Popup _playerNamePopup;
        [SerializeField] private InteractableButton _scoutButton;
        [SerializeField] private ScoutPanel _scoutPopup;
        [SerializeField] private ScoutRewardsPanel _scoutRewardsPopup;
        [SerializeField] private PlanetProgression _planetProgression;

        private static readonly int WobbleTrigger = Animator.StringToHash("Wobble");
        private static readonly int SubtractAmountTrigger = Animator.StringToHash("SubtractAmount");

        private const float LeagueScreenOriginalPosition = -1500.0f;
        private const float LeagueScreenShowPosition = 0.0f;
        public static readonly float CreateParticlesInitialDelay = 0.5f;

        private void Start()
        {
            QualitySettings.vSyncCount = 0;
            UnityEngine.Application.targetFrameRate = 60;

            MusicManager.Instance.PlayMenuMusic();
            ScreenFader.ScreenFader.Instance.FadeOut();
            ObjectPool.Instance.Initialize();

            _goldCounter.Initialize(CurrencyDataManager.Instance.CurrentGold());
            _navigationBar.Initialize();
            _optionsButton.OnClicked += OptionsButton_OnClicked;
            _scoutButton.OnClicked += ScoutButton_OnClicked;
            _scoutRewardsPopup.OnClosed += ScoutRewardsPopup_OnClosed;

            FtueManager.Instance.Initialize();
            FtueManager.Instance.ShowNextStep();

            UpdateUIWithGoldAndRewardedParticles();
            _leagueUpCelebrationPanelParticles.SetActive(false);
            _leagueScreen.Initialize();
            _leagueScreenButton.OnClicked += LeagueScreenButton_OnClicked;
            _leagueScreenButton.Initialize();
            
            _playerNameText.text = NameDataManager.Instance.GetPlayerName();
            _newPlayerNameText.text = NameDataManager.Instance.GetPlayerName();
            _newPlayerNameText.onFocusSelectAll = true;
            _playerNameButton.OnClicked += PlayerNameButton_OnClicked;
            _playerNameChangeButton.OnClicked += PlayerNameChangeButton_OnClicked;
            
            _planetProgression.Initialize();
        }

        private void ScoutRewardsPopup_OnClosed()
        {
            StartCoroutine(CreateParticles());
        }

        private void ScoutButton_OnClicked(InteractableButton button)
        {
            _scoutPopup.Initialize();
            _scoutPopup.Show();
        }

        private void PlayerNameButton_OnClicked(InteractableButton button)
        {
            _playerNamePopup.OnShown += PlayerNamePopup_OnShown;
            _playerNamePopup.Show();
        }

        private void PlayerNamePopup_OnShown()
        {
            _playerNamePopup.OnShown -= PlayerNamePopup_OnShown;
            _newPlayerNameText.Select();
            _newPlayerNameText.ActivateInputField();
        }

        private void PlayerNameChangeButton_OnClicked(InteractableButton button)
        {
            _playerNamePopup.Hide();
            string newPlayerName = _newPlayerNameText.text;
            _playerNameText.text = newPlayerName;
            NameDataManager.Instance.SavePlayerName(newPlayerName);
        }
        
        private void LeagueScreenButton_OnClicked(InteractableButton button)
        {
            _leagueScreenButton.OnClicked -= LeagueScreenButton_OnClicked;
            _leagueScreen.OnOkButtonClicked += OnOkButtonClicked;
            _leagueScreen.OpenLeagueScreen();
            _leagueScreen.GetComponent<RectTransform>().DOMoveX(LeagueScreenShowPosition, _leagueScreenButtonTransitionDuration);
        }

        private void OnOkButtonClicked()
        {
            _leagueScreen.OnOkButtonClicked -= OnOkButtonClicked;
            _leagueScreenButton.OnClicked += LeagueScreenButton_OnClicked;
            _leagueScreen.GetComponent<RectTransform>().DOMoveX(LeagueScreenOriginalPosition, _leagueScreenButtonTransitionDuration);
        }

        public void UpdateUIWithGoldAndRewardedParticles()
        {
            _screenInputBlocker.SetActive(true);
            StartCoroutine(CreateParticles());
        }


        private IEnumerator CreateParticles()
        {
            if (!RewardUIParticleManager.Instance.HasSomethingToReward() && FtueManager.Instance.HasFinishedThirdRealGame())
            {
                yield return new WaitForSeconds(1.0f);
                _screenInputBlocker.SetActive(false);
                yield break;
            }

            int trophiesWonAmount = RewardUIParticleManager.Instance.TrophiesWonAmount();
            yield return new WaitForSeconds(CreateParticlesInitialDelay);
            bool triggerNewLeaguePanel = false;
            int currentLeagueIndex = LeaguesManager.Instance.GetCurrentLeagueIndex();
            int previousLeagueIndex = Math.Max(0, currentLeagueIndex - 1);
            LeagueData leagueData = LeaguesManager.Instance.GetLeagueInfo(currentLeagueIndex);

            if (trophiesWonAmount < 0)
            {
                StartCoroutine(LoseTrophiesLogic(trophiesWonAmount));
            }
            else if (trophiesWonAmount > 0 && currentLeagueIndex != previousLeagueIndex)
            {
                int currentTrophies = CurrencyDataManager.Instance.CurrentTrophies();
                int previousTrophies = currentTrophies - trophiesWonAmount;
                triggerNewLeaguePanel = previousTrophies < leagueData.MinTrophiesInclusive && !ProgressionDataManager.Instance.HasReceivedLeagueReward(previousLeagueIndex);
            }

            if (triggerNewLeaguePanel)
            {
                LeagueScreenButton.OnTrophiesWonChoreographyEnded += LeagueScreenButton_OnTrophiesWonChoreographyEnded;
            }
            
            RewardUIParticleManager.Instance.OnTrophyParticleReached += RewardUIParticleManager_OnTrophiesParticleReached;
            RewardUIParticleManager.Instance.OnGoldParticleReached += RewardUIParticleManager_OnGoldParticleReached;
            RewardUIParticleManager.Instance.OnGemParticleReached += RewardUIParticleManager_OnGemParticleReached;
            RewardUIParticleManager.Instance.OnKeyParticleReached += RewardUIParticleManager_OnKeyParticleReached;
            RewardUIParticleManager.Instance.OnMaterialOrEquipmentParticleReached += RewardUIParticleManager_OnMaterialOrEquipmentParticleReached;
            Vector3 particlesOriginPosition = _particlesOrigin.transform.position;

            if (trophiesWonAmount > 0)
            {
                StartCoroutine(RewardUIParticleManager.Instance.CreateTrophiesParticles(particlesOriginPosition, _trophiesDestination.transform.position));
                yield return new WaitForSeconds(Random.value);
            }

            if (RewardUIParticleManager.Instance.HasGoldParticlesToCreate())
            {
                StartCoroutine(RewardUIParticleManager.Instance.CreateGoldParticles(particlesOriginPosition, _goldDestination.transform.position));
                yield return new WaitForSeconds(Random.value);
            }

            if (RewardUIParticleManager.Instance.HasGemsParticlesToCreate())
            {
                StartCoroutine(RewardUIParticleManager.Instance.CreateGemsParticles(particlesOriginPosition, _shopItemsDestination.transform.position));
                yield return new WaitForSeconds(Random.value);
            }

            if (RewardUIParticleManager.Instance.HasKeyParticlesToCreate())
            {
                StartCoroutine(RewardUIParticleManager.Instance.CreateKeysParticles(particlesOriginPosition, _shopItemsDestination.transform.position));
                yield return new WaitForSeconds(Random.value);
            }
            
            if (RewardUIParticleManager.Instance.HasMaterialParticlesToCreate())
            {
                StartCoroutine(RewardUIParticleManager.Instance.CreateMaterialParticles(particlesOriginPosition, _equipmentDestination.transform.position));
                yield return new WaitForSeconds(Random.value);
            }

            if (RewardUIParticleManager.Instance.HasEquipmentParticlesToCreate())
            {
                StartCoroutine(RewardUIParticleManager.Instance.CreateEquipmentParticles(particlesOriginPosition, _equipmentDestination.transform.position));
            }

            if (!triggerNewLeaguePanel)
            {
                yield return new WaitForSeconds(NumberConstants.Half);
                _screenInputBlocker.SetActive(false);
            }
        }

        private void LeagueScreenButton_OnTrophiesWonChoreographyEnded()
        {
            LeagueScreenButton.OnTrophiesWonChoreographyEnded -= LeagueScreenButton_OnTrophiesWonChoreographyEnded;
            StartCoroutine(WaitAndShowLevelUpPanel());
        }

        private IEnumerator WaitAndShowLevelUpPanel()
        {
            yield return new WaitForSeconds(NumberConstants.Half);
            int currentLeagueIndex = LeaguesManager.Instance.GetCurrentLeagueIndex();
            int previousLeagueIndex = Math.Max(NumberConstants.Zero, currentLeagueIndex - NumberConstants.One);
            LeagueData leagueData = LeaguesManager.Instance.GetLeagueInfo(currentLeagueIndex);
            LeagueData previousLeagueData = LeaguesManager.Instance.GetLeagueInfo(previousLeagueIndex);
            _leagueUpCelebrationPanelParticles.SetActive(true);
            _leagueUpCelebrationPanel.Initialize(previousLeagueData, leagueData);
            GiveRewardsFromLeagueFirstTimeReach(previousLeagueData);
            _leagueUpCelebrationPanel.OnLeagueUpCelebrationPanelPopupClosed += OnLeagueUpCelebrationPanelPopupClosed;
            ProgressionDataManager.Instance.ReceiveLeagueReward(previousLeagueIndex);
        }

        private void RewardUIParticleManager_OnTrophiesParticleReached()
        {
            if (_trophiesPlayerAnimator)
            {
                _trophiesPlayerAnimator.SetTrigger(WobbleTrigger);
            }
        }

        private void RewardUIParticleManager_OnGoldParticleReached()
        {
            if (_goldAnimator)
            {
                _goldAnimator.SetTrigger(WobbleTrigger);
            }
        }

        private void RewardUIParticleManager_OnGemParticleReached()
        {
            if (_shopItemsAnimator)
            {
                _shopItemsAnimator.SetTrigger(WobbleTrigger);
            }
        }

        private void RewardUIParticleManager_OnKeyParticleReached()
        {
            if (_shopItemsAnimator)
            {
                _shopItemsAnimator.SetTrigger(WobbleTrigger);
            }
        }

        private void RewardUIParticleManager_OnMaterialOrEquipmentParticleReached()
        {
            if (_equipmentAnimator)
            {
                _equipmentAnimator.SetTrigger(WobbleTrigger);
            }
        }
        private void OnLeagueUpCelebrationPanelPopupClosed()
        {
            _leagueUpCelebrationPanel.OnLeagueUpCelebrationPanelPopupClosed -= OnLeagueUpCelebrationPanelPopupClosed;
            _leagueUpCelebrationPanelParticles.SetActive(false);
            UpdateUIWithGoldAndRewardedParticles();
        }

        private IEnumerator LoseTrophiesLogic(int trophiesWonAmount)
        {
            _trophiesPlayerAnimatorSubtractAmountText.text = $"{trophiesWonAmount}";
            _trophiesPlayerAnimatorSubtractAmount.SetTrigger(SubtractAmountTrigger);
            yield return new WaitForSeconds(NumberConstants.Half);
            _trophiesPlayerAnimator.SetTrigger(WobbleTrigger);
        }

        private void GiveRewardsFromLeagueFirstTimeReach(LeagueData leagueData)
        {
            LeagueReachedRewardsData rewardsData = leagueData.LeagueReachedReward;

            if (rewardsData.Gold > 0)
            {
                CurrencyDataManager.Instance.GainGold(rewardsData.Gold);
                AmplitudeManager.Instance.SoftCurrencyBalanceChange(SoftCurrencyType.Gold, rewardsData.Gold, CurrencyDataManager.Instance.CurrentGold(), SoftCurrencyModificationType.LeagueReachedReward);
            }

            if (rewardsData.Gems > 0)
            {
                CurrencyDataManager.Instance.GainGems(rewardsData.Gems);
                AmplitudeManager.Instance.SoftCurrencyBalanceChange(SoftCurrencyType.Gems, rewardsData.Gems, CurrencyDataManager.Instance.CurrentGems(), SoftCurrencyModificationType.LeagueReachedReward);
            }

            if (rewardsData.SilverKeys > 0)
            {
                CurrencyDataManager.Instance.GainSilverKeys(rewardsData.SilverKeys);
                AmplitudeManager.Instance.SoftCurrencyBalanceChange(SoftCurrencyType.SilverKey, rewardsData.SilverKeys, CurrencyDataManager.Instance.CurrentSilverKeys(), SoftCurrencyModificationType.LeagueReachedReward);
            }

            if (rewardsData.GoldenKeys > 0)
            {
                CurrencyDataManager.Instance.GainSilverKeys(rewardsData.GoldenKeys);
                AmplitudeManager.Instance.SoftCurrencyBalanceChange(SoftCurrencyType.GoldenKey, rewardsData.GoldenKeys, CurrencyDataManager.Instance.CurrentGoldenKeys(), SoftCurrencyModificationType.LeagueReachedReward);
            }

            List<EquipmentPieceType> materialParticlesToCreate = new List<EquipmentPieceType>();
            List<int> equipmentParticlesToCreate = new List<int>();

            if (rewardsData.Materials.Length > 0)
            {
                foreach (MaterialReward materialReward in rewardsData.Materials)
                {
                    CurrencyDataManager.Instance.GainMaterials(materialReward.Type, materialReward.Amount);
                    AmplitudeManager.Instance.SoftCurrencyBalanceChange(EnumUtils.SoftCurrencyTypeByPieceType(materialReward.Type), materialReward.Amount, CurrencyDataManager.Instance.CurrentMaterials(materialReward.Type), SoftCurrencyModificationType.LeagueReachedReward);
                    materialParticlesToCreate.Add(materialReward.Type);
                }
            }

            if (rewardsData.Equipments.Length > 0)
            {
                EquipmentDataManager.Instance.AddPieceListToInventory(rewardsData.Equipments);

                foreach (EquipmentReward equipmentReward in rewardsData.Equipments)
                {
                    equipmentParticlesToCreate.Add(equipmentReward.Type.Id);
                }
            }

            RewardUIParticleManager.Instance.AddRewardsForNextMainMenuScreen(rewardsData.Gold, rewardsData.Gems, rewardsData.SilverKeys, rewardsData.GoldenKeys, equipmentParticlesToCreate, materialParticlesToCreate, 0);
        }

        private void OptionsButton_OnClicked(InteractableButton button)
        {
            _optionsPopup.Show();
            SoundManager.Instance.PlayMenuPopupButtonSound();
        }
    }
}