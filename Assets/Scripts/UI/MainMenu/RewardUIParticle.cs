using System;
using DG.Tweening;
using Game.Pool;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace UI.MainMenu
{
    public class RewardUIParticle : MonoBehaviour
    {
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private Image _iconImage;
            
        private Vector3 _growDirection;
        private float _growDirectionSpeed;
        private Vector3 _destination;
        private float _growTimeMax;
        private float _growTime;
        private float _travelTimeMax;
        private bool _isEnabled;
        private RewardUIParticle _prefab;
        
        public Action<RewardUIParticle> OnParticleReached;

        private void Awake()
        {
            _isEnabled = false;
        }

        private void Update()
        {
            if (!_isEnabled)
            {
                return;
            }

            if (_growTime > 0.0f)
            {
                _growTime -= Time.deltaTime;
                float currentScale = 1 - Mathf.Max(0.0f, _growTime / _growTimeMax);
                transform.localScale = new Vector3(currentScale, currentScale, currentScale);
                transform.position += _growDirection * (_growDirectionSpeed * Time.deltaTime);
                return;
            }

            transform.DOMove(_destination, _travelTimeMax).SetEase(Ease.InSine).onComplete = OnParticlesAnimationCompleted;
            _isEnabled = false;
        }

        public void InitializeParticle(RewardUIParticle prefab, Sprite iconSprite, float growDirectionSpeed, float growTime, Vector3 destination, float travelTime, bool isMaterial, Color particleSpriteMaterialColor)
        {
            _prefab = prefab;
            gameObject.SetActive(true);
            _iconImage.sprite = iconSprite;
            _iconImage.color = isMaterial ? particleSpriteMaterialColor : Color.white;
            _backgroundImage.enabled = isMaterial;
            transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);
            _growDirection = Random.insideUnitCircle.normalized;
            _growDirectionSpeed = growDirectionSpeed;
            _destination = destination;
            _travelTimeMax = travelTime;
            _growTimeMax = growTime;
            _growTime = growTime;
            _isEnabled = true;
        }

        private void OnParticlesAnimationCompleted()
        {
            OnParticleReached?.Invoke(this);
            _isEnabled = false;
            ReleaseParticleFromPool();
        }

        public void ReleaseParticleFromPool()
        {
            transform.DOKill();
            ObjectPool.Instance.Release(_prefab.gameObject, gameObject);
            gameObject.SetActive(false);
        }
    }
}