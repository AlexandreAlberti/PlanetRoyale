namespace Game.EnemyUnit
{
    public class EnemyAttackRunicGolemRockSlabBehaviourTreeProAction : BaseEnemyAttackBehaviourTreeProAction
    {
        protected override void SetAttackIndex()
        {
            _attackIndex = FirstAttackIndex;
        }
    }
}