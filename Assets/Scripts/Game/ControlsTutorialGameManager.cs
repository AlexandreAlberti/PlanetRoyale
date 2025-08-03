using System.Collections;
using System.Linq;
using Application.Data;
using Application.Sound;
using Game.Camera;
using Game.EnemyUnit;
using Game.Ftue;
using Game.PlayerUnit;
using Game.Skills;
using Game.UI;
using UI.ScreenFader;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public class ControlsTutorialGameManager : GameManager
    {
        [SerializeField] private PlayerSkillData[] _tutorialPreselectedSkills;

        protected override IEnumerator InitialZoomOut()
        {
            PlayerManager.Instance.GetHumanPlayer().SetFactorOfPlayerRadiusToSee(1.6f);
            CameraManager.Instance.RecalculateMagnitudeConstantMultiplier();
            PlayerManager.Instance.GetHumanPlayer().SetAttackRange(3.0f);
            
            return base.InitialZoomOut();
        }

        public override void InitializeGame()
        {
            base.InitializeGame();
            
            UIManager.Instance.HideCountDownPanel();
            UIManager.Instance.HideRanksPanels();
            UIManager.Instance.HideOptionsButton();
            
            FtueManager.Instance.Initialize();
            FtueManager.Instance.ShowNextStep();

            ControlsTutorialEnemySpawner.Instance.Initialize();
            PlayerManager.Instance.GetHumanPlayer().SetNextSkillsToChoose(_tutorialPreselectedSkills.ToList());
            PlayerManager.Instance.GetHumanPlayer().MagnetColliderDisable();
            StopAllCoroutines();
        }
        
        protected override IEnumerator LoadNextScene()
        {
            ScreenFader.Instance.FadeIn(true);
            MusicManager.Instance.FadeOutMusic();
            yield return new WaitForSeconds(LoadDuration);
            SceneManager.LoadScene((int) SceneEnum.Game);
        }

        protected override void ShowStartAnimation()
        {
        }
    }
}