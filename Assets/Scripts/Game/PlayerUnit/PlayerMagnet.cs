using UnityEngine;

namespace Game.PlayerUnit
{
    public class PlayerMagnet : MonoBehaviour
    {
        [SerializeField] private SphereCollider _sphereCollider;
        
        private int _playerIndex;

        public void Initialize(int playerIndex, float radius)
        {
            _playerIndex = playerIndex;
            _sphereCollider.radius = radius;
        }
        
        public int GetPlayerIndex()
        {
            return _playerIndex;
        }

        public void DisableCollider()
        {
            _sphereCollider.enabled = false;
        }

        public void EnableCollider()
        {
            _sphereCollider.enabled = true;
        }
    }
}
