using System.Linq;
using Application.Data.Talent;
using UnityEngine;

namespace Game.Talent
{
    public class TalentManager : MonoBehaviour
    {
        [SerializeField] private TalentsData _talentsData;

        private const float DefaultModifier = 0.0f;

        public static TalentManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        public TalentsData GetTalentsData()
        {
            return _talentsData;
        }

        public float GetMaxHpTalentAddition(int forcedLevel = -1)
        {
            return GetTalentModifier(TalentType.Strength, DefaultModifier, forcedLevel);
        }

        public float GetAttackTalentAddition(int forcedLevel = -1)
        {
            return GetTalentModifier(TalentType.Power, DefaultModifier, forcedLevel);
        }

        public float GetDamageReceivedTalentReduction(int forcedLevel = -1)
        {
            return GetTalentModifier(TalentType.Armor, DefaultModifier, forcedLevel);
        }

        public float GetAttackSpeedTalentMultiplier(int forcedLevel = -1)
        {
            return GetTalentModifier(TalentType.Agile, DefaultModifier, forcedLevel);
        }

        public float GetHpRecoveredOnLevelUpTalentPercent(int forcedLevel = -1)
        {
            return GetTalentModifier(TalentType.Recover, DefaultModifier, forcedLevel);
        }

        public bool GetStartingSkillsTalent(int forcedLevel = -1)
        {
            return GetTalentModifier(TalentType.Veteran, DefaultModifier, forcedLevel) > 0;
        }

        public float GetCollectRadiusTalentMultiplier(int forcedLevel = -1)
        {
            return GetTalentModifier(TalentType.Magnet, DefaultModifier, forcedLevel);
        }

        public float GetAttackRangeTalentPercent(int forcedLevel = -1)
        {
            return GetTalentModifier(TalentType.Range, DefaultModifier, forcedLevel);
        }

        public float GetMovementSpeedTalentPercent(int forcedLevel = -1)
        {
            return GetTalentModifier(TalentType.Boots, DefaultModifier, forcedLevel);
        }
        
        private float GetTalentModifier(TalentType talentType, float defaultModifier, int forcedLevel)
        {
            foreach (TalentData talentData in _talentsData.Talents)
            {
                if (talentData.Type == talentType)
                {
                    if (forcedLevel > 0)
                    {
                        return talentData.UpgradeAmountPerLevel[forcedLevel - 1];
                    }

                    int talentLevel = TalentsDataManager.Instance.GetTalentLevel(talentData.Type);

                    if (talentLevel == 0)
                    {
                        return defaultModifier;
                    }

                    return talentData.UpgradeAmountPerLevel[talentLevel - 1];
                }
            }

            return defaultModifier;
        }

        public int CostForNextUpgrade()
        {
            int numberOfTalentUpgrades = TalentsDataManager.Instance.GetNumberOfTalentUpgrades();
            return numberOfTalentUpgrades < _talentsData.GoldCostPerUpgrade.Length ? _talentsData.GoldCostPerUpgrade[numberOfTalentUpgrades] : _talentsData.GoldCostPerUpgrade.Last();
        }

        public bool CanUpgrade()
        {
            return TalentsDataManager.Instance.CanUpgrade(_talentsData);
        }
    }
}