using Application.Data.Rewards;
using UnityEngine;

namespace UI.MainMenu.RedDot
{
    public class QuickScoutClaimAvailableManager : MonoBehaviour
    {
        [SerializeField] private GameObject _indicator;
        
        private void Start()
        {
            ScoutDataManager.Instance.OnQuickScoutClaimed += ScoutDataManager_OnQuickScoutClaimed;
            ScoutDataManager.Instance.OnQuickScoutClaimAvailable += ScoutDataManager_OnQuickScoutClaimAvailable;
            
            RefreshIndicator();
        }
        
        private void ScoutDataManager_OnQuickScoutClaimed()
        {
            RefreshIndicator();
        }

        private void ScoutDataManager_OnQuickScoutClaimAvailable()
        {
            RefreshIndicator();
        }

        private void OnDestroy()
        {
            ScoutDataManager.Instance.OnQuickScoutClaimed -= ScoutDataManager_OnQuickScoutClaimed;
            ScoutDataManager.Instance.OnQuickScoutClaimAvailable -= ScoutDataManager_OnQuickScoutClaimAvailable;
        }

        private void RefreshIndicator()
        {
            _indicator.SetActive(ScoutDataManager.Instance.CanQuickClaim());
        }
    }
}
