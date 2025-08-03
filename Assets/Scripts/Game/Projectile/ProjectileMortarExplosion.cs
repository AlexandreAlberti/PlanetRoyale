using Game.Enabler;
using UnityEngine;

namespace Game.Projectile
{
    [RequireComponent(typeof(SphereCollider))]
    public abstract class ProjectileMortarExplosion : EnablerMonoBehaviour
    {
        private SphereCollider _collider;
        protected int _playerIndex;
        protected int _damage;

        private void Awake()
        {
            _collider = GetComponent<SphereCollider>();
        }

        private void OnEnable()
        {
            _collider.enabled = false;
            Disable();
        }

        public virtual void Initialize(int playerIndex, int damage)
        {
            _playerIndex = playerIndex;
            _damage = damage;
            _collider.enabled = true;
            Enable();
        }
    }
}