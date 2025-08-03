using Application.Data.Hazard;
using UnityEngine;

namespace Game.Hazard
{
    [RequireComponent(typeof(FireHazardManager))]
    public class FireHazardGameManager : HazardGameManager
    {
        public override void InitializeGame()
        {
            base.InitializeGame();

            if (_hazardData is FireHazardData fireHazardData)
            {
                FireHazardManager.Instance.Initialize(_sphereCenter, _sphereRadius, fireHazardData);
            }
        }
    }
}