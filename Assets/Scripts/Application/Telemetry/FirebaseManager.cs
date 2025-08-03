using Firebase;
using Firebase.Crashlytics;
using Firebase.Extensions;
using UnityEngine;

// using Firebase.RemoteConfig;

namespace Application.Telemetry
{
    public class FirebaseManager : MonoBehaviour
    {
        public static FirebaseManager Instance;

        private FirebaseApp _app;
        private bool _appReady;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            // FirebaseApp.LogLevel = LogLevel.Warning;
            // FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            // {
            //     var dependencyStatus = task.Result;
            //     if (dependencyStatus == DependencyStatus.Available)
            //     {
            //         _app = FirebaseApp.DefaultInstance;
            //         _appReady = true;
            //         Crashlytics.ReportUncaughtExceptionsAsFatal = true;
            //         FirebaseApp.LogLevel = LogLevel.Warning;
            //     }
            //     else
            //     {
            //         Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
            //     }
            // });
        }

        private bool IsFirebaseReady()
        {
            return _appReady;
        }
    }
}