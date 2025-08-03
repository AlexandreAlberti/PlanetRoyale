using System;
using Game.Enabler;
using UnityEngine;

namespace Game.BaseUnit
{
    public abstract class UnitStatusEffect : EnablerMonoBehaviour
    {
        [SerializeField] private float _statusEffectDuration;
        [SerializeField] private GameObject _statusEffectVfx;

        private float _statusEffectTimeLeft;

        public Action OnStatusEffectEnd;
        public Action OnStatusEffectStart;
        
        protected virtual void Update()
        {
            if (!_isEnabled)
            {
                return;
            }

            float deltaTime = Time.deltaTime;
            _statusEffectTimeLeft -= deltaTime;

            if (_statusEffectTimeLeft < -deltaTime)
            {
                OnStatusEffectEnd?.Invoke();
                Disable();
            }
        }

        public void ApplyStatusEffect()
        {
            Enable();
            _statusEffectTimeLeft = _statusEffectDuration;
            OnStatusEffectStart?.Invoke();
        }
        
        public override void Enable()
        {
            base.Enable();

            if (_statusEffectVfx)
            {
                _statusEffectVfx.SetActive(true);
            }
        }

        public override void Disable()
        {
            if (_statusEffectVfx)
            {
                _statusEffectVfx.SetActive(false);
            }

            base.Disable();
        }

    }
}