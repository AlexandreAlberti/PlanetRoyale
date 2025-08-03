using System.Collections.Generic;
using Game.BaseUnit;
using Game.EnemyUnit;

namespace Game.Detectors
{
    public abstract class EnemyDetector : UnitDetector
    {
        protected override void TargetUnit()
        {
            if (!_closestUnit)
            {
                return;
            }

            _closestUnit.GetComponent<Enemy>().TargetEnemy();
        }

        public Enemy GetClosestEnemy()
        {
            if (!_closestUnit)
            {
                return null;
            }

            return _closestUnit.GetComponent<Enemy>();
        }

        protected override List<Unit> GetUnitsList()
        {
            List<Unit> units = new List<Unit>();

            foreach (Enemy enemy in EnemyManager.Instance.GetEnemies())
            {
                if (!enemy)
                {
                    continue;
                }
                
                units.Add(enemy.GetUnit());
            }

            return units;
        }
    }
}