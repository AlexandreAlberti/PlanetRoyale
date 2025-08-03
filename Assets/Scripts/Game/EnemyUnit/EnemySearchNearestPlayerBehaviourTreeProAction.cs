namespace Game.EnemyUnit
{
    using UnityEngine;
    using Opsive.BehaviorDesigner.Runtime.Tasks;
    using Opsive.BehaviorDesigner.Runtime.Tasks.Actions;
    using Opsive.GraphDesigner.Runtime;
    using Opsive.GraphDesigner.Runtime.Variables;

    [NodeDescription("Returns the nearest player if any.")]
    public class EnemySearchNearestPlayerBehaviourTreeProAction : Action
    {

        private Enemy _enemy;

        [Tooltip("The variable that should be set.")] [SerializeField]
        protected SharedVariable<GameObject> m_StoreResult;

        public override void OnStart()
        {
            _enemy = GetComponent<Enemy>();
        }

        /// <summary>
        /// Executes the task.
        /// </summary>
        /// <returns>The execution status of the task.</returns>
        public override TaskStatus OnUpdate()
        {
            m_StoreResult.Value = _enemy.GetTargetedPlayerGameObject();
            return TaskStatus.Success;
        }

    }
}