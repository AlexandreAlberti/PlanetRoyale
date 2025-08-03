using DG.Tweening;
using Game.Enabler;
using Game.Pool;
using Game.Vfx;
using UnityEngine;

namespace Game.Drop
{
    public abstract class DropCollected : EnablerMonoBehaviour
    {
        [SerializeField] private GameObject _collectedVfx;
        [SerializeField] private float _travelDuration;
        [SerializeField] private DropVisuals _dropVisuals;

        private const float One = 1.0f;
        private const float TimeScaleFactor = 1.05f;

        private GameObject _prefab;
        private Transform _target;
        private Vector3 _targetLastPosition;
        private Tweener _tween;
        protected int _targetIndex;
        private float _timeScale;

        protected void Initialize(GameObject prefab, Transform target, int targetIndex)
        {
            _prefab = prefab;
            _target = target;
            _targetIndex = targetIndex;
            _tween = transform.DOMove(target.position, _travelDuration);
            _targetLastPosition = target.position;
            _timeScale = One;
            Enable();
        }

        void Update()
        {
            if (!_isEnabled)
            {
                return;
            }

            if (_targetLastPosition == _target.position)
            {
                return;
            }

            _tween.ChangeEndValue(_target.position, true).Restart();
            _tween.timeScale = _timeScale;
            _targetLastPosition = _target.position;
            _timeScale *= TimeScaleFactor;
        }

        public override void Disable()
        {
            base.Disable();
            _tween?.Kill();
        }

        protected void Destroy()
        {
            Disable();
            ObjectPool.Instance.Release(_prefab, gameObject);
            SpawnCollectedVfx();
        }

        private void SpawnCollectedVfx()
        {
            GameObject vfxInstance = ObjectPool.Instance.GetPooledObject(_collectedVfx, transform.position, transform.rotation);
            vfxInstance.GetComponent<AutoDestroyPooledObject>().Initialize(_collectedVfx);
        }
    }
}