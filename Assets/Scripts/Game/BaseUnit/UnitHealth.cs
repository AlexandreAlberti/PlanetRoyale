using System;
using Application.Utils;
using Game.Damage;
using Game.Enabler;
using UnityEngine;

namespace Game.BaseUnit
{
    public class UnitHealth : EnablerMonoBehaviour
    {
        [SerializeField] private float _damageCooldown;
        
        private int _maxHealth;
        private int _maxHealthOriginal;
        private int _currentHealth;
        private int _minimumHealth;
        private bool _isInvincible;
        private float _damageTimer;
        private bool _canBeDamaged;

        public Action<int, int, int, DamageType, int, int> OnDamaged;
        public Action<int, int, int> OnHealed;
        public Action OnDead;

        public void Initialize(int maxHealth)
        {
            _currentHealth = maxHealth;
            _maxHealth = maxHealth;
            _maxHealthOriginal = maxHealth;
            _isInvincible = false;
            _canBeDamaged = true;
            _damageTimer = _damageCooldown;
            MakePlayerKillable();
        }

        private void Update()
        {
            if (!_isEnabled)
            {
                return;
            }
            
            if (_canBeDamaged)
            {
                return;
            }
            
            _damageTimer -= Time.deltaTime;
            
            if (_damageTimer <= 0.0f)
            {
                _damageTimer = _damageCooldown;
                _canBeDamaged = true;
            }
        }

        public void Damage(int amount, DamageType damageType, int damagerIndex = -1, int amountOfParticles = 1)
        {
            if (!_isEnabled)
            {
                return;
            }
            
            if (_isInvincible)
            {
                return;
            }
            
            if (!IsAlive())
            {
                return;
            }

            if (IsDamageAffectedByCooldown(damageType))
            {
                CooldownDamage(amount, damageType, damagerIndex, amountOfParticles);
            }
            else
            {
                ApplyDamage(amount, damageType, damagerIndex, amountOfParticles);
            }
        }

        private static bool IsDamageAffectedByCooldown(DamageType damageType)
        {
            switch (damageType)
            {
                case DamageType.Melee:
                case DamageType.Ranged:
                    return true;
                case DamageType.Reflected:
                case DamageType.Poison:
                default:
                    return false;
            }
        }

        private void CooldownDamage(int amount, DamageType damageType, int damagerIndex, int amountOfParticles)
        {
            if (!_isEnabled)
            {
                return;
            }

            if (!_canBeDamaged)
            {
                return;
            }
            
            _damageTimer = _damageCooldown;
            _canBeDamaged = _damageCooldown <= 0.0f;
            
            ApplyDamage(amount, damageType, damagerIndex, amountOfParticles);
        }

        private void ApplyDamage(int amount, DamageType damageType, int damagerIndex, int amountOfParticles)
        {
            _currentHealth -= amount;

            if (_currentHealth <= _minimumHealth)
            {
                _currentHealth = _minimumHealth;
            }

            OnDamaged?.Invoke(amount, _currentHealth, _maxHealth, damageType, damagerIndex, amountOfParticles);

            if (!IsAlive())
            {
                OnDead?.Invoke();
            }
        }

        public void Heal(int amount, bool ignoreDead = false)
        {
            if (!_isEnabled)
            {
                return;
            }
            
            if (_isInvincible)
            {
                return;
            }
            
            if (!IsAlive() && !ignoreDead)
            {
                return;
            }

            _currentHealth += amount;

            if (_currentHealth > _maxHealth)
            {
                _currentHealth = _maxHealth;
            }

            OnHealed?.Invoke(amount, _currentHealth, _maxHealth);
        }

        public bool IsAlive()
        {
            return _currentHealth > 0;
        }

        public void MakeInvincible()
        {
            _isInvincible = true;
        }
        
        public void RemoveInvincible()
        {
            _isInvincible = false;
        }

        public int MaxHealth()
        {
            return _maxHealth;
        }

        public bool CanBeDamaged()
        {
            return _canBeDamaged;
        }

        public void IncreaseMaxHp(int newMaxHp)
        {
            int previousHealth = _currentHealth;
            int currentDamageTaken = _maxHealth - _currentHealth;
            _maxHealth = newMaxHp;
            _currentHealth = _maxHealth - currentDamageTaken;
            OnHealed?.Invoke(_currentHealth - previousHealth, _currentHealth, _maxHealth);
        }

        public int CurrentHealth()
        {
            return _currentHealth;
        }

        public void MakePlayerNonKillable()
        {
            _minimumHealth = NumberConstants.One;
        }
        
        public void MakePlayerKillable()
        {
            _minimumHealth = NumberConstants.Zero;
        }
    }
}