using System.Collections;
using Application.Data;
using Application.Data.Progression;
using Application.Sound;
using Application.Utils;
using TMPro;
using UI.Button;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI.MainMenu
{
    public class DungeonsScreen : MonoBehaviour
    {
        [SerializeField] private InteractableButton _goldDungeonPopupButton;
        [SerializeField] private Popup _goldDungeonPopup;
        [SerializeField] private TextMeshProUGUI _goldDungeonTicketsAmount;
        [SerializeField] private InteractableButton _goldDungeonGameButton;
        [SerializeField] private TextMeshProUGUI _goldDungeonPopupBottomDescription;
        [SerializeField] private string _goldDungeonPopupBottomDescriptionText;

        public void OnEnable()
        {
            _goldDungeonPopupButton.OnClicked -= GoldDungeonPopupButton_OnClicked;
            _goldDungeonPopupButton.OnClicked += GoldDungeonPopupButton_OnClicked;
            _goldDungeonGameButton.OnClicked -= GoldDungeonGameButton_OnClicked;
            _goldDungeonGameButton.OnClicked += GoldDungeonGameButton_OnClicked;
            int availableTickets = Mathf.FloorToInt(DungeonTicketsDataManager.Instance.GetGoldDungeonTickets());
            _goldDungeonTicketsAmount.text = $"{availableTickets}";
            _goldDungeonPopupBottomDescription.text = _goldDungeonPopupBottomDescriptionText + $" {availableTickets}";
        }

        
        private void GoldDungeonPopupButton_OnClicked(InteractableButton button)
        {
            _goldDungeonPopup.Show();
        }

        private void GoldDungeonGameButton_OnClicked(InteractableButton button)
        {
            if (!DungeonTicketsDataManager.Instance.SpendGoldDungeonTickets(NumberConstants.One))
            {
                SoundManager.Instance.PlayMenuPopupButtonSound();
                return;
            }
            
            _goldDungeonGameButton.OnClicked -= GoldDungeonGameButton_OnClicked;
            GameDataManager.Instance.SaveIsAGoldDungeonGame(true);
            
            _goldDungeonGameButton.Disable();
            
            SoundManager.Instance.PlayMenuPlayGameButtonSound();
            
            StartCoroutine(LoadGameScene());
        }
        
        private IEnumerator LoadGameScene()
        {
            ScreenFader.ScreenFader.Instance.FadeIn();
            MusicManager.Instance.FadeOutMusic();
            yield return new WaitForSeconds(NumberConstants.Half);
            SceneManager.LoadScene((int) SceneEnum.Game);
        }
    }
}
