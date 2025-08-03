using System;
using Application.Data.Talent;
using Game.Talent;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.MainMenu
{
    public class UpgradedTalentPopup : Popup
    {
        [SerializeField] private Image _upgradedTalentIcon;
        [SerializeField] private TextMeshProUGUI _upgradedTalentLevelText;
        [SerializeField] private TextMeshProUGUI _upgradedTalentNameText;
        [SerializeField] private TextMeshProUGUI _upgradedTalentDescriptionText;
        
        private const string Max = "Max";
        private const string Level = "Lv.";
    
        public void Initialize(int talentIndex)
        {
            int talentLevel = TalentsDataManager.Instance.GetTalentLevel((TalentType)talentIndex);
            TalentData talentData = TalentManager.Instance.GetTalentsData().Talents[talentIndex];

            _upgradedTalentNameText.text = talentData.Name;
            _upgradedTalentDescriptionText.text = String.Format(talentData.UpgradeComparisonDescription, talentLevel == 1 ? 0 : talentData.UpgradeAmountPerLevel[talentLevel - 2], talentData.UpgradeAmountPerLevel[talentLevel - 1]);
            _upgradedTalentLevelText.text = talentLevel == talentData.UpgradeAmountPerLevel.Length ? Max : $"{Level}{talentLevel}";
            _upgradedTalentIcon.sprite = TalentManager.Instance.GetTalentsData().Talents[talentIndex].Icon;
        }

        public override void Show()
        {
            gameObject.SetActive(true);
            base.Show();
        }
    }
}