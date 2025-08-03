using System.Collections;
using Game.BaseUnit;
using Game.PlayerUnit;
using Game.Pool;
using Game.Projectile;
using UnityEngine;

namespace Game.EnemyUnit.Attack.Active
{
    [RequireComponent(typeof(Enemy))]
    public class EnemyRangedJumpAttack : EnemyRangedAttack
    {
        private bool _isJumping;
        private Enemy _enemy;

        protected override void Awake()
        {
            base.Awake();
            _enemy = GetComponent<Enemy>();
        }

        protected override IEnumerator AttackChoreography(Player closestPlayer, Vector3 sphereCenter, float sphereRadius)
        {
            if (!_isEnabled)
            {
                yield break;
            }

            if (_isJumping)
            {
                yield break;
            }

            _isJumping = true;
            _enemy.MakeInvincible();
            StartCoroutine(base.AttackChoreography(closestPlayer, sphereCenter, sphereRadius));
        }
        
        protected override void SpawnProjectiles(Vector3 directionToPlayer, float distanceToPlayer, Vector3 sphereCenter, float sphereRadius)
        {
            if (!_isEnabled)
            {
                return;
            }

            GameObject projectileInstance = ObjectPool.Instance.GetPooledObject(_projectile, _shootingPoint.position, Quaternion.identity);
            ProjectileBase projectileBase = projectileInstance.GetComponent<ProjectileBase>();
            projectileBase.SetTargetTransform(_closestPlayerPosition, _closestPlayerRotation);
            projectileBase.Initialize(_shootingPoint.position, directionToPlayer, distanceToPlayer, sphereCenter, sphereRadius, 0, _projectile, -1);
            projectileBase.OnDestroyed -= Projectile_OnDestroyed;
            projectileBase.OnDestroyed += Projectile_OnDestroyed;
            EnemyRagdoll enemyRagdoll = projectileInstance.GetComponentInChildren<EnemyRagdoll>();
            enemyRagdoll.Initialize();
            enemyRagdoll.PlayJumpAnimation();
            _enemyVisuals.DisableVisuals();
        }

        private void Projectile_OnDestroyed(Vector3 position)
        {
            transform.position = position;
            _enemyVisuals.EnableVisuals();
            _enemy.RemoveInvincible();
            _isJumping = false;
        }
    }
}