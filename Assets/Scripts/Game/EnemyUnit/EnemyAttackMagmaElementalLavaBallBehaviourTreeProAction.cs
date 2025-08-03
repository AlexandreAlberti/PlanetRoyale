namespace Game.EnemyUnit
{
    public class EnemyAttackMagmaElementalLavaBallBehaviourTreeProAction : BaseEnemyAttackBehaviourTreeProAction
    {
        protected override void SetAttackIndex()
        {
            _attackIndex = SecondAttackIndex;
        }
    }
}