using System.Collections;
using UI.Button;
using UnityEngine;

namespace UI.MainMenu
{
    public class NavigationBar : MonoBehaviour
    {
        [SerializeField] private NavigationBarButton _shopButton;
        [SerializeField] private NavigationBarButton _equipmentButton;
        [SerializeField] private NavigationBarButton _raceButton;
        [SerializeField] private NavigationBarButton _talentsButton;
        [SerializeField] private NavigationBarButton _worldButton;
        [SerializeField] private EquipmentScreen _equipmentScreen;
        [SerializeField] private MainScreen _mainScreen;
        [SerializeField] private TalentsScreen _talentsScreen;
        [SerializeField] private DungeonsScreen _dungeonsScreen;

        private const float ButtonEnableTime = 0.2f;

        private InteractableButton _currentButton;
        private bool _isShowingShopTooltip;
        private bool _isShowingWorldTooltip;

        public void Initialize()
        {
            _shopButton.OnClicked += ShopButton_OnClicked;

            _equipmentButton.OnClicked += EquipmentButton_OnClicked;
            _raceButton.OnClicked += RaceButton_OnClicked;
            _talentsButton.OnClicked += TalentsButton_OnClicked;
            _worldButton.OnClicked += WorldButton_OnClicked;

            _talentsScreen.OnTalentUpgradeStarted += TalentsScreen_OnTalentUpgradeStarted;
            _talentsScreen.OnTalentUpgradeEnded += TalentsScreen_OnTalentUpgradeEnded;

            _raceButton.ForceClick();
        }

        private void TalentsScreen_OnTalentUpgradeEnded()
        {
            EnableAllButtons();
        }

        private void TalentsScreen_OnTalentUpgradeStarted()
        {
            DisableAllButtons();
        }

        private void EnableOtherButtons()
        {
            EnableAllButtons();
            
            _currentButton.Disable();
        }
        
        private void EnableAllButtons()
        {
            _equipmentButton.Enable();
            _raceButton.Enable();
            _talentsButton.Enable();
            _worldButton.Enable();
        }
        
        private void DisableAllButtons()
        {
            _equipmentButton.Disable();
            _raceButton.Disable();
            _talentsButton.Disable();
            _worldButton.Disable();
        }

        private IEnumerator WaitAndEnableButtons()
        {
            yield return new WaitForSeconds(ButtonEnableTime);
            EnableOtherButtons();
        }

        private void ShopButton_OnClicked(InteractableButton button)
        {
            if (_isShowingShopTooltip)
            {
                _isShowingShopTooltip = false;
                _shopButton.OnButtonUp();
            }
            else
            {
                _isShowingShopTooltip = true;
            }
        }
        
        private void WorldButton_OnClicked(InteractableButton button)
        {
            _currentButton = button;
            DisableEquipmentScreen();
            DisableTalentsScreen();
            DisableRaceScreen();
            _dungeonsScreen.gameObject.SetActive(true);
            HideTooltips();
            DisableAllButtons();
            StartCoroutine(WaitAndEnableButtons());
        }
        
        private void EquipmentButton_OnClicked(InteractableButton button)
        {
            _currentButton = button;
            DisableRaceScreen();
            DisableTalentsScreen();
            DisableDungeonsScreen();
            _equipmentScreen.gameObject.SetActive(true);
            HideTooltips();
            DisableAllButtons();
            StartCoroutine(WaitAndEnableButtons());
        }

        private void RaceButton_OnClicked(InteractableButton button)
        {
            _currentButton = button;
            DisableEquipmentScreen();
            DisableTalentsScreen();
            DisableDungeonsScreen();
            _mainScreen.gameObject.SetActive(true);
            HideTooltips();
            DisableAllButtons();
            StartCoroutine(WaitAndEnableButtons());
        }

        private void TalentsButton_OnClicked(InteractableButton button)
        {
            _currentButton = button;
            DisableRaceScreen();
            DisableEquipmentScreen();
            DisableDungeonsScreen();
            _talentsScreen.gameObject.SetActive(true);
            HideTooltips();
            DisableAllButtons();
            StartCoroutine(WaitAndEnableButtons());
        }

        private void HideTooltips()
        {
            HideShopTooltip();
        }

        private void HideShopTooltip()
        {
            if (_isShowingShopTooltip)
            {
                _isShowingShopTooltip = false;
                _shopButton.OnButtonUp();
            }
        }
        
        private void DisableEquipmentScreen()
        {
            _equipmentButton.OnButtonUp();
            _equipmentScreen.gameObject.SetActive(false);
        }

        private void DisableRaceScreen()
        {
            _raceButton.OnButtonUp();
            _mainScreen.gameObject.SetActive(false);
        }

        private void DisableTalentsScreen()
        {
            _talentsButton.OnButtonUp();
            _talentsScreen.gameObject.SetActive(false);
        }
        
        private void DisableDungeonsScreen()
        {
            _worldButton.OnButtonUp();
            _dungeonsScreen.gameObject.SetActive(false);
        }
    }
}