using System;
using System.Collections;
using Game.Damage;
using Game.Detectors;
using Game.Enabler;
using Game.EnemyUnit;
using UnityEngine;

namespace Game.PlayerUnit
{
    [RequireComponent(typeof(EnemyDetectorForPlayers))]
    [RequireComponent(typeof(PlayerStats))]
    public class PlayerMeleeAttacker : EnablerMonoBehaviour
    {
        [SerializeField] private PlayerAttackTrigger _playerAttackTrigger;
        [SerializeField] private float _pushBackForce;
        [SerializeField] private float _attackingCooldown;
        [SerializeField] private int _attackDamage;
        [SerializeField] private PlayerBehavior _playerBehavior;
        [SerializeField] private float _attackStartTime;
        [SerializeField] private float _attackTriggerDuration;
        [SerializeField] private ParticleSystem[] _attackParticles;
        [SerializeField] private TrailRenderer[] _attackTrails;
        
        private const int AttackTriggerFramesToRemainActive = 3;
        
        private Player _player;
        private EnemyDetectorForPlayers _enemyDetectorForPlayers;
        private PlayerStats _playerStats;
        private float _attackTimer;
        private float _initialAttackRange;

        public Action<Vector3> OnAttackPerformed;

        private void Awake()
        {
            _player = GetComponent<Player>();
            _enemyDetectorForPlayers = GetComponent<EnemyDetectorForPlayers>();
            _playerStats = GetComponent<PlayerStats>();
        }

        public void Initialize()
        {
            _initialAttackRange = _playerStats.GetAttackRange();
            _enemyDetectorForPlayers.Initialize(_player.GetPlayerIndex() == PlayerManager.Instance.GetHumanPlayer().GetPlayerIndex(), _player.GetCenterAnchor());
            _playerAttackTrigger.Initialize(_player.GetPlayerIndex(), _playerStats, DamageEffect.None, _attackDamage, 1, _pushBackForce);
            _playerAttackTrigger.Disable();
            _playerStats.OnAttackRangeUpdated += PlayerStats_OnAttackRangeUpdated;
            DeactivateAttackTrails();
            _attackTimer = _attackingCooldown;
        }

        private void PlayerStats_OnAttackRangeUpdated()
        {
            float newScale = _playerStats.GetAttackRange() / _initialAttackRange;
            _playerAttackTrigger.transform.localScale = new Vector3(newScale, newScale, newScale);
        }

        private void Update()
        {
            if (!_isEnabled)
            {
                return;
            }
            
            _attackTimer -= Time.deltaTime;

            Enemy closestEnemy = _enemyDetectorForPlayers.GetClosestEnemy();
            
            if (_attackTimer <= 0.0f && !_playerBehavior.IsMoving() && closestEnemy)
            {
                _attackTimer = GetShootingCooldown();
                Vector3 vectorToEnemy = closestEnemy.transform.position - transform.position;
                Vector3 directionToEnemy = vectorToEnemy.normalized;
                OnAttackPerformed?.Invoke(directionToEnemy);
                StartCoroutine(AttackChoreography());
                float signedAngle = Vector3.SignedAngle(transform.forward, directionToEnemy, transform.up);
                _playerAttackTrigger.transform.localRotation = Quaternion.AngleAxis(signedAngle, Vector3.up);
            }
        }
        
        private float GetShootingCooldown()
        {
            return _attackingCooldown * _playerStats.GetAttackSpeedModifier();
        }

        private IEnumerator AttackChoreography()
        {
            yield return new WaitForSeconds(_attackStartTime);
            _playerAttackTrigger.Enable();
            ActivateAttackTrails();
            PlayAttackParticles();

            for (int i = 0; i < AttackTriggerFramesToRemainActive; i++)
            {
                yield return new WaitForEndOfFrame();
            }
            
            _playerAttackTrigger.Disable();
            yield return new WaitForSeconds(_attackTriggerDuration);
            DeactivateAttackTrails();
        }

        public void UpdateAttackRange(float attackRangeModifier)
        {
            _playerAttackTrigger.transform.localScale = new Vector3(attackRangeModifier, attackRangeModifier, attackRangeModifier);
        }

        private void PlayAttackParticles()
        {
            foreach (ParticleSystem attackParticle in _attackParticles)
            {
                attackParticle.Play();
            }
        }

        private void DeactivateAttackTrails()
        {
            foreach (TrailRenderer attackTrail in _attackTrails)
            {
                attackTrail.enabled = false;
            }
        }

        private void ActivateAttackTrails()
        {
            foreach (TrailRenderer attackTrail in _attackTrails)
            {
                attackTrail.enabled = true;
            }
        }
    }
}