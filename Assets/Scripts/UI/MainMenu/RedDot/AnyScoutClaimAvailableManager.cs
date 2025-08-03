using Application.Data.Rewards;
using UnityEngine;

namespace UI.MainMenu.RedDot
{
    public class AnyScoutClaimAvailableManager : MonoBehaviour
    {
        [SerializeField] private GameObject _indicator;
        
        private void Start()
        {
            ScoutDataManager.Instance.OnScoutClaimed += ScoutDataManager_OnScoutClaimed;
            ScoutDataManager.Instance.OnQuickScoutClaimed += ScoutDataManager_OnQuickScoutClaimed;
            ScoutDataManager.Instance.OnScoutClaimAvailable += ScoutDataManager_OnScoutClaimAvailable;
            ScoutDataManager.Instance.OnQuickScoutClaimAvailable += ScoutDataManager_OnQuickScoutClaimAvailable;
            
            RefreshIndicator();
        }

        private void ScoutDataManager_OnScoutClaimed()
        {
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

        private void ScoutDataManager_OnScoutClaimAvailable()
        {
            RefreshIndicator();
        }

        private void OnDestroy()
        {
            ScoutDataManager.Instance.OnScoutClaimed -= ScoutDataManager_OnScoutClaimed;
            ScoutDataManager.Instance.OnQuickScoutClaimed -= ScoutDataManager_OnQuickScoutClaimed;
            ScoutDataManager.Instance.OnScoutClaimAvailable -= ScoutDataManager_OnScoutClaimAvailable;
            ScoutDataManager.Instance.OnQuickScoutClaimAvailable -= ScoutDataManager_OnQuickScoutClaimAvailable;
        }

        private void RefreshIndicator()
        {
            _indicator.SetActive(ScoutDataManager.Instance.CanClaim() || ScoutDataManager.Instance.CanQuickClaim());
        }
    }
}
