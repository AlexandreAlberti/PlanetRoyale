using Game.BaseUnit;
using UnityEngine;

namespace Game.PlayerUnit
{
    [RequireComponent(typeof(UnitVisuals))]
    [RequireComponent(typeof(PlayerVisuals))]
    public class PlayerRagdoll : UnitRagdoll
    {
        private UnitVisuals _unitVisuals;
        private PlayerVisuals _playerVisuals;

        private void Awake()
        {
            _unitVisuals = GetComponent<UnitVisuals>();
            _playerVisuals = GetComponent<PlayerVisuals>();
        }
        
        public override void PlayDeathAnimation()
        {
            _unitVisuals.EnableDamageFlash();
            _playerVisuals.PlayDieAnimation();
        }
    }
}