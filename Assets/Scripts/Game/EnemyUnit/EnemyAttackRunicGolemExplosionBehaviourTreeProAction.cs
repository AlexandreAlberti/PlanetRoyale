namespace Game.EnemyUnit
{
    public class EnemyAttackRunicGolemExplosionBehaviourTreeProAction : BaseEnemyAttackBehaviourTreeProAction
    {
        protected override void SetAttackIndex()
        {
            _attackIndex = SecondAttackIndex;
        }
    }
}