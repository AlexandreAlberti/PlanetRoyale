using System.Collections;
using Game.PlayerUnit;
using UnityEngine;

namespace Game.EnemyUnit.Attack.Active
{
    [RequireComponent(typeof(EnemyVisuals))]
    public class EnemyMeleeConeAttack : EnemyMeleeAttack
    {
        [SerializeField] protected GameObject _coneEffect;
        [SerializeField] protected float _coneScale;

        protected override void Awake()
        {
            base.Awake();
            _attackTrigger.transform.localScale = Vector3.one * _coneScale;
            _attackTrigger.transform.localScale = new Vector3(_attackTrigger.transform.localScale.x, 1.0f, _attackTrigger.transform.localScale.z);
            _coneEffect.SetActive(false);
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
            
            _attackTrigger.Enable();
            _coneEffect.SetActive(true);
            
            yield return new WaitForSeconds(_attackTriggerDuration);
            _attackTrigger.Disable();
            _coneEffect.SetActive(false);
        }
    }
}