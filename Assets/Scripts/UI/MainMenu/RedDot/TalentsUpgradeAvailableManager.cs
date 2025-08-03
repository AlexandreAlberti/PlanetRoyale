using Application.Data.Currency;
using Game.Talent;
using UnityEngine;

namespace UI.MainMenu.RedDot
{
    public class TalentsUpgradeAvailableManager : MonoBehaviour
    {
        [SerializeField] private GameObject _indicator;

        private void Start()
        {
            CurrencyDataManager.Instance.OnGoldUpdated += OnGoldUpdated;
            RefreshIndicator();
        }

        private void OnDisable()
        {
            CurrencyDataManager.Instance.OnGoldUpdated -= OnGoldUpdated;
        }

        private void OnGoldUpdated(int goldAmount)
        {
            RefreshIndicator();
        }

        private void RefreshIndicator()
        {
            int costForNextUpgrade = TalentManager.Instance.CostForNextUpgrade();
            bool canAffordUpgrade = CurrencyDataManager.Instance.CurrentGold() >= costForNextUpgrade;
            bool stillUpgradesToMake = TalentManager.Instance.CanUpgrade();
            _indicator.SetActive(canAffordUpgrade && stillUpgradesToMake);
        }
    }
}