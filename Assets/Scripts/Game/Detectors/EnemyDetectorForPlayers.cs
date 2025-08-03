using Game.EnemyUnit;
using Game.PlayerUnit;
using UnityEngine;

namespace Game.Detectors
{
    [RequireComponent(typeof(PlayerStats))]
    public class EnemyDetectorForPlayers : EnemyDetector
    {
        private PlayerStats _playerStats;
        
        private void Awake()
        {
            _playerStats = GetComponent<PlayerStats>();
        }
        
        public override float GetDetectionDistance()
        {
            return _playerStats.GetAttackRange();
        }
        
        protected override void ResetTargetUnit()
        {
            if (!_closestUnit)
            {
                return;
            }
            
            _closestUnit.GetComponent<Enemy>().UnTargetEnemy();
            base.ResetTargetUnit();
        }
        
    }
}