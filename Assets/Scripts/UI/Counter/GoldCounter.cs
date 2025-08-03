using TMPro;
using UnityEngine;

namespace UI.Counter
{
    public class GoldCounter : BaseCounter
    {
        [SerializeField] private TextMeshProUGUI _goldAmountText;

        public virtual void Initialize(int initialGold)
        {
            _goldAmountText.text = initialGold.ToString();
        }

        protected void UpdateGoldAmount(int goldAmount)
        {
            int.TryParse(_goldAmountText.text, out int currentAmount);

            if (currentAmount > goldAmount)
            {
                SubtractAnimationParticle(goldAmount - currentAmount);
            }

            _goldAmountText.text = goldAmount.ToString();
        }
    }
}