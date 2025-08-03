using System.Collections;
using System.Collections.Generic;
using Game.Camera;
using Game.Skills;
using Game.UI;
using UnityEngine;

namespace Game.PlayerUnit
{
    [RequireComponent(typeof(PlayerSphericalMovement))]
    public class PlayerHuman : PlayerBehavior
    {
        private PlayerSphericalMovement _playerSphericalMovement;

        protected override void Awake()
        {
            base.Awake();
            _playerSphericalMovement = GetComponent<PlayerSphericalMovement>();
        }

        public override void Enable()
        {
            base.Enable();
            _playerSphericalMovement.Enable();
        }

        public override void Disable()
        {
            base.Disable();
            _playerSphericalMovement.Disable();
        }

        public override float GetCurrentSpeed()
        {
            return _playerSphericalMovement.GetCurrentSpeed();
        }

        public override void Initialize(Vector3 sphereCenter, float sphereRadius)
        {
            base.Initialize(sphereCenter, sphereRadius);
            _playerSphericalMovement.Initialize(sphereCenter, sphereRadius);
            _playerSphericalMovement.OnMoved += PlayerSphericalMovement_OnMoved;
        }

        public override void CalculateForwardAndUpDirections(out Vector3 forwardDirection, out Vector3 upDirection)
        {
            _playerSphericalMovement.CalculateForwardAndUpDirections(out forwardDirection, out upDirection);
        }

        public override bool IsMoving()
        {
            return _playerSphericalMovement.IsMoving();
        }

        private void PlayerSphericalMovement_OnMoved(Vector3 forwardDirection, Vector3 movementDirection, Vector3 upDirection)
        {
            OnMoved?.Invoke(forwardDirection, movementDirection, upDirection);
        }

        public override void ManageLevelUp()
        {
            _pendingLevelUp++;
            StartCoroutine(OpenSkillSelectorPanel());
        }

        protected override void GainSkill(PlayerSkillData skillData)
        { 
            base.GainSkill(skillData);            

            if (_pendingLevelUp == 0)
            {
                return;
            }

            StartCoroutine(OpenSkillSelectorPanel());
        }

        private IEnumerator OpenSkillSelectorPanel()
        {
            yield return new WaitForSeconds(UIManager.Instance.GetTimeBetweenSkillSelection());
            
            if (UIManager.Instance.IsSkillPanelOpened())
            {
                yield break;
            }
            
            if (_preselectedSkills.Count == 0)
            {
                _preselectedSkills = SkillManager.Instance.GetRandomSkills(UIManager.Instance.GetAmountOfSkillsToChoose(), _playerSkillTracker.MaxedOutSkills());
            }
            
            UIManager.Instance.ShowSkillSelectionPanel(_preselectedSkills);
            UIManager.Instance.OnLevelUpSkillSelected += UIManager_OnLevelUpSkillSelected;
            _preselectedSkills.Clear();
        }
        
        private void UIManager_OnLevelUpSkillSelected(PlayerSkillData skillData)
        {
            UIManager.Instance.OnLevelUpSkillSelected -= UIManager_OnLevelUpSkillSelected;
            GainSkill(skillData);
        }

        public override void UpdateMaxDetectionDistance(float newAttackRangeRadius)
        {
            CameraManager.Instance.UpdateOffset(newAttackRangeRadius);
        }

        public override void PushBack(float pushBackForce, Vector3 pushBackDirection)
        {
            _playerSphericalMovement.PushBack(pushBackForce, pushBackDirection);
        }
    }
}