using Game.BaseObstacle;
using Game.EnemyUnit;
using Game.Pool;
using Game.Vfx;
using UnityEngine;

namespace Game.Projectile
{
    [RequireComponent(typeof(ProjectileSphericalMovement))]
    public class ProjectileFireball : ProjectileBase
    {
        [SerializeField] protected ProjectileMortarExplosion _explosionDamagePrefab;

        protected override void OnTriggerEnter(Collider other)
        {
            if (!_isEnabled)
            {
                return;
            }

            Obstacle obstacle = other.GetComponent<Obstacle>();

            if (obstacle)
            {
                Explode();
                return;
            }

            Enemy enemy = other.GetComponent<Enemy>();

            if (enemy)
            {
                Explode();
                return;
            }
        }

        private void Explode()
        {
            Destroy();
            ProjectilePlayerMortarExplosion explosion = ObjectPool.Instance.GetPooledObject(_explosionDamagePrefab.gameObject, transform.position, transform.rotation).GetComponent<ProjectilePlayerMortarExplosion>();
            explosion.Initialize(_heroIndex, Mathf.FloorToInt(_initialDamage));
            explosion.GetComponent<AutoDestroyPooledObject>().Initialize(_explosionDamagePrefab.gameObject);

        }
    }
}