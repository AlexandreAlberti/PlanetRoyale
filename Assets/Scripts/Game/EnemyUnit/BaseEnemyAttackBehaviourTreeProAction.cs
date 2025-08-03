using Game.EnemyUnit.Attack;
using Opsive.BehaviorDesigner.Runtime.Tasks;
using Opsive.BehaviorDesigner.Runtime.Tasks.Actions;
using UnityEngine;

namespace Game.EnemyUnit
{
    public abstract class BaseEnemyAttackBehaviourTreeProAction : Action
    {
        private EnemyAttackManager _enemyAttackManager;
        private bool _isAttacking;
        private float _attackingTimer;
        private float _totalAttackDuration;
        protected int _attackIndex;

        protected const int FirstAttackIndex = 0;
        protected const int SecondAttackIndex = 1;
        protected const int ThirdAttackIndex = 2;
        
        public override void OnStart()
        {
            SetAttackIndex();
            _enemyAttackManager = GetComponent<EnemyAttackManager>();
            _isAttacking = false;
            _attackingTimer = 0.0f;
            _totalAttackDuration = _enemyAttackManager.GetActiveAttackDurationByIndex(_attackIndex);
        }

        protected abstract void SetAttackIndex();

        public override TaskStatus OnUpdate()
        {
            if (!_isAttacking)
            {
                _isAttacking = true;
                _enemyAttackManager.UseActiveAttackByIndex(_attackIndex);
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