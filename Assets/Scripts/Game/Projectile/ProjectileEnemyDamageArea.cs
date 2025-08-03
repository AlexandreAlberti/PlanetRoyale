using Game.Damage;
using Game.EnemyUnit;
using Game.Pool;
using Game.Vfx;
using UnityEngine;

namespace Game.Projectile
{
    public class ProjectileEnemyDamageArea : ProjectileDamageArea
    {
        [SerializeField] protected EnemyDamageArea _damageAreaPrefab;
        
        protected override void SpawnDamageArea()
        {
            Vector3 directionFromCenter = (transform.position - _sphereCenter).normalized;
            Vector3 surfacePosition = _sphereCenter + directionFromCenter * _sphereRadius;
            Quaternion surfaceRotation = Quaternion.FromToRotation(Vector3.up, directionFromCenter);

            GameObject damageAreaInstance = ObjectPool.Instance.GetPooledObject(
                _damageAreaPrefab.gameObject,
                surfacePosition,
                surfaceRotation
            );

            damageAreaInstance.GetComponent<AutoDestroyPooledObject>().Initialize(_damageAreaPrefab.gameObject);
            EnemyDamageArea enemyDamageArea = damageAreaInstance.GetComponent<EnemyDamageArea>();
            enemyDamageArea.Initialize(_damage);
        }
        
        protected override bool IsTarget(Collider other)
        {
            return other.GetComponent<Enemy>();
        }
    }
}