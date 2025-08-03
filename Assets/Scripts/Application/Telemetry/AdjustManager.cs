using AdjustSdk;
using Application.Data.Progression;
using UnityEngine;

namespace Application.Telemetry
{
    public class AdjustManager : MonoBehaviour
    {
        public static AdjustManager Instance { get; private set; }

        private const string RegisterStartEventToken = "rhug9j";
        private const string GameStartEventToken = "s7gyy1";
        // private const string LevelStartEventToken = "vj888q";
        
        // private const string UserIdProperty = "external_device_id_md5";
        
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
             
            Instance = this;
        }

        private void Start()
        {
            // AdjustConfig adjustConfig = GetComponent<Adjust>().AdjustConfig();
            // adjustConfig.ExternalDeviceId = ProgressionDataManager.Instance.GetUserId();
            // Adjust.InitSdk(adjustConfig);
            
            // TrackGameStart();
        }

        private void TrackGameStart()
        {
            if (!ProgressionDataManager.Instance.IsRegisterDoneAdjust())
            {
                // SendEventWithParameters(RegisterStartEventToken);
                ProgressionDataManager.Instance.SetRegisterDoneAdjust();
            }

            // SendEventWithParameters(GameStartEventToken);
        }

        // public void TrackLevelStart()
        // {
        //     AdjustEvent adjustEvent = new AdjustEvent(LevelStartEventToken);
        //     Adjust.trackEvent(adjustEvent);
        // }
        
        private void SendEventWithParameters(string adjustEventName)
        {
            // AdjustEvent adjustEvent  = new AdjustEvent(adjustEventName);
            // adjustEvent.addPartnerParameter(UserIdProperty, ProgressionDataManager.Instance.GetUserId());
            // Adjust.TrackEvent(adjustEvent);
        }
    }
}