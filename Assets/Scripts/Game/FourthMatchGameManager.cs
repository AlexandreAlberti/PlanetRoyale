using Game.Hazard;
using UnityEngine;

namespace Game
{
    public class FourthMatchGameManager : FireHazardGameManager
    {
        public override void InitializeGame()
        {
            base.InitializeGame();
            Debug.Log($"Fourth Match");
        }
    }
}