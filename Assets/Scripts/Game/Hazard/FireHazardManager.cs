using Application.Data.Hazard;
using Application.Utils;
using Game.Enabler;
using Game.Pool;
using Game.Projectile;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Game.Hazard
{
    public class FireHazardManager : EnablerMonoBehaviour
    {
        public static FireHazardManager Instance { get; private set; }
        
        private FireHazardData _fireHazardData;
        private float _spawnTimer;
        private Vector3 _sphereCenter;
        private float _sphereRadius;

        private void Awake()
        {
            Instance = this;
        }

        public void Initialize(Vector3 sphereCenter, float sphereRadius, FireHazardData fireHazardData)
        {
            _sphereCenter = sphereCenter;
            _sphereRadius = sphereRadius;
            _fireHazardData = fireHazardData;
            ObjectPool.Instance.RegisterPrefab(_fireHazardData.MeteoritePrefab, _fireHazardData.MeteoriteSpawnAmount);
            ObjectPool.Instance.RegisterPrefab(_fireHazardData.MeteoriteDestroyVfxPrefab, _fireHazardData.MeteoriteSpawnAmount);
            ObjectPool.Instance.RegisterPrefab(_fireHazardData.MeteoriteExplosionPrefab, _fireHazardData.MeteoriteSpawnAmount);
            ObjectPool.Instance.RegisterPrefab(_fireHazardData.MeteoriteAreaOfEffectPrefab, _fireHazardData.MeteoriteSpawnAmount);
            _spawnTimer = 0;
            Enable();
        }
        
        private void Update()
        {
            if (!_isEnabled)
            {
                return;
            }

            _spawnTimer += Time.deltaTime;

            if (_spawnTimer >= _fireHazardData.MeteoriteSpawnCooldown)
            {
                _spawnTimer -= _fireHazardData.MeteoriteSpawnCooldown;
                SpawnMeteorites();
            }
        }

        private void SpawnMeteorites()
        {
            for (int i = 0; i < _fireHazardData.MeteoriteSpawnAmount; i++)
            {
                Vector3 upDirection = (_sphereCenter + Random.insideUnitSphere).normalized;
                Vector3 spawnPosition = upDirection * _sphereRadius;
                Vector3 forwardDirection = Vector3.Cross(upDirection, Vector3.forward);;
                GameObject projectileInstance = ObjectPool.Instance.GetPooledObject(_fireHazardData.MeteoritePrefab.gameObject, spawnPosition, Quaternion.identity);
                ProjectileBase projectileBase = projectileInstance.GetComponent<ProjectileBase>();
                projectileBase.SetTargetTransform(spawnPosition, Quaternion.LookRotation(forwardDirection, upDirection));
                projectileBase.Initialize(spawnPosition, Vector3.zero, NumberConstants.Zero, _sphereCenter, _sphereRadius, 
                    _fireHazardData.MeteoriteAreaOfEffectBaseDamage, _fireHazardData.MeteoritePrefab.gameObject, -1);
                Meteorite meteorite = projectileInstance.GetComponent<Meteorite>();
                meteorite.Initialize(_fireHazardData.MeteoriteExplosionBaseDamage, _fireHazardData.MeteoriteAreaOfEffectBaseDamage, _fireHazardData.MeteoriteAreaOfEffectDuration);
            }
        }
    }
}