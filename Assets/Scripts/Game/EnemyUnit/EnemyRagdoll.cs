using Game.BaseUnit;
using UnityEngine;

namespace Game.EnemyUnit
{
    [RequireComponent(typeof(UnitVisuals))]
    [RequireComponent(typeof(EnemyVisuals))]
    public class EnemyRagdoll : UnitRagdoll
    {
        private UnitVisuals _unitVisuals;
        private EnemyVisuals _enemyVisuals;

        private void Awake()
        {
            _unitVisuals = GetComponent<UnitVisuals>();
            _enemyVisuals = GetComponent<EnemyVisuals>();
        }

        public override void Initialize()
        {
            _enemyVisuals.Initialize();
        }

        public override void PlayDeathAnimation()
        {
            _unitVisuals.EnableDamageFlash();
            _enemyVisuals.PlayDieAnimation();
        }

        public override void PlaySpawnAnimation()
        {
            _enemyVisuals.PlayAppearAnimation();
        }

        public float GetSpawnAnimationDuration()
        {
            return _enemyVisuals.GetAppearAnimationLength();
        }

        public void PlayJumpAnimation()
        {
            _enemyVisuals.PlayJumpAnimation();
        }

        public UnitVisuals GetUnitVisuals()
        {
            return _unitVisuals;
        }
    }
}