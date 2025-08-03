using System;
using UnityEngine;

namespace Game.BaseUnit
{
    public class UnitStatusEffectPoison : UnitStatusEffect
    {
        [Range(0, 1)] [SerializeField] private float _poisonDamageFactor;
        [SerializeField] private float _poisonDamageCooldown;

        private float _poisonCooldownTimer;

        public Action<float> OnPoisonDamage;
        
        protected override void Update()
        {
            base.Update();
            
            if (!_isEnabled)
            {
                return;
            }

            float deltaTime = Time.deltaTime;
            _poisonCooldownTimer += deltaTime;

            if (_poisonCooldownTimer >= _poisonDamageCooldown)
            {
                _poisonCooldownTimer -= _poisonDamageCooldown;
                OnPoisonDamage?.Invoke(_poisonDamageFactor);
            }
        }

    }
}