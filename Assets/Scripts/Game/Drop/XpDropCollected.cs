using Game.PlayerUnit;
using UnityEngine;

namespace Game.Drop
{
    public class XpDropCollected : DropCollected
    {
        private int _xpAmount;
        
        public void Initialize(GameObject prefab, Transform target, int targetIndex, int xpAmount)
        {
            base.Initialize(prefab, target, targetIndex);
            _xpAmount = xpAmount;
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
                player.GainXp(_xpAmount);
                Destroy();
            }
        }
    }
}