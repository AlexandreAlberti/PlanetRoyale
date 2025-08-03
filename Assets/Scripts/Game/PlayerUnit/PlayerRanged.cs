using Application.Sound;
using UnityEngine;

namespace Game.PlayerUnit
{
    [RequireComponent(typeof(PlayerProjectileSpawner))]
    public class PlayerRanged : Player
    {
        private PlayerProjectileSpawner _playerProjectileSpawner;

        protected override void Awake()
        {
            base.Awake();
            _playerProjectileSpawner = GetComponent<PlayerProjectileSpawner>();
        }

        public override void Enable()
        {
            base.Enable();
            _playerProjectileSpawner.Enable();
        }

        public override void Disable()
        {
            base.Disable();
            _playerProjectileSpawner.Disable();
        }

        public override void Initialize(int playerIndex, Vector3 sphereCenter, float sphereRadius, string playerName)
        {
            base.Initialize(playerIndex, sphereCenter, sphereRadius, playerName);
            _playerProjectileSpawner.Initialize(sphereCenter, sphereRadius);
            _playerProjectileSpawner.OnProjectileSpawned += PlayerProjectileSpawner_OnProjectileSpawned;
        }

        private void PlayerProjectileSpawner_OnProjectileSpawned(Vector3 directionToEnemy)
        {
            SoundManager.Instance.PlayPlayerShootSound();
            FaceToEnemyDirection(directionToEnemy);
        }

        public override void AttackOnlyEnable()
        {
            _playerProjectileSpawner.Enable();
        }

        public override void AttackOnlyDisable()
        {
            _playerProjectileSpawner.Disable();
        }
    }
}