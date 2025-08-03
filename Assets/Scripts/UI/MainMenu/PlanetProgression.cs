using Application.Data.Progression;
using Game.Ftue;
using UnityEngine;

namespace UI.MainMenu
{
    public class PlanetProgression : MonoBehaviour
    {
        [SerializeField] private GameObject _planet2GameObject;
        [SerializeField] private GameObject _lockedPlanet2GameObject;
        [SerializeField] private GameObject _planet3GameObject;
        [SerializeField] private GameObject _lockedPlanet3GameObject;
        
        private LeagueData _leagueData;

        public void Initialize()
        {
            bool hasFinishedFirstRealGame = FtueManager.Instance.HasFinishedFirstRealGame();
            _planet2GameObject.SetActive(hasFinishedFirstRealGame);
            _lockedPlanet2GameObject.SetActive(!hasFinishedFirstRealGame);
            
            bool hasFinishedFourthRealGame = FtueManager.Instance.HasFinishedFourthRealGame();
            _planet3GameObject.SetActive(hasFinishedFourthRealGame);
            _lockedPlanet3GameObject.SetActive(!hasFinishedFourthRealGame);
        }
    }
}
