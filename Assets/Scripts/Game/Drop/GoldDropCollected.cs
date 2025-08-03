using Game.PlayerUnit;
using UnityEngine;

namespace Game.Drop
{
    public class GoldDropCollected : DropCollected
    {
        private int _goldAmount;
        
        public void Initialize(GameObject prefab, Transform target, int targetIndex, int goldAmount)
        {
            base.Initialize(prefab, target, targetIndex);
            _goldAmount = goldAmount;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_isEnabled)
            {
                return;
            }

            Player player = other.GetComponent<Player>();

            if (player && _targetIndex == player.GetPlayerIndex())
            {
                player.GainGold(_goldAmount);
                Destroy();
            }
        }
    }
}