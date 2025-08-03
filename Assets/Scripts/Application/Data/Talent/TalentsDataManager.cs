using System;
using UnityEngine;

namespace Application.Data.Talent
{
    public class TalentsDataManager : MonoBehaviour
    {
        private const string NumberOfTalentUpgrades = "NumberOfTalentUpgrades";
        private const string StrengthTalentLevel = "StrengthTalentLevel";
        private const string PowerTalentLevel = "PowerTalentLevel";
        private const string ArmorTalentLevel = "ArmorTalentLevel";
        private const string AgileTalentLevel = "AgileTalentLevel";
        private const string RecoverTalentLevel = "RecoverTalentLevel";
        private const string VeteranTalentLevel = "VeteranTalentLevel";
        private const string MagnetTalentLevel = "MagnetTalentLevel";
        private const string RangeTalentLevel = "RangeTalentLevel";
        private const string BootsTalentLevel = "BootsTalentLevel";
        private const string LastTalentGiven = "LastTalentGiven";

        public static TalentsDataManager Instance { get; set; }

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

        public int GetNumberOfTalentUpgrades()
        {
            return PlayerPrefs.GetInt(NumberOfTalentUpgrades, 0);
        }

        public void IncreaseNumberOfTalentUpgrades()
        {
            PlayerPrefs.SetInt(NumberOfTalentUpgrades, GetNumberOfTalentUpgrades() + 1);
        }

        public int GetLastTalentGiven()
        {
            return PlayerPrefs.GetInt(LastTalentGiven, -1);
        }

        public void SetLastTalentGiven(int talentGiven)
        {
            PlayerPrefs.SetInt(LastTalentGiven, talentGiven);
        }
        
        public int GetTalentLevel(TalentType talentType)
        {
            switch (talentType)
            {
                case TalentType.Strength:
                    return PlayerPrefs.GetInt(StrengthTalentLevel);
                case TalentType.Power:
                    return PlayerPrefs.GetInt(PowerTalentLevel);
                case TalentType.Armor:
                    return PlayerPrefs.GetInt(ArmorTalentLevel);
                case TalentType.Agile:
                    return PlayerPrefs.GetInt(AgileTalentLevel);
                case TalentType.Recover:
                    return PlayerPrefs.GetInt(RecoverTalentLevel);
                case TalentType.Veteran:
                    return PlayerPrefs.GetInt(VeteranTalentLevel);
                case TalentType.Magnet:
                    return PlayerPrefs.GetInt(MagnetTalentLevel);
                case TalentType.Range:
                    return PlayerPrefs.GetInt(RangeTalentLevel);
                case TalentType.Boots:
                    return PlayerPrefs.GetInt(BootsTalentLevel);
                default:
                    throw new NotImplementedException();
            }
        }

        public void IncreaseTalentLevel(TalentType talentType)
        {
            switch (talentType)
            {
                case TalentType.Strength:
                    PlayerPrefs.SetInt(StrengthTalentLevel, GetTalentLevel(talentType) + 1);
                    break;
                case TalentType.Power:
                    PlayerPrefs.SetInt(PowerTalentLevel, GetTalentLevel(talentType) + 1);
                    break;
                case TalentType.Armor:
                    PlayerPrefs.SetInt(ArmorTalentLevel, GetTalentLevel(talentType) + 1);
                    break;
                case TalentType.Agile:
                    PlayerPrefs.SetInt(AgileTalentLevel, GetTalentLevel(talentType) + 1);
                    break;
                case TalentType.Recover:
                    PlayerPrefs.SetInt(RecoverTalentLevel, GetTalentLevel(talentType) + 1);
                    break;
                case TalentType.Veteran:
                    PlayerPrefs.SetInt(VeteranTalentLevel, GetTalentLevel(talentType) + 1);
                    break;
                case TalentType.Magnet:
                    PlayerPrefs.SetInt(MagnetTalentLevel, GetTalentLevel(talentType) + 1);
                    break;
                case TalentType.Range:
                    PlayerPrefs.SetInt(RangeTalentLevel, GetTalentLevel(talentType) + 1);
                    break;
                case TalentType.Boots:
                    PlayerPrefs.SetInt(BootsTalentLevel, GetTalentLevel(talentType) + 1);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public void SetAllTalentsToLevel(int level)
        {
            PlayerPrefs.SetInt(StrengthTalentLevel, level);
            PlayerPrefs.SetInt(PowerTalentLevel, level);
            PlayerPrefs.SetInt(ArmorTalentLevel, level);
            PlayerPrefs.SetInt(AgileTalentLevel, level);
            PlayerPrefs.SetInt(RecoverTalentLevel, level);
            PlayerPrefs.SetInt(VeteranTalentLevel, level > 1 ? 1 : level);
            PlayerPrefs.SetInt(MagnetTalentLevel, level);
            PlayerPrefs.SetInt(RangeTalentLevel, level);
            PlayerPrefs.SetInt(BootsTalentLevel, level);
        }
        
        public bool CanUpgrade(TalentsData talentsData)
        {
            foreach (TalentData talent in talentsData.Talents)
            {
                if (GetTalentLevel(talent.Type) < talent.UpgradeAmountPerLevel.Length)
                {
                    return true;
                }
            }

            return false;
        }

        public void Clear()
        {
            PlayerPrefs.SetInt(NumberOfTalentUpgrades, 0);
            SetAllTalentsToLevel(0);
        }
    }
}