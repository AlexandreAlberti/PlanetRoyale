using System.Collections;
using Application.Data;
using Application.Data.Equipment;
using Application.Sound;
using Game.Equipment;
using Game.Ftue;
using UI.Button;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI.SplashScreen
{
    public class SplashScreen : MonoBehaviour
    {
        [SerializeField] private InteractableButton _tapToStartButton;

        private const float LoadDuration = 0.5f;

        private void Start()
        {
            QualitySettings.vSyncCount = 0;
            UnityEngine.Application.targetFrameRate = 60;
            MusicManager.Instance.PlaySplashScreenMusic();
            ScreenFader.ScreenFader.Instance.FadeOut();
            FtueManager.Instance.Initialize();
            _tapToStartButton.OnClicked += TapToStartButton_OnClicked;

            if (EquipmentDataManager.Instance.GetAllEquipmentPieces().Count == 0)
            {
                EquipmentDataManager.Instance.GiveInitialEquipment();
            }
        }

        private void TapToStartButton_OnClicked(InteractableButton button)
        {
            _tapToStartButton.OnClicked -= TapToStartButton_OnClicked;
            _tapToStartButton.Disable();
            StartCoroutine(LoadNextScene());
        }

        private IEnumerator LoadNextScene()
        {
            ScreenFader.ScreenFader.Instance.FadeIn();
            ScreenFader.ScreenFader.Instance.FadeIn(!FtueManager.Instance.HasClearedControlsTutorialLevel() || !FtueManager.Instance.HasClearedMatchTutorialLevel());
            MusicManager.Instance.FadeOutMusic();
            yield return new WaitForSeconds(LoadDuration);
            SceneManager.LoadScene((int)(FtueManager.Instance.HasClearedMatchTutorialLevel() ? SceneEnum.MainMenu : SceneEnum.Game));
        }
    }
}