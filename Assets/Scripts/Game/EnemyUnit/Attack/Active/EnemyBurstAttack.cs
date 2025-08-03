using System.Collections;
using Game.PlayerUnit;
using UnityEngine;

namespace Game.EnemyUnit.Attack.Active
{
    public class EnemyBurstAttack : EnemyMeleeAttack
    {
        [SerializeField] protected float _burstRadius;
        [SerializeField] private GameObject _burstGroundVfxPrefab;
        
        public override void Initialize()
        {
            base.Initialize();
            _attackTrigger.SetScale(_burstRadius);
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
            SpawnBurstGroundVfx();
            yield return new WaitForSeconds(_attackTriggerDuration);
            _attackTrigger.Disable();
        }

        protected void SpawnBurstGroundVfx()
        {
            if (!_burstGroundVfxPrefab)
            {
                return;
            }
            
            GameObject vfxInstance = _enemyVisuals.SpawnVfx(transform, _burstGroundVfxPrefab);
            vfxInstance.transform.localScale = Vector3.one * _burstRadius * 2;
        }
    }
}