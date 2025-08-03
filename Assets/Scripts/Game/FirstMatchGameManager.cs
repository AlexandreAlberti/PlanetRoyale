using UnityEngine;

namespace Game
{
    public class FirstMatchGameManager : GameManager
    {
        public override void InitializeGame()
        {
            base.InitializeGame();
            Debug.Log($"First Match");
        }
    }
}