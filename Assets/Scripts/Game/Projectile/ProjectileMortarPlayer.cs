using Game.Damage;
using Game.Pool;
using Game.Vfx;
using UnityEngine;

namespace Game.Projectile
{
    public class ProjectileMortarPlayer : ProjectileMortar
    {
        protected override void OnTriggerEnter(Collider other)
        {
        }
        
        protected override void SpawnExplosionDamageAndAreaOfEffect()
        {
            if (_explosionDamagePrefab)
            {
                ProjectilePlayerMortarExplosion explosion = ObjectPool.Instance.GetPooledObject(_explosionDamagePrefab.gameObject, transform.position, transform.rotation).GetComponent<ProjectilePlayerMortarExplosion>();
                explosion.Initialize(_heroIndex, Mathf.FloorToInt(_initialDamage));
                explosion.GetComponent<AutoDestroyPooledObject>().Initialize(_explosionDamagePrefab.gameObject);
            }

            if (_damageAreaPrefab)
            {
                Vector3 floorPosition = (transform.position - _sphereCenter).normalized * _sphereRadius;
                EnemyDamageArea area = ObjectPool.Instance.GetPooledObject(_damageAreaPrefab.gameObject, floorPosition, transform.rotation).GetComponent<EnemyDamageArea>();
                area.Initialize(_heroIndex, Mathf.CeilToInt(_initialDamage * _areaOfEffectDamageFactor));
                area.GetComponent<AutoDestroyPooledObject>().Initialize(_damageAreaPrefab.gameObject);
            }
        }
    }
}