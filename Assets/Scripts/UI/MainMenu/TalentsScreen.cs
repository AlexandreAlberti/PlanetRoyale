using System;
using System.Collections;
using System.Collections.Generic;
using Application.Data.Currency;
using Application.Data.Talent;
using Application.Sound;
using Application.Telemetry;
using Game.Talent;
using TMPro;
using UI.Button;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace UI.MainMenu
{
    public class TalentsScreen : MonoBehaviour
    {
        [SerializeField] private bool _isLockEnabled;
        [SerializeField] private Sprite _lockedTalent;
        [SerializeField] private GameObject _lockedPanel;
        [SerializeField] private UpgradeButton _upgradeButton;
        [SerializeField] private TextMeshProUGUI _upgradeCostText;
        [SerializeField] private TextMeshProUGUI _numberOfUpgradesText;
        [SerializeField] private TextMeshProUGUI[] _talentNameTexts;
        [SerializeField] private TextMeshProUGUI[] _talentLevelTexts;
        [SerializeField] private TextMeshProUGUI[] _talentDescriptionTexts;
        [SerializeField] private Image[] _talentIcons;
        [SerializeField] private Image[] _talentBackground;
        [SerializeField] private Animator[] _talentAnimators;
        [SerializeField] private Color _lockedBackgroundColor;
        [SerializeField] private Color _obtainedBackgroundColor;
        [SerializeField] private UpgradedTalentPopup _upgradedTalentPopup;

        private const string Max = "Max";
        private const string Level = "Lv.";
        private const string Times = "times";
        private const string Locked = "Locked";
        private const string Upgraded = "Upgraded";
        private const int RouletteIterations = 10;
        private const float RouletteIterationDuration = 0.1f;
        private const float UpgradedTalentPanelWaitTime = 0.5f;

        private static readonly int Glow = Animator.StringToHash("Glow");
        private static readonly int Select = Animator.StringToHash("Select");
        private static readonly int ShowTooltip = Animator.StringToHash("ShowTooltip");
        private static readonly int HideTooltip = Animator.StringToHash("HideTooltip");
        private static readonly int Show = Animator.StringToHash("Show");
        private static readonly int Hide = Animator.StringToHash("Hide");

        private bool _isDoingRouletteAnimation;
        private bool _isUpgradeButtonEnabled;
        private int _tooltipTalentIndex;

        public Action OnTalentUpgradeStarted;
        public Action OnTalentUpgradeAnimationEnded;
        public Action OnTalentUpgradeEnded;

        public void OnEnable()
        {
            CurrencyDataManager.Instance.OnGoldUpdated -= CurrencyDataManager_UpdateGoldNeeded;
            CurrencyDataManager.Instance.OnGoldUpdated += CurrencyDataManager_UpdateGoldNeeded;
            _upgradedTalentPopup.OnClosed -= UpgradedTalentPopup_OnClosed;
            _upgradedTalentPopup.OnClosed += UpgradedTalentPopup_OnClosed;
            Refresh();
        }

        private void OnDestroy()
        {
            CurrencyDataManager.Instance.OnGoldUpdated -= CurrencyDataManager_UpdateGoldNeeded;
        }

        private void UpgradedTalentPopup_OnClosed()
        {
            if (_isUpgradeButtonEnabled)
            {
                _upgradeButton.Enable();
            }

            OnTalentUpgradeEnded?.Invoke();
        }

        private void CurrencyDataManager_UpdateGoldNeeded(int goldAmount)
        {
            RefreshUpgradeButton();
        }

        private void Refresh()
        {
            RefreshLockedPanel();
            RefreshTalents();
            RefreshNumberOfTalentUpgrades();
            RefreshUpgradeButton();
        }

        private void OnDisable()
        {
            ResetTooltip();
        }

        private void RefreshLockedPanel()
        {
            _lockedPanel.SetActive(_isLockEnabled);
        }

        private void RefreshNumberOfTalentUpgrades()
        {
            _numberOfUpgradesText.text = $"{Upgraded} {TalentsDataManager.Instance.GetNumberOfTalentUpgrades()} {Times}";
        }

        private void RefreshTalents()
        {
            _tooltipTalentIndex = -1;

            for (int i = 0; i < TalentManager.Instance.GetTalentsData().Talents.Length; i++)
            {
                RefreshTalent(i);
            }
        }

        private void RefreshTalent(int talentIndex)
        {
            int talentLevel = TalentsDataManager.Instance.GetTalentLevel((TalentType)talentIndex);
            Image talentIcon = _talentIcons[talentIndex];
            TalentData talentData = TalentManager.Instance.GetTalentsData().Talents[talentIndex];
            TextMeshProUGUI talentNameText = _talentNameTexts[talentIndex];
            TextMeshProUGUI talentLevelText = _talentLevelTexts[talentIndex];
            TextMeshProUGUI talentDescriptionText = _talentDescriptionTexts[talentIndex];
            Image talentBackground = _talentBackground[talentIndex];

            talentNameText.text = talentData.Name;
            talentDescriptionText.text = String.Format(talentData.Description, talentData.UpgradeAmountPerLevel[talentLevel == 0 ? 0 : talentLevel - 1]);
            talentLevelText.text = talentLevel == 0 ? Locked : talentLevel == talentData.UpgradeAmountPerLevel.Length ? Max : $"{Level}{talentLevel}";
            talentIcon.sprite = talentLevel == 0 ? _lockedTalent : talentData.Icon;
            talentBackground.color = talentLevel == 0 ? _lockedBackgroundColor : _obtainedBackgroundColor;
        }

        private void RefreshUpgradeButton()
        {
            int costForNextUpgrade = TalentManager.Instance.CostForNextUpgrade();
            _upgradeCostText.text = $"x{costForNextUpgrade}";

            if (CurrencyDataManager.Instance.CurrentGold() < costForNextUpgrade)
            {
                _isUpgradeButtonEnabled = false;
                _upgradeCostText.color = Color.red;
                _upgradeButton.Disable();
            }
            else if (!TalentManager.Instance.CanUpgrade())
            {
                _isUpgradeButtonEnabled = false;
                _upgradeButton.Disable();
            }
            else
            {
                _isUpgradeButtonEnabled = true;
                _upgradeCostText.color = Color.white;
                _upgradeButton.Enable();
            }
        }

        public void OnTalentPressed(int talentIndex)
        {
            ResetTooltip();

            Animator talentAnimator = _talentAnimators[talentIndex];

            _tooltipTalentIndex = talentIndex;
            talentAnimator.ResetTrigger(ShowTooltip);
            talentAnimator.SetTrigger(ShowTooltip);
        }

        public void ResetTooltip()
        {
            if (_tooltipTalentIndex == -1)
            {
                return;
            }

            _talentAnimators[_tooltipTalentIndex].ResetTrigger(HideTooltip);
            _talentAnimators[_tooltipTalentIndex].SetTrigger(HideTooltip);
            _tooltipTalentIndex = -1;
        }

        public void OnUpgradeTalentPressed()
        {
            if (!_upgradeButton.IsEnabled())
            {
                return;
            }
            
            _upgradeButton.Disable();

            if (!TalentManager.Instance.CanUpgrade())
            {
                RefreshTalents();
                return;
            }

            ResetTooltip();

            if (_isDoingRouletteAnimation)
            {
                return;
            }

            _isDoingRouletteAnimation = true;

            RefreshNumberOfTalentUpgrades();
            int numberOfTalentUpgrades = TalentsDataManager.Instance.GetNumberOfTalentUpgrades();
            int costForNextUpgrade = TalentManager.Instance.CostForNextUpgrade();
            TalentsDataManager.Instance.IncreaseNumberOfTalentUpgrades();
            CurrencyDataManager.Instance.RemoveGold(costForNextUpgrade);
            AmplitudeManager.Instance.SoftCurrencyBalanceChange(SoftCurrencyType.Gold, -costForNextUpgrade, CurrencyDataManager.Instance.CurrentGold(), SoftCurrencyModificationType.TalentUpgrade);
            RefreshUpgradeButton();

            StartCoroutine(RouletteAnimation(numberOfTalentUpgrades));

            OnTalentUpgradeStarted?.Invoke();
        }

        private IEnumerator RouletteAnimation(int numberOfTalentUpgrades)
        {
            int lastTalentGiven = TalentsDataManager.Instance.GetLastTalentGiven();
            var availableTalentsIndexList = AvailableTalentsIndexList();
            int randomTalent = ChooseRandomTalent(availableTalentsIndexList);

            for (int i = 0; i < RouletteIterations; i++)
            {
                _talentAnimators[randomTalent].SetTrigger(Glow);

                if (numberOfTalentUpgrades == 0 && i == RouletteIterations - 1)
                {
                    randomTalent = (int) TalentType.Veteran;
                }
                else if (i == RouletteIterations - 1 && availableTalentsIndexList.Count > 1 && lastTalentGiven >= 0)
                {
                    availableTalentsIndexList.Remove(lastTalentGiven);
                    randomTalent = ChooseRandomTalent(availableTalentsIndexList);
                }
                else
                {
                    randomTalent = ChooseRandomTalent(availableTalentsIndexList);
                }

                SoundManager.Instance.PlayUpgradeTalentButtonSound();
                yield return new WaitForSeconds(RouletteIterationDuration);
            }

            _talentAnimators[randomTalent].SetTrigger(Select);
            _isDoingRouletteAnimation = false;

            TalentsDataManager.Instance.IncreaseTalentLevel((TalentType)randomTalent);
            TalentsDataManager.Instance.SetLastTalentGiven(randomTalent);
            RefreshTalents();

            TalentData talentData = TalentManager.Instance.GetTalentsData().Talents[randomTalent];
            int talentLevel = TalentsDataManager.Instance.GetTalentLevel((TalentType)randomTalent);
            AmplitudeManager.Instance.TalentUpgrade(talentData.Name, talentLevel, numberOfTalentUpgrades);
            _upgradedTalentPopup.Initialize(randomTalent);
            yield return new WaitForSeconds(UpgradedTalentPanelWaitTime);
            SoundManager.Instance.PlayUpgradedTalentScreenSound();
            _upgradedTalentPopup.Show();

            OnTalentUpgradeAnimationEnded?.Invoke();
        }

        private List<int> AvailableTalentsIndexList()
        {
            TalentsData talentsData = TalentManager.Instance.GetTalentsData();
            List<int> availableTalentsIndexList = new List<int>();

            for (int i = 0; i < talentsData.Talents.Length; i++)
            {
                if (TalentsDataManager.Instance.GetTalentLevel((TalentType)i) < talentsData.Talents[i].UpgradeAmountPerLevel.Length)
                {
                    availableTalentsIndexList.Add(i);
                }
            }

            return availableTalentsIndexList;
        }

        private int ChooseRandomTalent(List<int> availableTalentsIndexList)
        {
            int randomIndex = Random.Range(0, availableTalentsIndexList.Count);
            return availableTalentsIndexList[randomIndex];
        }
    }
}