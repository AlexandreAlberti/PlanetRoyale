using System.Collections;
using Game.PlayerUnit;
using Game.Pool;
using Game.Projectile;
using UnityEngine;

namespace Game.EnemyUnit.Attack.Active
{
    public class EnemyRangedAttack : EnemyActiveAttack
    {
        [SerializeField] protected GameObject _projectile;
        [SerializeField] protected Transform _shootingPoint;

        protected Vector3 _closestPlayerPosition;
        protected Quaternion _closestPlayerRotation;

        protected override IEnumerator AttackChoreography(Player closestPlayer, Vector3 sphereCenter, float sphereRadius)
        {
            if (!_isEnabled)
            {
                yield break;
            }

            _closestPlayerPosition = closestPlayer.transform.position;
            _closestPlayerRotation = closestPlayer.transform.rotation;
            CalculateFaceToPlayerParams(_closestPlayerPosition, sphereCenter, out Vector3 directionToPlayer, out Vector3 upDirection);
            _enemyVisuals.PlayAttackAnimation(directionToPlayer, upDirection, _attackAnimationIndex);
            float distanceToPlayer = GetGreatCircleDistanceToPlayer(transform.position, _closestPlayerPosition, sphereRadius);
            yield return new WaitForSeconds(_attackStartTime);
            SpawnProjectiles(directionToPlayer, distanceToPlayer, sphereCenter, sphereRadius);
        }

        private float GetGreatCircleDistanceToPlayer(Vector3 myPosition, Vector3 playerPosition, float sphereRadius)
        {
            Vector3 myPositionNormalized = myPosition.normalized;
            Vector3 playerPositionNormalized = playerPosition.normalized;
            float angleInRadians = Mathf.Acos(Vector3.Dot(myPositionNormalized, playerPositionNormalized));
            return sphereRadius * angleInRadians;
        }
        
        protected virtual void SpawnProjectiles(Vector3 directionToPlayer, float distanceToPlayer, Vector3 sphereCenter, float sphereRadius)
        {
            if (!_isEnabled)
            {
                return;
            }

            GameObject projectileInstance = ObjectPool.Instance.GetPooledObject(_projectile, _shootingPoint.position, Quaternion.identity);
            ProjectileBase projectileBase = projectileInstance.GetComponent<ProjectileBase>();
            projectileBase.SetTargetTransform(_closestPlayerPosition, _closestPlayerRotation);
            projectileBase.Initialize(_shootingPoint.position, directionToPlayer, distanceToPlayer, sphereCenter, sphereRadius, _damage, _projectile, -1);
        }

        public override float GetAttackDuration()
        {
            return _enemyVisuals.GetAttackAnimationLength();
        }
    }
}