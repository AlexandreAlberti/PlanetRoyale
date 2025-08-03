using System;
using Game.Detectors;
using Game.Enabler;
using Game.EnemyUnit;
using Game.Pool;
using Game.Projectile;
using UnityEngine;

namespace Game.PlayerUnit
{
    [RequireComponent(typeof(EnemyDetectorForPlayers))]
    [RequireComponent(typeof(PlayerStats))]
    public class PlayerProjectileSpawner : EnablerMonoBehaviour
    {
        [SerializeField] private GameObject _projectile;
        [SerializeField] private Transform _shootingPoint;
        [SerializeField] private PlayerBehavior _playerBehavior;
        
        private Player _player;
        private EnemyDetectorForPlayers _enemyDetectorForPlayers;
        private PlayerStats _playerStats;
        private float _shootTimer;
        private bool _canShoot;
        private Vector3 _sphereCenter;
        private float _sphereRadius;

        public Action<Vector3> OnProjectileSpawned;

        private void Awake()
        {
            _player = GetComponent<Player>();
            _enemyDetectorForPlayers = GetComponent<EnemyDetectorForPlayers>();
            _playerStats = GetComponent<PlayerStats>();
        }

        public void Initialize(Vector3 sphereCenter, float sphereRadius)
        {
            _enemyDetectorForPlayers.Initialize(_player.GetPlayerIndex() == Player.GetHumanPlayerIndex(), _player.GetCenterAnchor());
            _shootTimer = GetShootingCooldown();
            _sphereCenter = sphereCenter;
            _sphereRadius = sphereRadius;
        }

        private void Update()
        {
            if (!_isEnabled)
            {
                return;
            }
            
            _shootTimer -= Time.deltaTime;

            Enemy closestEnemy = _enemyDetectorForPlayers.GetClosestEnemy();
            
            if (_shootTimer <= 0.0f && !_playerBehavior.IsMoving() && closestEnemy)
            {
                _shootTimer = GetShootingCooldown();
                Vector3 vectorToEnemy = closestEnemy.transform.position - _shootingPoint.position;
                Vector3 directionToEnemy = vectorToEnemy.normalized;
                float distanceToEnemy = vectorToEnemy.magnitude;
                GameObject projectileInstance = ObjectPool.Instance.GetPooledObject(_projectile, _shootingPoint.position, Quaternion.identity);
                projectileInstance.GetComponent<ProjectileBase>().Initialize(_shootingPoint.position, directionToEnemy, distanceToEnemy, _sphereCenter, _sphereRadius, GetProjectileDamage(), _projectile, _player.GetPlayerIndex());
                OnProjectileSpawned?.Invoke(directionToEnemy);
            }
        }

        private int GetProjectileDamage()
        {
            return _playerStats.GetAttack();
        }
        
        private float GetShootingCooldown()
        {
            return _playerStats.GetAttackSpeedModifier();
        }
        
        public void RemoveProjectiles()
        {
            foreach (ProjectileBow projectilePlayer in FindObjectsOfType<ProjectileBow>())
            {
                projectilePlayer.Destroy();
            }
        }
    }
}