using Game.Damage;
using Game.Pool;
using Game.Vfx;
using UnityEngine;

namespace Game.Projectile
{
    public class ProjectileMortarEnemy : ProjectileMortar
    {
        protected override void OnTriggerEnter(Collider other)
        {
        }

        protected override void SpawnExplosionDamageAndAreaOfEffect()
        {
            if (_explosionDamagePrefab)
            {
                ProjectileEnemyMortarExplosion explosion = ObjectPool.Instance.GetPooledObject(_explosionDamagePrefab.gameObject, transform.position, transform.rotation).GetComponent<ProjectileEnemyMortarExplosion>();
                explosion.Initialize(_heroIndex, Mathf.FloorToInt(_initialDamage));
                explosion.GetComponent<AutoDestroyPooledObject>().Initialize(_explosionDamagePrefab.gameObject);
            }

            if (_damageAreaPrefab)
            {
                Vector3 floorPosition = (transform.position - _sphereCenter).normalized * _sphereRadius;
                PlayerDamageArea area = ObjectPool.Instance.GetPooledObject(_damageAreaPrefab.gameObject, floorPosition, transform.rotation).GetComponent<PlayerDamageArea>();
                area.Initialize(Mathf.FloorToInt(_initialDamage * _areaOfEffectDamageFactor));
                area.GetComponent<AutoDestroyPooledObject>().Initialize(_damageAreaPrefab.gameObject);
            }
        }
    }
}