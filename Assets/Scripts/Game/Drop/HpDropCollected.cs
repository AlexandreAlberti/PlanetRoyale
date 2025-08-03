using Game.PlayerUnit;
using UnityEngine;

namespace Game.Drop
{
    public class HpDropCollected : DropCollected
    {
        private float _healingPercentage;

        public void Initialize(GameObject prefab, Transform target, int targetIndex, float healingPercentage)
        {
            base.Initialize(prefab, target, targetIndex);
            _healingPercentage = healingPercentage;
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
                player.HealPercentage(_healingPercentage);
                Destroy();
            }
        }
    }
}