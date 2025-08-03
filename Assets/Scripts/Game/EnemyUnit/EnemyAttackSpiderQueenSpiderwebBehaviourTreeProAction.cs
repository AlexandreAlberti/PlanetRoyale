namespace Game.EnemyUnit
{
    public class EnemyAttackSpiderQueenSpiderwebBehaviourTreeProAction : BaseEnemyAttackBehaviourTreeProAction
    {
        protected override void SetAttackIndex()
        {
            _attackIndex = SecondAttackIndex;
        }
    }
}