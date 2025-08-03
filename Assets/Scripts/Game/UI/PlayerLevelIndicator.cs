using TMPro;
using UnityEngine;

namespace Game.UI
{
    public class PlayerLevelIndicator : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _level;
        
        public void UpdateLevel(int level)
        {
            _level.text = $"{level + 1}";
        }
    }
}
