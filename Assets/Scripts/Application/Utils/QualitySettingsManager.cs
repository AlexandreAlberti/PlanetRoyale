using UnityEngine;

namespace Application.Utils
{
    public class QualitySettingsManager : MonoBehaviour
    {
        private static QualitySettingsManager Instance;

        private const int PerformantQualityLevel = 0;
        // private const int BalancedQualityLevel = 1;
        // private const int HighFidelityQualityLevel = 2;
        // private const int HighFidelityQualityLevelMinMemorySize = 8000;
        // private const int BalancedQualityLevelMinMemorySize = 4000;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                AdaptQualityLevelBasedOnDevice();
                DontDestroyOnLoad(this);
            }
        }

        private void AdaptQualityLevelBasedOnDevice()
        {
            // int memorySize = SystemInfo.systemMemorySize;
            //
            // if (memorySize >= HighFidelityQualityLevelMinMemorySize)
            // {
            //     Debug.Log("Setting to High Fidelity Quality Level");
            //     QualitySettings.SetQualityLevel(HighFidelityQualityLevel, true);
            // }
            // else if (memorySize >= BalancedQualityLevelMinMemorySize)
            // {
            //     Debug.Log("Setting to Balanced Quality Level");
            //     QualitySettings.SetQualityLevel(BalancedQualityLevel, true);
            // }
            // else
            // {

            if (QualitySettings.GetQualityLevel() == PerformantQualityLevel)
            {
                return;
            }

            QualitySettings.SetQualityLevel(PerformantQualityLevel, true);
            // }
        }
    }
}