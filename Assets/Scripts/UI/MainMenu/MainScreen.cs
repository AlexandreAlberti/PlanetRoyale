using System.Collections;
using Application.Data;
using Application.Data.Progression;
using Application.Sound;
using TMPro;
using UI.Button;
using UI.MainMenu.RedDot;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI.MainMenu
{
    public class MainScreen : MonoBehaviour
    {
        [SerializeField] private InteractableButton _playGameButton;
        [SerializeField] private TextMeshProUGUI _raceEnergyCostText;
        [SerializeField] private int _raceEnergyCost;
        [SerializeField] private Popup _energyPopup;
        [SerializeField] private LeagueScreenButton _leagueScreenButton;
        [SerializeField] private ScoutClaimAvailableManager _scoutClaimAvailableManager;
        
        private Coroutine _updateTimeCoroutine;

        private const float GameSceneLoadDuration = 0.5f;
        
        private void OnEnable()
        {
            _raceEnergyCostText.text = _raceEnergyCost.ToString();
            _playGameButton.OnClicked -= PlayGameButton_OnClicked;
            _playGameButton.OnClicked += PlayGameButton_OnClicked;
        }
        
        private void PlayGameButton_OnClicked(InteractableButton button)
        {
            if (!EnergyDataManager.Instance.SpendEnergy(_raceEnergyCost))
            {
                _energyPopup.Show();
                SoundManager.Instance.PlayMenuPopupButtonSound();
                return;
            }
            
            _playGameButton.OnClicked -= PlayGameButton_OnClicked;
            _playGameButton.Disable();
            
            SoundManager.Instance.PlayMenuPlayGameButtonSound();
            
            StartCoroutine(LoadGameScene());
        }

        private IEnumerator LoadGameScene()
        {
            ScreenFader.ScreenFader.Instance.FadeIn();
            MusicManager.Instance.FadeOutMusic();
            yield return new WaitForSeconds(GameSceneLoadDuration);
            SceneManager.LoadScene((int) SceneEnum.SearchForPlayers);
        }
    }
}