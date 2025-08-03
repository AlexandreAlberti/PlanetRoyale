using Opsive.GraphDesigner.Runtime;
using Opsive.GraphDesigner.Runtime.Variables;
using UnityEngine;

#if GRAPH_DESIGNER
/// ---------------------------------------------
/// Behavior Designer
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------
namespace Opsive.BehaviorDesigner.Runtime.Tasks.Conditionals
{
    [NodeDescription("Returns true if the specified object is inside range.")]
    public class IsCloseToTarget : Conditional
    {
        [Tooltip("The variable to check Distance.")]
        [SerializeField] protected SharedVariable<GameObject> m_Target;
        [Tooltip("Max Distance to consider it in range.")]
        [SerializeField] protected SharedVariable<float> m_Distance;

        /// <summary>
        /// Executes the task.
        /// </summary>
        /// <returns>The execution status of the task.</returns>
        public override TaskStatus OnUpdate()
        {
            object variableValue = null;
            if (m_Target == null || (variableValue = m_Target.GetValue()) == null) {
                return TaskStatus.Failure;
            }
            if (m_Distance == null || (variableValue = m_Distance.GetValue()) == null) {
                return TaskStatus.Failure;
            }
            
            return Vector3.Distance(m_Target.Value.transform.position, transform.position) < m_Distance.Value ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}
#endif