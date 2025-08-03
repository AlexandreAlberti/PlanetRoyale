using Application.Data.Progression;
using Game.BaseUnit;
using Game.Damage;
using Game.PlayerUnit;
using UnityEngine;

namespace Game.EnemyUnit
{
    public class EnemyAttackTrigger : UnitAttackTrigger
    {
        [SerializeField] private float _damageCoolDown;
        [SerializeField] private GameObject _visuals;

        private bool _canDamage;
        private float _currentDamageCoolDown;
        private float _enemyDamageMultiplier;

        public override void Initialize(DamageEffect damageEffect, int damage, int amountOfParticles = 1)
        {
            base.Initialize(damageEffect, damage, amountOfParticles);
            _enemyDamageMultiplier = LeaguesManager.Instance.GetCurrentLeagueData().EnemyDamageMultiplier;
        }

        public override void Disable()
        {
            base.Disable();
            _canDamage = false;

            if (_visuals)
            { 
                _visuals.SetActive(false);
            }
        }

        public override void Enable()
        {
            base.Enable();
            _canDamage = true;

            if (_visuals)
            {
                _visuals.SetActive(true);
            }
        }

        private void Update()
        {
            if (!_isEnabled)
            {
                return;
            }

            _currentDamageCoolDown += Time.deltaTime;

            if (!_canDamage && _currentDamageCoolDown >= _damageCoolDown)
            {
                _currentDamageCoolDown = 0.0f;
                _canDamage = true;
            }
        }

        protected override void OnTriggerStay(Collider other)
        {
            if (!_isEnabled)
            {
                return;
            }

            if (!_canDamage)
            {
                return;
            }
            
            _canDamage = false;
            _currentDamageCoolDown = 0.0f;

            Player player = other.GetComponent<Player>();

            if (player)
            {
                player.Damage(Mathf.CeilToInt(_damage * _enemyDamageMultiplier), DamageType.Melee, _damageEffect, _amountOfParticles);
            }
        }
    }
}