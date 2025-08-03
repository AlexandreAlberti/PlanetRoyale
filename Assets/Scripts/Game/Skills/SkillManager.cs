using System.Collections.Generic;
using UnityEngine;

namespace Game.Skills
{
    public class SkillManager : MonoBehaviour
    {
        [SerializeField] private PlayerSkillData[] _availableSkills;

        public static SkillManager Instance { get; private set; }
        
        private void Awake()
        {
            Instance = this;
        }
        
        public List<PlayerSkillData> GetRandomSkills(int maxNumberOfSkills, List<PlayerSkillType> excludedSkills)
        {
            List<PlayerSkillData> randomSkillList = new List<PlayerSkillData>();
            int remainingSkillsToGet = _availableSkills.Length - excludedSkills.Count;
            int numberOfSkillsToShow = remainingSkillsToGet < maxNumberOfSkills ? remainingSkillsToGet : maxNumberOfSkills;

            for (int i = 0; i < numberOfSkillsToShow; i++)
            {
                int randomIndex = Random.Range(0, _availableSkills.Length);
                PlayerSkillData randomSkillData = _availableSkills[randomIndex];

                while (excludedSkills.Contains(randomSkillData.PlayerSkillType) || randomSkillList.Contains(randomSkillData))
                {
                    randomIndex = Random.Range(0, _availableSkills.Length);
                    randomSkillData = _availableSkills[randomIndex];
                }

                randomSkillList.Add(randomSkillData);
            }

            return randomSkillList;
        }

        public PlayerSkillData GetSkillDataFromType(PlayerSkillType playerSkillType)
        {
            foreach (PlayerSkillData playerSkillData in _availableSkills)
            {
                if (playerSkillData.PlayerSkillType == playerSkillType)
                {
                    return playerSkillData;
                }
            }

            return null;
        }
    }
}
