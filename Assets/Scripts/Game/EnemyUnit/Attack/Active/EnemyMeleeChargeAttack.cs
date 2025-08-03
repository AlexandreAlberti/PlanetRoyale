using System.Collections;
using Game.AI;
using Game.PlayerUnit;
using Game.Pool;
using Game.Vfx;
using UnityEngine;

namespace Game.EnemyUnit.Attack.Active
{
    [RequireComponent(typeof(EnemySphericalMovement))]
    public class EnemyMeleeChargeAttack : EnemyMeleeAttack 
    {
        [SerializeField] private AIMovement _aiMovement;
        [SerializeField] private ParticleSystem _windGustVfx;
        [SerializeField] private GameObject _attackArea;
        [SerializeField] private GameObject _chargeTrailVfx;

        
        private EnemySphericalMovement _enemySphericalMovement;

        protected override void Awake()
        {
            base.Awake();
            _enemySphericalMovement = GetComponent<EnemySphericalMovement>();
            _attackArea.SetActive(false);
            _chargeTrailVfx.SetActive(false);
            _windGustVfx.gameObject.SetActive(false);
        }
        
        protected override IEnumerator AttackChoreography(Player closestPlayer, Vector3 sphereCenter, float sphereRadius)
        {
            if (!_isEnabled)
            {
                yield break;
            }

            CalculateFaceToPlayerParams(closestPlayer.transform.position, sphereCenter, out Vector3 directionToPlayer, out Vector3 upDirection);
            _enemyVisuals.PlayAttackAnimation(directionToPlayer, upDirection, _attackAnimationIndex);
            _aiMovement.Disable();
            _attackArea.SetActive(true);
            yield return new WaitForSeconds(_attackStartTime);
            _attackArea.SetActive(false);
            _chargeTrailVfx.SetActive(true);
            _attackTrigger.Enable();
            _enemySphericalMovement.OnCollisionFound += EndCharge;
            _enemySphericalMovement.Initialize(directionToPlayer, sphereCenter);
            yield return new WaitForSeconds(_attackTriggerDuration);
            _attackChoreography = null;
            EndCharge();
        }

        private void EndCharge()
        {
            if (_attackChoreography != null)
            {
                StopCoroutine(_attackChoreography);
            }
            
            _enemySphericalMovement.OnCollisionFound -= EndCharge;
            _enemyVisuals.PlayIdleAnimation();
            _attackTrigger.Disable();
            _chargeTrailVfx.SetActive(false);
            _enemySphericalMovement.Disable();
            _aiMovement.Enable();
            SpawnWindGustVfx();
        }

        private void SpawnWindGustVfx()
        {
            _windGustVfx.gameObject.SetActive(true);
            _windGustVfx.Play();
            StartCoroutine(WaitAndDisableWindGustVfxCoroutine());
        }

        private IEnumerator WaitAndDisableWindGustVfxCoroutine()
        {
            yield return new WaitForSeconds(_windGustVfx.main.duration);
            _windGustVfx.gameObject.SetActive(false);
        }
    }
}