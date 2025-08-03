using Game.Enabler;
using UnityEngine;

namespace Resources.Scripts_tests
{
    public class SimpleRotator : EnablerMonoBehaviour
    {
        [SerializeField] private Vector3 _rotationAxes;
        [SerializeField] private float _rotationSpeed;
        [SerializeField] private float _rotationStartDelay;

        private float _timer;

        public void Initialize()
        {
            _timer = 0f;
            Enable();
        }
        
        private void Update()
        {
            if (!_isEnabled)
            {
                return;
            }

            _timer += Time.deltaTime;
            
            if (_timer >= _rotationStartDelay)
            {
                transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles + _rotationAxes.normalized * (_rotationSpeed * Time.deltaTime));
            }
        }
    }
}