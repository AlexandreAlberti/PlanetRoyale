using System.Collections.Generic;
using Game.AI;
using Game.Skills;
using UnityEngine;

namespace Game.PlayerUnit
{
    [RequireComponent(typeof(AISphericalMovement))]
    public class PlayerNpc : PlayerBehavior
    {
        [SerializeField] private SkinnedMeshRenderer _skinnedMeshRenderer;
        
        private AISphericalMovement _aiMovement;

        protected override void Awake()
        {
            base.Awake();
            _aiMovement = GetComponent<AISphericalMovement>();
        }

        public override void Enable()
        {
            base.Enable();
            _aiMovement.Enable();
        }
        
        public override void Disable()
        {
            base.Disable();
            _aiMovement.Disable();
        }

        public override float GetCurrentSpeed()
        {
            return _aiMovement.GetCurrentSpeed();
        }

        public override void Initialize(Vector3 sphereCenter, float sphereRadius)
        {
            base.Initialize(sphereCenter, sphereRadius);
            _aiMovement.Initialize(sphereCenter, sphereRadius);
            _aiMovement.OnMoved += AiMovement_OnMoved;
        }

        public void InitializeColor(PlayerNpcConfigData playerNpcConfig)
        {
            _skinnedMeshRenderer.materials = playerNpcConfig.ColorMeshMaterials;
        }
        
        private void AiMovement_OnMoved(Vector3 forwardDirection, Vector3 movementDirection, Vector3 upDirection)
        {
            OnMoved?.Invoke(forwardDirection, movementDirection, upDirection);
        }

        public override void CalculateForwardAndUpDirections(out Vector3 forwardDirection, out Vector3 upDirection)
        {
            _aiMovement.CalculateForwardAndUpDirections(out forwardDirection, out upDirection);
        }

        public override bool IsMoving()
        {
            return _aiMovement.IsMoving();
        }

        public override void ManageLevelUp()
        {
            _pendingLevelUp++;
            GainSkill(SelectRandomSkill());
        }

        protected override void GainSkill(PlayerSkillData skillData)
        {
            if (!skillData)
            {
                return;
            }

            base.GainSkill(skillData);
            
            if (_pendingLevelUp == 0)
            {
                return;
            }

            GainSkill(SelectRandomSkill());
        }

        public override void PushBack(float pushBackForce, Vector3 pushBackDirection)
        {
            _aiMovement.PushBack(pushBackForce, pushBackDirection);
        }

        private PlayerSkillData SelectRandomSkill()
        {
            List<PlayerSkillData> randomSkills = SkillManager.Instance.GetRandomSkills(1, _playerSkillTracker.MaxedOutSkills());

            if (randomSkills.Count == 0)
            {
                return null;
            }

            return randomSkills[0];
        }
    }
}