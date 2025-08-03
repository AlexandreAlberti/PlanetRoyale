using Application.Utils;
using DG.Tweening;
using Game.Enabler;
using UnityEngine;

namespace Game.Drop
{
    public class DropVisuals : EnablerMonoBehaviour
    {
        [SerializeField] private Ease _rotationEase;
        [SerializeField] private float _rotationDuration;
        [SerializeField] private Transform _shadow;

        private const float FullRotation = 360.0f;
        private const int InfiniteLoops = -1;
        
        private Tweener _tween;
        private Vector3 _startingShadowLocalPosition;

        public void StartRotating()
        {
            _shadow.localPosition = _startingShadowLocalPosition;
            _tween = transform.DOLocalRotate(new Vector3(NumberConstants.Zero, FullRotation, NumberConstants.Zero), _rotationDuration, RotateMode.FastBeyond360)
                .SetEase(_rotationEase).SetLoops(InfiniteLoops, LoopType.Restart);
        }
        
        public void StopRotating()
        {
            _tween?.Kill();
        }

        public void UpdateShadowLocalPosition(float moveUpMagnitude)
        {
            _shadow.localPosition = _startingShadowLocalPosition - Vector3.up * moveUpMagnitude;
        }

        private void Awake()
        {
            _startingShadowLocalPosition = _shadow.localPosition;
        }
    }
}