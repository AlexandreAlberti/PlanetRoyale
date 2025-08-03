namespace Game.EnemyUnit
{
    public class EnemyAttackMagmaElementalFireBreathBehaviourTreeProAction : BaseEnemyAttackBehaviourTreeProAction
    {
        protected override void SetAttackIndex()
        {
            _attackIndex = FirstAttackIndex;
        }
    }
}