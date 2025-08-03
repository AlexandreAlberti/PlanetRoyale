using TMPro;
using UnityEngine;

namespace UI.MainMenu
{
    public class RewardCoinsHolder : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _levelText;

        private const string AmountMark = "x";

        public void Initialize(int amount)
        {
            _levelText.text = $"{AmountMark}{amount}";
        }
    }
}