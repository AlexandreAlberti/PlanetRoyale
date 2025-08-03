using Game.PlayerUnit;
using Game.Pool;
using UnityEngine;

namespace Game.Drop
{
    public class GoldDrop : Drop
    {
        private int _goldAmount;
        
        public void Initialize(GameObject dropPrefab, Vector3 upDirection, int amount)
        {
            _goldAmount = amount;
            base.Initialize(dropPrefab, upDirection);
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (!_isEnabled)
            {
                return;
            }

            Player player = other.GetComponent<Player>();

            if (player)
            {
                player.GainGold(_goldAmount);
                Destroy();
                return;
            }

            PlayerMagnet playerMagnet = other.GetComponent<PlayerMagnet>();

            if (playerMagnet)
            {
                SpawnCollectedDrop(playerMagnet.transform, playerMagnet.GetPlayerIndex());
                Destroy();
            }
        }
        
        private void SpawnCollectedDrop(Transform playerMagnetTransform, int playerIndex)
        {
            GameObject collectedGoldDropInstance = ObjectPool.Instance.GetPooledObject(_collectedDrop, transform.position, transform.rotation);
            collectedGoldDropInstance.GetComponent<GoldDropCollected>().Initialize(_collectedDrop, playerMagnetTransform, playerIndex, _goldAmount);
        }
    }
}