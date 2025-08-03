using UnityEngine;

namespace Game.PlayerUnit
{
    [RequireComponent(typeof(PlayerMeleeAttacker))]
    public class PlayerMelee : Player
    {
        private PlayerMeleeAttacker _playerMeleeAttacker;

        protected override void Awake()
        {
            base.Awake();
            _playerMeleeAttacker = GetComponent<PlayerMeleeAttacker>();
        }

        public override void Enable()
        {
            base.Enable();
            _playerMeleeAttacker.Enable();
        }

        public override void Disable()
        {
            base.Disable();
            _playerMeleeAttacker.Disable();
        }

        public override void Initialize(int playerIndex, Vector3 sphereCenter, float sphereRadius, string playerName)
        {
            base.Initialize(playerIndex, sphereCenter, sphereRadius, playerName);
            _playerMeleeAttacker.Initialize();
            _playerMeleeAttacker.OnAttackPerformed += PlayerMeleeAttacker_OnAttackPerformed;
        }

        private void PlayerMeleeAttacker_OnAttackPerformed(Vector3 directionToEnemy)
        {
            FaceToEnemyDirection(directionToEnemy);
        }
        
        public override void AttackOnlyEnable()
        {
            _playerMeleeAttacker.Enable();
        }

        public override void AttackOnlyDisable()
        {
            _playerMeleeAttacker.Disable();
        }
    }
}