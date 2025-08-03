using UnityEngine;

namespace Application.Telemetry
{
    public class SingletonForTelemetry : MonoBehaviour
    {
        private static SingletonForTelemetry Instance { get; set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
        }
    }
}