using Game.PlayerUnit;
using Game.Pool;
using UnityEngine;

namespace Game.Drop
{
    public class XpDrop : Drop
    {
        [Header("Xp Drop")]
        [SerializeField] private int _xpAmount;
        
        private void OnTriggerEnter(Collider other)
        {
            if (!_isEnabled)
            {
                return;
            }

            Player player = other.GetComponent<Player>();

            if (player)
            {
                player.GainXp(_xpAmount);
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
        
        public int TagXp()
        {
            return _xpAmount;
        }

        private void SpawnCollectedDrop(Transform playerMagnetTransform, int playerIndex)
        {
            GameObject collectedXpDropInstance = ObjectPool.Instance.GetPooledObject(_collectedDrop, transform.position, transform.rotation);
            collectedXpDropInstance.GetComponent<XpDropCollected>().Initialize(_collectedDrop, playerMagnetTransform, playerIndex, _xpAmount);
        }
    }
}