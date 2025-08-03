using Game.Enabler;
using Game.Pool;
using Game.Vfx;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Drop
{
    public abstract class Drop : EnablerMonoBehaviour
    {
        [SerializeField] protected GameObject _collectedDrop;
        [SerializeField] private GameObject _collectedDropVfx;
        [SerializeField] private DropVisuals _dropVisuals;
        
        [Header("Animation")]
        [SerializeField] private float _scatterDistanceMaxRadius;
        [SerializeField] private AnimationCurve _spawnCurve;
        [SerializeField] private float _spawnDuration;
        [SerializeField] private float _spawnHeight;

        private Vector3 _startingPosition;
        private Vector3 _upDirection;
        private Vector3 _forwardDirection;
        private Vector3 _scatteredMovement;
        private GameObject _dropPrefab;
        private float _spawnTimer;
        private bool _animationEnded;

        public void Initialize(GameObject dropPrefab, Vector3 upDirection, bool useRandomScatteredMovement = false)
        {
            _scatteredMovement = Vector3.zero;

            if (useRandomScatteredMovement)
            {
                Vector2 flatMovementAmount = Random.insideUnitCircle * Random.Range(0, _scatterDistanceMaxRadius);
                Vector3 flatRightDirection = Vector3.Cross(Vector3.forward, upDirection).normalized;
                Vector3 flatForwardDirection = Vector3.Cross(upDirection, flatRightDirection).normalized;
                _scatteredMovement = flatRightDirection * flatMovementAmount.x + flatForwardDirection * flatMovementAmount.y;
            }

            _upDirection = upDirection;
            _dropPrefab = dropPrefab;
            _spawnTimer = 0.0f;
            _startingPosition = transform.position;
            _animationEnded = false;
            
            Enable();
        }

        private void Update()
        {
            if (!_isEnabled || _animationEnded)
            {
                return; 
            }
            
            _spawnTimer += Time.deltaTime;
            float animationPercentage = Mathf.Min(1.0f, _spawnTimer / _spawnDuration);
            Vector3 animationScatterPoint = _scatteredMovement * animationPercentage;
            float animationHeightPoint = _spawnCurve.Evaluate(animationPercentage);
            Vector3 moveUp = _upDirection * (_spawnHeight * animationHeightPoint);
            transform.position = _startingPosition + moveUp + animationScatterPoint;
            _dropVisuals.UpdateShadowLocalPosition(moveUp.magnitude);
            
            if (_spawnTimer >= _spawnDuration)
            {
                _dropVisuals.StartRotating();
                _animationEnded = true;
            }
        }
        
        public override void Disable()
        {
            base.Disable();
            _dropVisuals.StopRotating();
        }

        
        protected void Destroy()
        {
            Disable();
            ObjectPool.Instance.Release(_dropPrefab, gameObject);
            SpawnCollectedVfx();
        }

        private void SpawnCollectedVfx()
        {
            GameObject vfxInstance = ObjectPool.Instance.GetPooledObject(_collectedDropVfx, transform.position, transform.rotation);
            vfxInstance.GetComponent<AutoDestroyPooledObject>().Initialize(_collectedDropVfx);
        }
    }
}