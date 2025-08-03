using Application.Data.Rewards;
using UnityEngine;

namespace UI.MainMenu.RedDot
{
    public class ScoutClaimAvailableManager : MonoBehaviour
    {
        [SerializeField] private GameObject _indicator;
        
        private void Start()
        {
            ScoutDataManager.Instance.OnScoutClaimed += ScoutDataManager_OnScoutClaimed;
            ScoutDataManager.Instance.OnScoutClaimAvailable += ScoutDataManager_OnScoutClaimAvailable;
            
            RefreshIndicator();
        }

        private void ScoutDataManager_OnScoutClaimed()
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
            ScoutDataManager.Instance.OnScoutClaimAvailable -= ScoutDataManager_OnScoutClaimAvailable;
        }

        private void RefreshIndicator()
        {
            _indicator.SetActive(ScoutDataManager.Instance.CanClaim());
        }
    }
}
