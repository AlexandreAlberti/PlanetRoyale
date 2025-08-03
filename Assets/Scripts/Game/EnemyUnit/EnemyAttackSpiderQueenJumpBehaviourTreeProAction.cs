namespace Game.EnemyUnit
{
    public class EnemyAttackSpiderQueenJumpBehaviourTreeProAction : BaseEnemyAttackBehaviourTreeProAction
    {
        protected override void SetAttackIndex()
        {
            _attackIndex = FirstAttackIndex;
        }
    }
}