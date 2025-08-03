using Game.BaseObstacle;
using Game.PlayerUnit;
using Game.Pool;
using Game.Vfx;
using UnityEngine;

namespace Game.Projectile
{
    [RequireComponent(typeof(ProjectileSphericalMovement))]
    public class ProjectileMagma : ProjectileBase
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

            Player player = other.GetComponent<Player>();

            if (player)
            {
                Explode();
                return;
            }
        }

        private void Explode()
        {
            Destroy();
            ProjectileEnemyMortarExplosion explosion = ObjectPool.Instance.GetPooledObject(_explosionDamagePrefab.gameObject, transform.position, transform.rotation).GetComponent<ProjectileEnemyMortarExplosion>();
            explosion.Initialize(-1, Mathf.FloorToInt(_initialDamage));
            explosion.GetComponent<AutoDestroyPooledObject>().Initialize(_explosionDamagePrefab.gameObject);

        }
    }
}