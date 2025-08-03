namespace Game.EnemyUnit
{
    public class EnemyAttackRunicGolemDoublePunchBehaviourTreeProAction : BaseEnemyAttackBehaviourTreeProAction
    {
        protected override void SetAttackIndex()
        {
            _attackIndex = ThirdAttackIndex;
        }
    }
}