using Application.Data.Currency;

namespace UI.Counter
{
    public class MenuGoldCounter : GoldCounter
    {
        public override void Initialize(int initialGold)
        {
            base.Initialize(initialGold);
            CurrencyDataManager.Instance.OnGoldUpdated += UpdateGoldAmount;
        }
        
        private void OnDestroy()
        {
            CurrencyDataManager.Instance.OnGoldUpdated -= UpdateGoldAmount;
        }
    }
}