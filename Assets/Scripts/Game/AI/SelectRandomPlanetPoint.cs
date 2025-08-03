using Game.EnemyUnit;
using Opsive.BehaviorDesigner.Runtime;
using Opsive.BehaviorDesigner.Runtime.Tasks;
using Opsive.BehaviorDesigner.Runtime.Tasks.Actions;

namespace Game.AI
{
    public class SelectRandomPlanetPoint : Action
    {
        public override TaskStatus OnUpdate()
        {
            GetComponent<BehaviorTree>().SetVariableValue("NavmeshTargetPosition", EnemyManager.Instance.GeneratePointInSurface());
            return TaskStatus.Success;
        }

    }
}
