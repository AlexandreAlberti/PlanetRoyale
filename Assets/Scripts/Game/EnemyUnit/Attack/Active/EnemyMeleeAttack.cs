using System.Collections;
using Game.Damage;
using Game.PlayerUnit;
using UnityEngine;

namespace Game.EnemyUnit.Attack.Active
{
    public class EnemyMeleeAttack : EnemyActiveAttack
    {
        [SerializeField] protected DamageEffect _damageEffect;
        [SerializeField] protected EnemyAttackTrigger _attackTrigger;
        [SerializeField] protected float _attackTriggerDuration;
        [Range(1, 3)] [SerializeField] protected int _amountOfParticles;
        [SerializeField] protected GameObject _attackVfx;
        [SerializeField] protected Transform _attackVfxTransform;
        [SerializeField] private TrailRenderer[] _attackTrails;
        
        private const int AttackTriggerFramesToRemainActive = 3;

        public override void Initialize()
        {
            _attackTrigger.Initialize(_damageEffect, _damage, _amountOfParticles);
            _attackTrigger.Disable();
            DeactivateAttackTrails();
        }

        public override float GetAttackDuration()
        {
            return _enemyVisuals.GetAttackAnimationLength();
        }

        protected override IEnumerator AttackChoreography(Player closestPlayer, Vector3 sphereCenter, float sphereRadius)
        {
            if (!_isEnabled)
            {
                yield break;
            }

            CalculateFaceToPlayerParams(closestPlayer.transform.position, sphereCenter, out Vector3 directionToPlayer, out Vector3 upDirection);
            _enemyVisuals.PlayAttackAnimation(directionToPlayer, upDirection, _attackAnimationIndex);
            yield return new WaitForSeconds(_attackStartTime);
            ActivateAttackTrails();
            SpawnAttackVfx();
            _attackTrigger.Enable();
            
            for (int i = 0; i < AttackTriggerFramesToRemainActive; i++)
            {
                yield return new WaitForEndOfFrame();
            }
            
            _attackTrigger.Disable();
            yield return new WaitForSeconds(_attackTriggerDuration);
            DeactivateAttackTrails();
        }

        private void SpawnAttackVfx()
        {
            if (!_attackVfx)
            {
                return;
            }

            _enemyVisuals.SpawnVfx(_attackVfxTransform ? _attackVfxTransform : transform, _attackVfx);
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