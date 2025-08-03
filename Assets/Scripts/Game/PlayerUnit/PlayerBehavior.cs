using System;
using System.Collections.Generic;
using Game.Enabler;
using Game.Skills;
using UnityEngine;

namespace Game.PlayerUnit
{
    [RequireComponent(typeof(PlayerSkillTracker))]
    public abstract class PlayerBehavior : EnablerMonoBehaviour
    {
        public Action<Vector3, Vector3, Vector3> OnMoved;

        protected PlayerSkillTracker _playerSkillTracker;
        protected int _pendingLevelUp;

        protected List<PlayerSkillData> _preselectedSkills;
        
        protected virtual void Awake()
        {
            _playerSkillTracker = GetComponent<PlayerSkillTracker>();
        }

        public virtual void Initialize(Vector3 sphereCenter, float sphereRadius)
        {
            _playerSkillTracker.Initialize();
            _pendingLevelUp = 0;
            _preselectedSkills = new List<PlayerSkillData>();
        }

        public abstract float GetCurrentSpeed();

        public abstract void CalculateForwardAndUpDirections(out Vector3 forwardDirection, out Vector3 upDirection);

        public abstract bool IsMoving();

        public abstract void ManageLevelUp();

        protected virtual void GainSkill(PlayerSkillData skillData)
        {
            if (!skillData)
            {
                _pendingLevelUp = 0;
                return;
            }

            _playerSkillTracker.GainSkill(skillData);

            if (_pendingLevelUp <= 0)
            {
                _pendingLevelUp = 0;
                return;
            }

            _pendingLevelUp--;
        }

        public virtual void UpdateMaxDetectionDistance(float newAttackRangeRadius) {}

        public abstract void PushBack(float pushBackForce, Vector3 pushBackDirection);

        public void SetPreselectedSkills(List<PlayerSkillData> preselectedSkills)
        {
            _preselectedSkills = preselectedSkills;
        }
    }
}