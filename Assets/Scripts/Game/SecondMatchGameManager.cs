using Game.Hazard;
using UnityEngine;

namespace Game
{
    public class SecondMatchGameManager : FireHazardGameManager
    {
        public override void InitializeGame()
        {
            base.InitializeGame();
            Debug.Log($"Second Match");
        }
    }
}