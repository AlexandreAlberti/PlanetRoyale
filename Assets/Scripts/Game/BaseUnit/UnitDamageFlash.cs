using System.Collections;
using UnityEngine;

namespace Game.BaseUnit
{
    public class UnitDamageFlash : MonoBehaviour
    {
        [SerializeField] private UnitVisuals _unitVisuals;
        [SerializeField] protected Material _flashMaterial;
        [SerializeField] private float _flashLength;

        private Coroutine _flashCoroutine;
        
        public void Initialize()
        {
            StopFlash();
            
            if (_flashCoroutine != null)
            {
                StopCoroutine(_flashCoroutine);
            }
        }

        public void Flash()
        {
            _unitVisuals.ChangeRenderersMaterial(_flashMaterial);
        
            _flashCoroutine = StartCoroutine(WaitAndStopFlash());
        }

        private void StopFlash()
        {
            _unitVisuals.RestoreOriginalRenderersMaterial();
        }

        private IEnumerator WaitAndStopFlash()
        {
            yield return new WaitForSeconds(_flashLength);

            StopFlash();
        }
    }
}