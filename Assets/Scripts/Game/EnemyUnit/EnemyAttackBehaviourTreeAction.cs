using BehaviorDesigner.Runtime.Tasks;
using Game.EnemyUnit.Attack;
using UnityEngine;

namespace Game.EnemyUnit
{
    public class EnemyAttackBehaviourTreeAction : Action
    {
        private EnemyAttackManager _enemyAttackManager;
        private bool _isAttacking;
        private float _attackingTimer;
        private float _totalAttackDuration;

        private const int MainAttackIndex = 0;
        
        public override void OnStart()
        {
            _enemyAttackManager = GetComponent<EnemyAttackManager>();
            _isAttacking = false;
            _attackingTimer = 0.0f;
            _totalAttackDuration = _enemyAttackManager.GetActiveAttackDurationByIndex(MainAttackIndex);
        }

        public override TaskStatus OnUpdate()
        {
            if (!_isAttacking)
            {
                _isAttacking = true;
                _enemyAttackManager.UseActiveAttackByIndex(MainAttackIndex);
            }

            _attackingTimer += Time.deltaTime;

            if (_attackingTimer >= _totalAttackDuration)
            {
                return TaskStatus.Success;
            }
            
            return TaskStatus.Running;
        }
    }
}