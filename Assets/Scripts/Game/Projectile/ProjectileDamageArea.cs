using Game.BaseObstacle;
using UnityEngine;

namespace Game.Projectile
{
    [RequireComponent(typeof(ProjectileSphericalMovement))]
    public abstract class ProjectileDamageArea : ProjectileBase
    {
        [SerializeField] protected int _damage;
        
        protected override void OnTriggerEnter(Collider other)
        {
            if (!_isEnabled)
            {
                return;
            }

            Obstacle obstacle = other.GetComponent<Obstacle>();

            if (obstacle)
            {
                SpawnDamageArea();
                Destroy();
                return;
            }

            if (IsTarget(other))
            {
                SpawnDamageArea();
                Destroy();
            }
        }

        protected abstract bool IsTarget(Collider other);
        protected abstract void SpawnDamageArea();
    }
}