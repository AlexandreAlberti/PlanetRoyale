using System;
using Application.Utils;
using UnityEngine;

namespace Application.Data.Progression
{
    public class EnergyDataManager : MonoBehaviour
    {
        private const string Energy = "Energy";
        private const string EnergyLastCheck = "EnergyLastCheck";

        [SerializeField] private float _startingEnergy;
        [SerializeField] private float _maxEnergy;
        [SerializeField] private float _energyRecoverPerHour;

        public static EnergyDataManager Instance { get; set; }

        public Action<int> OnEnergySpent { get; set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
        }

        public float GetEnergy()
        {
            CheckEnergySystemIsStarted();
            float energy = Mathf.Max(0.0f, PlayerPrefs.GetFloat(Energy));
            float maxEnergy = GetMaxEnergy();

            if (energy > maxEnergy)
            {
                return energy;
            }
            
            float recoveryPerHour = GetEnergyRecoveryRatePerHour();
            long now = DateTimeOffset.Now.ToUnixTimeSeconds();
            long then = Convert.ToInt64(PlayerPrefs.GetString(EnergyLastCheck));
            long timePassedInSeconds = now - then;
            float energyRecovered = timePassedInSeconds * recoveryPerHour / TimeUtils.SecondsInHour;
            energy = MathF.Min(energy + energyRecovered, maxEnergy);
            PlayerPrefs.SetFloat(Energy, energy);
            PlayerPrefs.SetString(EnergyLastCheck, now.ToString());
            return energy;
        }

        public float GetMaxEnergy()
        {
            return _maxEnergy;
        }

        public float GetEnergyRecoveryRatePerHour()
        {
            return _energyRecoverPerHour;
        }

        public bool SpendEnergy(float amount)
        {
            float energy = GetEnergy() - amount;
            bool enoughEnergy = energy >= 0.0f;

            if (enoughEnergy)
            {
                PlayerPrefs.SetFloat(Energy, energy);
                OnEnergySpent?.Invoke(Mathf.FloorToInt(amount));
            }

            return enoughEnergy;
        }

        public void AddEnergy(float amount)
        {
            float energy = GetEnergy() + amount;

            if (energy > _maxEnergy)
            {
                // As we go over max because external stuff, we don't keep restoring progress
                energy = Mathf.FloorToInt(energy);
            }

            PlayerPrefs.SetFloat(Energy, energy);
        }

        private void CheckEnergySystemIsStarted()
        {
            if (!PlayerPrefs.HasKey(EnergyLastCheck) || 1 > Convert.ToInt64(PlayerPrefs.GetString(EnergyLastCheck)))
            {
                PlayerPrefs.SetFloat(Energy, _startingEnergy);
                PlayerPrefs.SetString(EnergyLastCheck, DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
            }
        }

        public void RecoverFullEnergy()
        {
            PlayerPrefs.SetFloat(Energy, _maxEnergy);
            PlayerPrefs.SetString(EnergyLastCheck, DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
        }
        
        public void EmptyEnergy()
        {
            PlayerPrefs.SetFloat(Energy, 0);
            PlayerPrefs.SetString(EnergyLastCheck, DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
        }

        public void Clear()
        {
            PlayerPrefs.SetFloat(Energy, _startingEnergy);
            PlayerPrefs.SetString(EnergyLastCheck, DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
        }
    }
}