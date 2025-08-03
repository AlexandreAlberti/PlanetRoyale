using System.Collections;
using Application.Data;
using Application.Sound;
using Game.Camera;
using Game.Ftue;
using Game.PlayerUnit;
using Game.UI;
using UI.ScreenFader;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public class MatchTutorialGameManager : GameManager
    {
        public override void InitializeGame()
        {
            base.InitializeGame();

            UIManager.Instance.InitializeForMatchTutorial();
            
            FtueManager.Instance.Initialize();
            FtueManager.Instance.ShowNextStep();
            
            StartCoroutine(InitialZoomOut());
        }
        
        protected override IEnumerator LoadNextScene()
        {
            ScreenFader.Instance.FadeIn(true);
            MusicManager.Instance.FadeOutMusic();
            yield return new WaitForSeconds(LoadDuration);
            SceneManager.LoadScene((int) SceneEnum.MainMenu);
        }
        
        protected override void ShowStartAnimation()
        {
            UIManager.Instance.OnStartGameAnimationEnded += UIManager_OnStartGameAnimationEnded;
        }
    }
}