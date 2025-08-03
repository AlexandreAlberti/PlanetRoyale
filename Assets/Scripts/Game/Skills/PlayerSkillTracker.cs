using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Skills
{
    public class PlayerSkillTracker : MonoBehaviour
    {
        private Dictionary<PlayerSkillType, int> _skillsProgression;
        private List<PlayerSkillType> _maxedOutSkills;
        private List<PlayerSkillData> _skillDataList;

        public Action<PlayerSkillData> OnSkillGained;
        
        public void Initialize()
        {
            _maxedOutSkills = new List<PlayerSkillType>();
            _skillDataList = new List<PlayerSkillData>();
            _skillsProgression = new Dictionary<PlayerSkillType, int>
            {
                { PlayerSkillType.Attack, 0 },
                { PlayerSkillType.AttackRange, 0 },
                { PlayerSkillType.AttackSpeed, 0 },
                { PlayerSkillType.DamageReduction, 0 },
                { PlayerSkillType.DodgeChance, 0 },
                { PlayerSkillType.Health, 0 },
                { PlayerSkillType.MovementSpeed, 0 },
                { PlayerSkillType.XpGained, 0 }
            };
        }

        public void GainSkill(PlayerSkillData skillData)
        {
            _skillsProgression[skillData.PlayerSkillType]++;

            if (_skillsProgression[skillData.PlayerSkillType] >= skillData.MaxStackAmount)
            {
                _maxedOutSkills.Add(skillData.PlayerSkillType);
            }

            _skillDataList.Add(skillData);

            OnSkillGained?.Invoke(skillData);
        }

        public List<PlayerSkillType> MaxedOutSkills()
        {
            return _maxedOutSkills;
        }

        public List<PlayerSkillData> GainedSkills()
        {
            return _skillDataList;
        }

        public int GetSkillLevel(PlayerSkillType skillType)
        {
            return _skillsProgression[skillType];
        }
    }
}