using UnityEngine;

namespace Game.Detectors
{
    public class EnemyDetectorForProjectiles : EnemyDetector
    {
        [SerializeField] private float _detectionDistance;

        public override float GetDetectionDistance()
        {
            return _detectionDistance;
        }
    }
}