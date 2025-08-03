namespace Game.EnemyUnit
{
    public class EnemyAttackBehaviourTreeProAction : BaseEnemyAttackBehaviourTreeProAction
    {
        protected override void SetAttackIndex()
        {
            _attackIndex = FirstAttackIndex;
        }
    }
}