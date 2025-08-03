using System.Collections.Generic;
using Game.BaseUnit;
using Game.Enabler;
using UnityEngine;

namespace Game.Detectors
{
    public abstract class UnitDetector : EnablerMonoBehaviour
    {
        [SerializeField] private float _detectionCooldown;
        [SerializeField] private LayerMask _obstacleMask;
        
        private Transform _detectorCenterPosition;
        private float _detectionTimer;
        protected Unit _closestUnit;
        private bool _isHumanPlayer;
        private RaycastHit[] _raycastHits;

        public void Initialize(bool isHumanPlayer, Transform detectorCenterPosition)
        {
            _raycastHits = new RaycastHit[1];
            _closestUnit = null;
            _isHumanPlayer = isHumanPlayer;
            _detectorCenterPosition = detectorCenterPosition;
            Enable();
        }
        
        private void Update()
        {
            if (!_isEnabled)
            {
                return;
            }

            _detectionTimer -= Time.deltaTime;

            if (_detectionTimer <= 0.0f)
            {
                _detectionTimer = _detectionCooldown;

                FindClosestUnit();
            }
        }

        public abstract float GetDetectionDistance();
        
        protected abstract List<Unit> GetUnitsList();

        public void FindClosestUnit(Unit unitToIgnore = null)
        {
            Unit newClosestUnit = null;
            float closestDistance = GetDetectionDistance();

            foreach (Unit unit in GetUnitsList())
            {
                if (!unit || unitToIgnore == unit)
                {
                    continue;
                }
                
                Vector3 direction = unit.GetCenterAnchor().position - _detectorCenterPosition.position;
                float distance = direction.magnitude - unit.GetUnitRadius();

                if (distance < closestDistance)
                {
                    Ray ray = new Ray(_detectorCenterPosition.position, direction.normalized);
                    int hitCount = Physics.RaycastNonAlloc(ray, _raycastHits, distance, _obstacleMask);

                    if (hitCount == 0)
                    {
                        closestDistance = distance;
                        newClosestUnit = unit;
                    }
                }
            }

            bool differentUnitDetected = newClosestUnit != _closestUnit;
            
            if (!newClosestUnit || differentUnitDetected)
            {
                ResetTargetUnit();
            }
            
            if (newClosestUnit && differentUnitDetected)
            {
                _closestUnit = newClosestUnit;
                _closestUnit.OnDead += Unit_OnDead;

                if (_isHumanPlayer)
                {
                    TargetUnit();
                }

            }
        }

        protected virtual void ResetTargetUnit()
        {
            if (!_closestUnit)
            {
                return;
            }
            
            _closestUnit.OnDead -= Unit_OnDead;
            _closestUnit = null;
        }
        
        protected virtual void TargetUnit()
        {
        }
        
        private void Unit_OnDead(Unit unit, int heroIndex)
        {
            unit.OnDead -= Unit_OnDead;
            _closestUnit = null;
        }
    }
}