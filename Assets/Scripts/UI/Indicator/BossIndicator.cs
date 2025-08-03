using Game.EnemyUnit;
using UnityEngine;

namespace UI.Indicator
{
    public class BossIndicator : BaseIndicator
    {
        [SerializeField] private int _bossIndex;
        [SerializeField] private bool _isMiniBoss;
        
        protected override Transform Target()
        {
            return _isMiniBoss ? EnemyManager.Instance.GetMiniBossTransform(_bossIndex) : EnemyManager.Instance.GetBossTransform(_bossIndex);
        }
    }
}