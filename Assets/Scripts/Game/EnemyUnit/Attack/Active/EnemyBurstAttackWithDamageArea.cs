using System.Collections;
using Game.Damage;
using Game.PlayerUnit;
using Game.Pool;
using Game.Vfx;
using UnityEngine;

namespace Game.EnemyUnit.Attack.Active
{
    public class EnemyBurstAttackWithDamageArea : EnemyBurstAttack
    {
        [SerializeField] private PlayerDamageArea _damageAreaPrefab;
        [SerializeField] private int _damageAreaDamage;

        private Vector3 _scaleToUse;

        public override void Initialize()
        {
            base.Initialize();
            _scaleToUse = Vector3.one * (_burstRadius * 2);
        }

        protected override IEnumerator AttackChoreography(Player closestPlayer, Vector3 sphereCenter, float sphereRadius)
        {
            if (!_isEnabled)
            {
                yield break;
            }

            CalculateFaceToPlayerParams(closestPlayer.transform.position, sphereCenter, out Vector3 directionToPlayer, out Vector3 upDirection);
            _enemyVisuals.PlayAttackAnimation(directionToPlayer, upDirection, _attackAnimationIndex);
            SpawnAttackVfx(_scaleToUse, upDirection);
            yield return new WaitForSeconds(_attackStartTime);
            _attackTrigger.Enable();
            SpawnBurstGroundVfx();
            SpawnDamageArea();

            yield return new WaitForSeconds(_attackTriggerDuration);
            _attackTrigger.Disable();
        }

        private void SpawnAttackVfx(Vector3 scaleToUse, Vector3 upDirection)
        {
            if (!_attackVfx)
            {
                return;
            }

            _enemyVisuals.SpawnVfx(_attackVfxTransform ? _attackVfxTransform : transform, _attackVfx, scaleToUse, upDirection);
        }

        private void SpawnDamageArea()
        {
            if (!_isEnabled)
            {
                return;
            }

            GameObject damageAreaInstance = ObjectPool.Instance.GetPooledObject(_damageAreaPrefab.gameObject, transform.position, transform.rotation);
            damageAreaInstance.GetComponent<AutoDestroyPooledObject>().Initialize(_damageAreaPrefab.gameObject);
            damageAreaInstance.GetComponent<PlayerDamageArea>().Initialize(_damageAreaDamage);
            damageAreaInstance.transform.localScale = _scaleToUse;
        }
    }
}