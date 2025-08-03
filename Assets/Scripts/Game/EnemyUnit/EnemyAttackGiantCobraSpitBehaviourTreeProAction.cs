namespace Game.EnemyUnit
{
    public class EnemyAttackGiantCobraSpitBehaviourTreeProAction : BaseEnemyAttackBehaviourTreeProAction
    {
        protected override void SetAttackIndex()
        {
            _attackIndex = SecondAttackIndex;
        }
    }
}