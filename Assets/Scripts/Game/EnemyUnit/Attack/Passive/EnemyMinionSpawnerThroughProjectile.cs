using System.Collections.Generic;
using Game.Pool;
using Game.Projectile;
using UnityEngine;
using UnityEngine.AI;

namespace Game.EnemyUnit.Attack.Passive
{
    public class EnemyMinionSpawnerThroughProjectile : EnemyMinionSpawner
    {
        [SerializeField] private Transform _projectileSpawnPoint;
        [SerializeField] private GameObject _minionEnemyEggPrefab;

        private Vector3 _sphereCenter;
        private float _sphereRadius;
        private List<ProjectileSphericalMortarMovement> _projectiles;

        public override void Initialize()
        {
            base.Initialize();
            _projectiles = new List<ProjectileSphericalMortarMovement>();
        }

        public void SetSphereParameters(Vector3 sphereCenter, float sphereRadius)
        {
            _sphereCenter = sphereCenter;
            _sphereRadius = sphereRadius;
        }

        protected override void SpawnMinionLogic()
        {
            if (!_isEnabled)
            {
                return;
            }

            Vector3 targetPoint = EnemyManager.Instance.GeneratePointInSurfaceWithRadius(transform.position, _minionSpawnRadius, NavMesh.AllAreas);
            Vector3 vectorToTarget = targetPoint - transform.position;
            GameObject projectileInstance = ObjectPool.Instance.GetPooledObject(_minionEnemyEggPrefab, _projectileSpawnPoint.position, Quaternion.identity);
            projectileInstance.GetComponent<ProjectileMortarEnemy>().Initialize(_projectileSpawnPoint.position, vectorToTarget.normalized, vectorToTarget.magnitude,
                _sphereCenter, _sphereRadius, 0, _minionEnemyEggPrefab, -1);
            ProjectileSphericalMortarMovement movement = projectileInstance.GetComponent<ProjectileSphericalMortarMovement>();
            movement.OnMortarEndedMovement += OnMortarEndedMovement;
            _projectiles.Add(movement);
        }

        private void OnMortarEndedMovement(ProjectileSphericalMortarMovement instance)
        {
            instance.OnMortarEndedMovement -= OnMortarEndedMovement;
            
            if (!_isEnabled)
            {
                return;
            }

            _projectiles.Remove(instance);
            StartCoroutine(EnemyManager.Instance.SpawnEnemyChoreography(instance.transform.position, _minionEnemyConfig.EnemyPrefab, _minionEnemyConfig.EnemyRagdollPrefab,
                null, null, Tier.None, creatorInstanceId: GetInstanceID(), isMinion: true, useSpawnerVisuals: false));
        }

        protected override void Enemy_OnDead(Enemy enemy, int heroIndex)
        {
            base.Enemy_OnDead(enemy, heroIndex);

            foreach (ProjectileSphericalMortarMovement projectile in _projectiles)
            {
                projectile.OnMortarEndedMovement -= OnMortarEndedMovement;
            }
        }
    }
}