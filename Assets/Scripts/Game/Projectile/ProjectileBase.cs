using System;
using Game.Enabler;
using Game.Pool;
using Game.Vfx;
using UnityEngine;

namespace Game.Projectile
{
    [RequireComponent(typeof(AutoDestroyPooledObject))]
    public abstract class ProjectileBase : EnablerMonoBehaviour
    {
        [SerializeField] private GameObject _destructionVfx;
        [SerializeField] protected ProjectileSphericalMovement _projectileSphericalMovement;
        
        private AutoDestroyPooledObject _autoDestroyPooledObject;
        private Vector3 _position;
        protected int _heroIndex;
        private GameObject _projectilePrefab;
        protected int _initialDamage;
        protected Vector3 _targetTransformPosition;
        protected Quaternion _targetTransformRotation;
        protected Vector3 _sphereCenter;
        protected float _sphereRadius;

        public Action<Vector3> OnDestroyed;

        protected virtual void Awake()
        {
            _autoDestroyPooledObject = GetComponent<AutoDestroyPooledObject>();
        }

        public virtual void Initialize(Vector3 position, Vector3 direction, float distanceToTarget, Vector3 sphereCenter, float sphereRadius, int damage, GameObject projectilePrefab, int playerIndex)
        {
            transform.position = position;
            _sphereCenter = sphereCenter;
            _sphereRadius = sphereRadius;
            _projectilePrefab = projectilePrefab;
            _projectileSphericalMovement.Initialize(direction, distanceToTarget, sphereCenter, sphereRadius);
            _autoDestroyPooledObject.Initialize(_projectilePrefab);
            _heroIndex = playerIndex;
            _initialDamage = damage;
            Enable();
        }

        public void SetTargetTransform(Vector3 targetTransformPosition, Quaternion targetTransformRotation)
        {
            _targetTransformPosition = targetTransformPosition;
            _targetTransformRotation = targetTransformRotation;
        }

        public void Destroy()
        {
            _autoDestroyPooledObject.CancelAutoDestroy();
            ObjectPool.Instance.Release(_projectilePrefab, gameObject);
            SpawnDestructionVfx();
            OnDestroyed?.Invoke(transform.position);
        }
        
        private void SpawnDestructionVfx()
        {
            if (!_destructionVfx)
            {
                return;
            }
            
            GameObject vfxInstance = ObjectPool.Instance.GetPooledObject(_destructionVfx, transform.position, Quaternion.identity);
            vfxInstance.GetComponent<AutoDestroyPooledObject>().Initialize(_destructionVfx);
        }
        
        protected abstract void OnTriggerEnter(Collider other);
    }
}