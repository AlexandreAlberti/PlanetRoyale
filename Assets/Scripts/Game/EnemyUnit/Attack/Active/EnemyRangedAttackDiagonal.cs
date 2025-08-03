using Game.Pool;
using Game.Projectile;
using UnityEngine;

namespace Game.EnemyUnit.Attack.Active
{
    public class EnemyRangedAttackDiagonal : EnemyRangedAttack
    {
        [SerializeField] private float _projectileShootAngle;

        protected override void SpawnProjectiles(Vector3 directionToPlayer, float distanceToPlayer, Vector3 sphereCenter, float sphereRadius)
        {
            if (!_isEnabled)
            {
                return;
            }

            base.SpawnProjectiles(directionToPlayer, distanceToPlayer, sphereCenter, sphereRadius);

            Vector3 upDirection = (transform.position - sphereCenter).normalized;
            Quaternion leftRotation = Quaternion.AngleAxis(-_projectileShootAngle, upDirection);
            Quaternion rightRotation = Quaternion.AngleAxis(_projectileShootAngle, upDirection);
            Vector3 projectileLeftDirection = leftRotation * directionToPlayer;
            Vector3 projectileRightDirection = rightRotation * directionToPlayer;

            GameObject projectileLeftInstance = ObjectPool.Instance.GetPooledObject(_projectile, _shootingPoint.position, Quaternion.identity);
            GameObject projectileRightInstance = ObjectPool.Instance.GetPooledObject(_projectile, _shootingPoint.position, Quaternion.identity);
            projectileLeftInstance.GetComponent<ProjectileBase>().Initialize(_shootingPoint.position, projectileLeftDirection, distanceToPlayer, sphereCenter, sphereRadius, _damage, _projectile, -1);
            projectileRightInstance.GetComponent<ProjectileBase>().Initialize(_shootingPoint.position, projectileRightDirection, distanceToPlayer, sphereCenter, sphereRadius, _damage, _projectile, -1);
        }
    }
}