using Game.BaseUnit;
using Game.Enabler;
using UnityEngine;

namespace Game.PlayerUnit
{
    public class PlayerIceHazardFogFieldOfView : EnablerMonoBehaviour
    {
        [SerializeField] private SphereCollider _sphereCollider;

        public void Initialize(float colliderRadius)
        {
            _sphereCollider.radius = colliderRadius;
            Enable();
        }
        
        public override void Enable()
        {
            base.Enable();
            _sphereCollider.enabled = true;
        }
        public override void Disable()
        {
            base.Disable();
            _sphereCollider.enabled = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_isEnabled)
            {
                return;
            }
            
            UnitVisuals unitVisuals = other.GetComponent<UnitVisuals>();

            if (unitVisuals)
            {
                unitVisuals.ShowIceHazardHideableObjects();
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (!_isEnabled)
            {
                return;
            }

            UnitVisuals unitVisuals = other.GetComponent<UnitVisuals>();

            if (unitVisuals)
            {
                unitVisuals.HideIceHazardHideableObjects();
            }
        }
    }
}