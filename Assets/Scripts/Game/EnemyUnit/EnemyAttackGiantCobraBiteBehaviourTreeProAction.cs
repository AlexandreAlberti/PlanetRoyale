namespace Game.EnemyUnit
{
    public class EnemyAttackGiantCobraBiteBehaviourTreeProAction : BaseEnemyAttackBehaviourTreeProAction
    {
        protected override void SetAttackIndex()
        {
            _attackIndex = FirstAttackIndex;
        }
    }
}