using Application.Data.Progression;
using UnityEngine;
using UnityEngine.UI;

namespace UI.MainMenu
{
    public class MainMenuNodeProgress : MainMenuNodeTransitionProgress
    {
        [SerializeField] private Image _questIcon;

        public override void Initialize(LevelData levelData, bool completedNode, bool currentNode)
        {
            base.Initialize(levelData, completedNode, currentNode);

            if (!levelData.IsFinalNode)
            {
                _questIcon.gameObject.SetActive(false);
            }
        }
    }
}