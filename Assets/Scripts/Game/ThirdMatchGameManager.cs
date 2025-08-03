using System.Linq;
using Game.PlayerUnit;
using Game.Skills;
using UnityEngine;

namespace Game
{
    public class ThirdMatchGameManager : GameManager
    {
        [SerializeField] private PlayerSkillData[] _playerSkillData;
        public override void InitializeGame()
        {
            base.InitializeGame();
            Debug.Log($"Third Match");
            PlayerManager.Instance.GetHumanPlayer().SetNextSkillsToChoose(_playerSkillData.ToList());
        }
    }
}