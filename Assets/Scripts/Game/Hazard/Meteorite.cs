using Game.Damage;
using Game.Pool;
using Game.Projectile;
using Game.Vfx;
using UnityEngine;

namespace Game.Hazard
{
    public class Meteorite : ProjectileMortarEnemy
    {
        [SerializeField] private string _landingMarkAnimationName;
        
        private int _meteoriteExplosionDamage;
        private int _meteoriteAreaOfEffectBaseDamage;
        private int _meteoriteAreaOfEffectDuration;

        public void Initialize(int explosionDamage, int areaOfEffectBaseDamage, int areaOfEffectDuration)
        {
            _meteoriteExplosionDamage = explosionDamage;
            _meteoriteAreaOfEffectBaseDamage = areaOfEffectBaseDamage;
            _meteoriteAreaOfEffectDuration = areaOfEffectDuration;
            PlayLandingMarkAnimation(_landingMarkAnimationName);
        }
        
        protected override void SpawnExplosionDamageAndAreaOfEffect()
        {
            if (_explosionDamagePrefab)
            {
                ProjectileEnemyMortarExplosion explosion = ObjectPool.Instance.GetPooledObject(_explosionDamagePrefab.gameObject, transform.position, transform.rotation).GetComponent<ProjectileEnemyMortarExplosion>();
                explosion.Initialize(_heroIndex, _meteoriteExplosionDamage);
                explosion.GetComponent<AutoDestroyPooledObject>().Initialize(_explosionDamagePrefab.gameObject);
            }

            if (_damageAreaPrefab)
            {
                Vector3 floorPosition = (transform.position - _sphereCenter).normalized * _sphereRadius;
                PlayerDamageArea area = ObjectPool.Instance.GetPooledObject(_damageAreaPrefab.gameObject, floorPosition, transform.rotation).GetComponent<PlayerDamageArea>();
                area.Initialize(_meteoriteAreaOfEffectBaseDamage);
                AutoDestroyPooledObject autoDestroyPooledObject = area.GetComponent<AutoDestroyPooledObject>();
                autoDestroyPooledObject.ForceAutoDestroyTime(_meteoriteAreaOfEffectDuration);
                autoDestroyPooledObject.Initialize(_damageAreaPrefab.gameObject);
            }
        }

    }
}