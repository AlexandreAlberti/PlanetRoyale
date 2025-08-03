namespace Game.EnemyUnit
{
    public class EnemyAttackGiantCobraChargeBehaviourTreeProAction : BaseEnemyAttackBehaviourTreeProAction
    {
        protected override void SetAttackIndex()
        {
            _attackIndex = ThirdAttackIndex;
        }
    }
}