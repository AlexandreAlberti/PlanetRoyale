using System;
using System.Collections.Generic;
using System.Linq;
using Application.Data.Progression;
using Application.Data.Rewards;
using Application.Utils;
using Unity.Notifications.Android;
using UnityEngine;
using UnityEngine.Android;

namespace Application.Notifications
{
    public class NotificationManager : MonoBehaviour
    {
        [SerializeField] private int _notificationCooldownInSeconds;
        [SerializeField] private int _backgroundCooldownInSeconds;
        [SerializeField] private bool _useTestNotifications;
        [SerializeField] private string _notificationMissingYouTitle;
        [SerializeField] private string _notificationMissingYouContent;
        [SerializeField] private string _notificationEnergyFullTitle;
        [SerializeField] private string _notificationEnergyFullContent;
        [SerializeField] private string _notificationScoutClaimAvailableTitle;
        [SerializeField] private string _notificationScoutClaimAvailableContent;
        [SerializeField] private string _notificationScoutClaimMaxedOutTitle;
        [SerializeField] private string _notificationScoutClaimMaxedOutContent;
        
        private const string AndroidPermissionPostNotifications = "android.permission.POST_NOTIFICATIONS";
        private const string NotificationsChannelId = "notifications";
        private const string NotificationsChannelName = "Notifications";
        private const string NotificationsChannelDescription = "Game Notifications";
        
        private Dictionary<int, NotificationConfig> _notificationDictionary;
        
        private static NotificationManager Instance { get; set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(this);
        }

        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            _notificationDictionary = new Dictionary<int, NotificationConfig>();
            
            AndroidNotificationCenter.CancelAllNotifications();

            AndroidNotificationChannel channel = new AndroidNotificationChannel
            {
                Id = NotificationsChannelId,
                Name = NotificationsChannelName,
                Importance = Importance.Default,
                Description = NotificationsChannelDescription,
            };

            AndroidNotificationCenter.RegisterNotificationChannel(channel);

            if (!Permission.HasUserAuthorizedPermission(AndroidPermissionPostNotifications))
            {
                Permission.RequestUserPermission(AndroidPermissionPostNotifications);
            }
        }

        private void OnApplicationQuit()
        {
            RegisterNotifications();
            SendNotifications();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                RegisterNotifications();
                SendNotifications();
            }
            else
            {
                AndroidNotificationCenter.CancelAllNotifications();
            }
        }

        private void RegisterNotifications()
        {
            RegisterMissingYouNotifications();
            RegisterEnergyMaxedOutNotification();
            RegisterScoutRewardsAvailableNotification();
            RegisterScoutRewardsMaxedOutNotification();
        }

        private void RegisterMissingYouNotifications()
        {
            RegisterMissingYouNotification(DateTime.Now.AddHours(2));
            RegisterMissingYouNotification(DateTime.Now.AddHours(8));
            RegisterMissingYouNotification(DateTime.Now.AddHours(16));
            RegisterMissingYouNotification(DateTime.Now.AddHours(48));
        }

        private void RegisterMissingYouNotification(DateTime dateTime)
        {
            RegisterNotification(new NotificationConfig(_notificationMissingYouTitle, _notificationMissingYouContent, dateTime));
        }

        private void RegisterEnergyMaxedOutNotification()
        {
            float currentEnergy = EnergyDataManager.Instance.GetEnergy();
            float maxEnergy = EnergyDataManager.Instance.GetMaxEnergy();

            if (currentEnergy >= maxEnergy)
            {
                return;
            }

            float energyRecoveryRatePerHour = EnergyDataManager.Instance.GetEnergyRecoveryRatePerHour();
            float timeToGetNext = TimeUtils.TimeToGetNextInHours(currentEnergy, energyRecoveryRatePerHour);
            float timeToGetAll = energyRecoveryRatePerHour * Mathf.FloorToInt(maxEnergy - currentEnergy);
            float totalTimeToRefillInHours = timeToGetNext + timeToGetAll;
            
            RegisterNotification(new NotificationConfig(_notificationEnergyFullTitle, _notificationEnergyFullContent, DateTime.Now.AddHours(totalTimeToRefillInHours)));
        }

        private void RegisterScoutRewardsAvailableNotification()
        {
            if (!ScoutDataManager.Instance.CanClaim())
            {
                RegisterNotification(new NotificationConfig(_notificationScoutClaimAvailableTitle, _notificationScoutClaimAvailableContent, DateTime.Now.AddSeconds(ScoutDataManager.Instance.GetClaimCooldownTime() - ScoutDataManager.Instance.GetScoutTime())));
            }
        }

        private void RegisterScoutRewardsMaxedOutNotification()
        {
            if (!ScoutDataManager.Instance.IsAtMaxScoutTime())
            {
                RegisterNotification(new NotificationConfig(_notificationScoutClaimMaxedOutTitle, _notificationScoutClaimMaxedOutContent, DateTime.Now.AddSeconds(ScoutDataManager.Instance.GetMaxScoutTime() - ScoutDataManager.Instance.GetScoutTime())));
            }
        }

        private void RegisterNotification(NotificationConfig notificationConfig)
        {
            int remainingSecondsToFire = (int)(notificationConfig.FireTime - DateTime.Now).TotalSeconds;
            _notificationDictionary.TryAdd(remainingSecondsToFire, notificationConfig);
        }

        private void SendTestNotifications()
        {
            SendNotification(_notificationScoutClaimAvailableTitle, _notificationScoutClaimAvailableContent, DateTime.Now.AddSeconds(30));
            SendNotification(_notificationScoutClaimMaxedOutTitle, _notificationScoutClaimMaxedOutContent, DateTime.Now.AddMinutes(1));
        }

        private void SendNotifications()
        {
            if (_useTestNotifications)
            {
                SendTestNotifications();
                return;
            }

            _notificationDictionary = new Dictionary<int, NotificationConfig>(_notificationDictionary.OrderBy(item => item.Key));
            int previousRemainingSeconds = 0;
            
            foreach (KeyValuePair<int, NotificationConfig> item in _notificationDictionary)
            {
                if (item.Key < _backgroundCooldownInSeconds)
                {
                    continue;
                }
                
                if (item.Key - previousRemainingSeconds < _notificationCooldownInSeconds)
                {
                    continue;
                }
                
                previousRemainingSeconds = item.Key;
                SendNotification(item.Value.Title, item.Value.Content, item.Value.FireTime);
            }
        }

        private void SendNotification(string title, string content, DateTime fireTime)
        {
            AndroidNotificationCenter.SendNotification(
                new AndroidNotification
                {
                    Title = title,
                    Text = content,
                    FireTime = fireTime
                },
                NotificationsChannelId);
        }
    }
}