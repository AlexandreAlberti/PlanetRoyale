using Application.Data.Progression;
using UnityEngine;
using UnityEngine.UI;

namespace UI.MainMenu
{
    public class MainMenuNodeTransitionProgress : MonoBehaviour
    {
        [SerializeField] protected Image _background;
        [SerializeField] protected Color _completedNodeColor;
        [SerializeField] protected Color _currentNodeColor;
        [SerializeField] protected Color _nextNodeColor;

        public virtual void Initialize(LevelData levelData, bool completedNode, bool currentNode)
        {
            if (completedNode)
            {
                _background.color = _completedNodeColor;
            }
            else if (currentNode)
            {
                _background.color = _currentNodeColor;
            }
            else
            {
                _background.color = _nextNodeColor;
            }
        }
    }
}