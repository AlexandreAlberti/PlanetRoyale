using Game.PlayerUnit;
using Game.Pool;
using UnityEngine;

namespace Game.Drop
{
    public class HpDrop : Drop
    {
        private void OnTriggerEnter(Collider other)
        {
            if (!_isEnabled)
            {
                return;
            }

            Player player = other.GetComponent<Player>();

            if (player)
            {
                player.HealPercentage(DropManager.Instance.GetHpDropHealingPercentage());
                Destroy();
                return;
            }

            PlayerMagnet playerMagnet = other.GetComponent<PlayerMagnet>();

            if (playerMagnet)
            {
                SpawnCollectedPrefab(playerMagnet.transform, playerMagnet.GetPlayerIndex());
                Destroy();
            }
        }

        private void SpawnCollectedPrefab(Transform playerMagnetTransform, int playerIndex)
        {
            GameObject collectedHpDropInstance = ObjectPool.Instance.GetPooledObject(_collectedDrop, transform.position, transform.rotation);
            collectedHpDropInstance.GetComponent<HpDropCollected>().Initialize(_collectedDrop, playerMagnetTransform, playerIndex, DropManager.Instance.GetHpDropHealingPercentage());
        }
    }
}