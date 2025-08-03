using System;
using Game.Damage;
using Game.Enabler;
using UnityEngine;

namespace Game.BaseUnit
{
    [SelectionBase]
    [RequireComponent(typeof(UnitHealth))]
    [RequireComponent(typeof(UnitVisuals))]
    [RequireComponent(typeof(UnitStatusEffectPoison))]
    [RequireComponent(typeof(UnitStatusEffectSlowdown))]
    public class Unit : EnablerMonoBehaviour
    {
        [SerializeField] private Transform _center;
        [SerializeField] private Transform _floor;
        [SerializeField] private int _initialHealth;

        public Action<int, int, int, DamageType, int> OnDamaged;
        public Action<int, int> OnHealed;
        public Action<Unit, int> OnDead;

        private UnitHealth _unitHealth;
        private UnitVisuals _unitVisuals;
        private UnitStatusEffectPoison _unitStatusEffectPoison;
        private UnitStatusEffectSlowdown _unitStatusEffectSlowdown;
        private float _unitRadius;
        
        
        private int _lastHeroIndexProvidingDamage;

        private void Awake()
        {
            _unitHealth = GetComponent<UnitHealth>();
            _unitVisuals = GetComponentInChildren<UnitVisuals>();
            _unitStatusEffectPoison = GetComponentInChildren<UnitStatusEffectPoison>();
            _unitStatusEffectSlowdown = GetComponentInChildren<UnitStatusEffectSlowdown>();
            _unitRadius = GetComponentInChildren<CapsuleCollider>().radius;
        }

        public void Initialize(int forcedInitialHealth = 0, float healthMultiplier = 1)
        {
            int initialHealth = forcedInitialHealth != 0 ? forcedInitialHealth : Mathf.FloorToInt(_initialHealth * healthMultiplier);
            _unitHealth.Initialize(initialHealth);
            _unitVisuals.Initialize(initialHealth);
            _unitStatusEffectPoison.Disable();
            _unitStatusEffectPoison.OnStatusEffectStart += StatusEffectPoison_OnStatusEffectStart;
            _unitStatusEffectPoison.OnStatusEffectEnd += StatusEffectPoison_OnStatusEffectEnd;
            _unitStatusEffectPoison.OnPoisonDamage += StatusEffectPoison_OnPoisonDamage;
            _unitStatusEffectSlowdown.Disable();
            _unitStatusEffectSlowdown.OnStatusEffectStart += StatusEffectSlowdown_OnStatusEffectStart;
            _unitStatusEffectSlowdown.OnStatusEffectEnd += StatusEffectSlowdown_OnStatusEffectEnd;
        }

        public override void Enable()
        {
            if (_isEnabled)
            {
                return;
            }

            _unitHealth.OnDamaged += UnitHealth_OnDamaged;
            _unitHealth.OnHealed += UnitHealth_OnHealed;
            _unitHealth.OnDead += UnitHealth_OnDead;
            _unitHealth.Enable();
            base.Enable();
        }

        public override void Disable()
        {
            if (!_isEnabled)
            {
                return;
            }

            base.Disable();
            _unitHealth.OnDamaged -= UnitHealth_OnDamaged;
            _unitHealth.OnHealed -= UnitHealth_OnHealed;
            _unitHealth.OnDead -= UnitHealth_OnDead;
            _unitHealth.Disable();
        }

        private void UnitHealth_OnDamaged(int damageAmount, int currentHealth, int maxHealth, DamageType damageType, int damagerIndex, int amountOfParticles)
        {
            switch (damageType)
            {
                case DamageType.Reflected:
                    _unitVisuals.SpawnReflectedDamageTextParticle(damageAmount);
                    break;
                case DamageType.Poison:
                    _unitVisuals.SpawnPoisonDamageTextParticle(damageAmount);
                    break;
                case DamageType.Melee:
                case DamageType.Ranged:
                case DamageType.Environment:
                default:
                    _unitVisuals.SpawnDamageTextParticle(damageAmount, amountOfParticles);
                    break;
            }

            OnDamaged?.Invoke(damageAmount, currentHealth, maxHealth, damageType, damagerIndex);
        }

        private void UnitHealth_OnHealed(int healAmount, int currentHealth, int maxHealth)
        {
            _unitVisuals.SpawnHealTextParticle(healAmount);
        }

        private void UnitHealth_OnDead()
        {
            _unitVisuals.RestoreOriginalRenderersMaterial();
            _unitVisuals.PlayDeathAnimation();
            OnDead?.Invoke(this, _lastHeroIndexProvidingDamage);
        }

        public void Damage(int damageAmount, DamageType damageType, DamageEffect damageEffect, int damagerIndex = -1, int amountOfParticles = 1)
        {
            if (!_isEnabled)
            {
                return;
            }

            _lastHeroIndexProvidingDamage = damagerIndex;
            _unitHealth.Damage(damageAmount, damageType, damagerIndex, amountOfParticles);

            switch (damageEffect)
            {
                case DamageEffect.Poison:
                    Poison();
                    break;
                case DamageEffect.Slowdown:
                    Slowdown();
                    break;
            }
            
            if (!_unitHealth.IsAlive())
            {
                return;
            }

            if (damageAmount == 0)
            {
                return;
            }
            
            _unitVisuals.EnableDamageFlash();
        }

        public void Heal(int amountToHeal)
        {
            if (!_isEnabled)
            {
                return;
            }

            _unitHealth.Heal(amountToHeal);
        }
        
        public void HealPercentage(float healAmountPercentage)
        {
            if (!_isEnabled)
            {
                return;
            }
            
            _unitHealth.Heal((int)(_unitHealth.MaxHealth() * healAmountPercentage));
        }

        public Transform GetCenterAnchor()
        {
            return _center;
        }

        public Transform GetFloorAnchor()
        {
            return _floor;
        }

        public void InstantKill(DamageType damageType)
        {
            if (!_isEnabled)
            {
                return;
            }

            _unitHealth.Damage(_unitHealth.MaxHealth(), damageType);
        }

        public void IncreaseMaxHp(int newMaxHp)
        {
            _unitHealth.IncreaseMaxHp(newMaxHp);
        }
        
        public bool IsAlive()
        {
            return _unitHealth.IsAlive();
        }

        private void Poison()
        {
            _unitStatusEffectPoison.ApplyStatusEffect();
        }

        private void Slowdown()
        {
            _unitStatusEffectSlowdown.ApplyStatusEffect();
        }
        
        private void StatusEffectPoison_OnPoisonDamage(float amount)
        {
            Damage(Mathf.FloorToInt(amount * _unitHealth.MaxHealth()), DamageType.Poison, DamageEffect.None);
        }

        private void StatusEffectPoison_OnStatusEffectStart()
        {
            _unitVisuals.ShowPoisonedStatus();
        }

        private void StatusEffectPoison_OnStatusEffectEnd()
        {
            _unitVisuals.HidePoisonedStatus();
        }

        private void StatusEffectSlowdown_OnStatusEffectStart()
        {
            _unitVisuals.ShowSlowdownStatus();
        }

        private void StatusEffectSlowdown_OnStatusEffectEnd()
        {
            _unitVisuals.HideSlowdownStatus();
        }

        public void MakeInvincible()
        {
            _unitHealth.MakeInvincible();
        }

        public void RemoveInvincible()
        {
            _unitHealth.RemoveInvincible();
        }

        public int CurrentHealth()
        {
            return _unitHealth.CurrentHealth();
        }

        public void MakePlayerNonKillable()
        {
            _unitHealth.MakePlayerNonKillable();
        }

        public float GetUnitRadius()
        {
            return _unitRadius;
        }

        public void SpawnDodgeTextParticle()
        {
            _unitVisuals.SpawnDodgeTextParticle();
        }

        public void HideIceHazardHideableObjects()
        {
            _unitVisuals.HideIceHazardHideableObjects();
        }
        
        public void ShowIceHazardHideableObjects()
        {
            _unitVisuals.ShowIceHazardHideableObjects();
        }
    }
}