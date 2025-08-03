using System;

namespace Game.EnemyUnit
{
    [Serializable]
    public struct PooledEnemyConfig
    {
        public Enemy EnemyPrefab;
        public int EnemyPrewarmCount;
        public EnemyRagdoll EnemyRagdollPrefab;
        public int EnemyRagdollPrewarmCount;
    }
}