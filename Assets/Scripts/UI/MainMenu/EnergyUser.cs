using System;
using Application.Data.Progression;
using Application.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.MainMenu
{
    public class EnergyUser : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _currentEnergyLabel;
        [SerializeField] private GameObject _nextEnergyObject;
        [SerializeField] private TextMeshProUGUI _nextEnergyLabel;
        [SerializeField] private Image _fillImage;

        public float _timeToWaitTillNextUpdateInSeconds;
        private bool _energyIsFilled;

        private void Start()
        {
            EnergyDataManager.Instance.OnEnergySpent += EnergyDataManager_OnEnergySpent;
            _timeToWaitTillNextUpdateInSeconds = 0.0f;
            CheckForEnergyUpdate();
        }

        private void OnDestroy()
        {
            EnergyDataManager.Instance.OnEnergySpent -= EnergyDataManager_OnEnergySpent;
        }

        private void EnergyDataManager_OnEnergySpent(int amount)
        {
            EnergyUpdate();
        }

        private void LateUpdate()
        {
            _timeToWaitTillNextUpdateInSeconds -= Time.deltaTime;
            CheckForEnergyUpdate();
        }

        private void CheckForEnergyUpdate()
        {
            if (_timeToWaitTillNextUpdateInSeconds <= 0.0f)
            {
                EnergyUpdate();
            }

            _nextEnergyObject.SetActive(!_energyIsFilled);
            _nextEnergyLabel.text = AddLeadingZero(Mathf.FloorToInt(_timeToWaitTillNextUpdateInSeconds / 60)) + ":" +
                                    AddLeadingZero(Mathf.FloorToInt(_timeToWaitTillNextUpdateInSeconds % 60));
        }

        private void EnergyUpdate()
        {
            float currentEnergy = EnergyDataManager.Instance.GetEnergy();
            float maxEnergy = EnergyDataManager.Instance.GetMaxEnergy();
            int currentIntEnergy = Mathf.FloorToInt(currentEnergy);
            _energyIsFilled = currentEnergy >= maxEnergy;
            _currentEnergyLabel.text = currentIntEnergy + "/" + Mathf.FloorToInt(maxEnergy);
            _fillImage.fillAmount = currentEnergy / maxEnergy;

            if (!_energyIsFilled)
            {
                _timeToWaitTillNextUpdateInSeconds = TimeUtils.TimeToGetNextInSeconds(currentEnergy,
                    EnergyDataManager.Instance.GetEnergyRecoveryRatePerHour());
            }
        }

        private string AddLeadingZero(int value)
        {
            return (value < 10 ? "0" : "") + value;
        }
    }
}