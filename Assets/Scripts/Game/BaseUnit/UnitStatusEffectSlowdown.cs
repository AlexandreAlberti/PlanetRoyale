using UnityEngine;

namespace Game.BaseUnit
{
    public class UnitStatusEffectSlowdown : UnitStatusEffect
    {
        [Range(0.0f, 0.5f)]
        [SerializeField] private float _movementSpeedReduction;

        private const float DefaultMovementSpeedReduction = 1.0f;
        
        public float GetMovementSpeedReduction()
        {
            if (_isEnabled)
            {
                return DefaultMovementSpeedReduction - _movementSpeedReduction;
            }

            return DefaultMovementSpeedReduction;
        }
    }
}