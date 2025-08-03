using Game.EnemyUnit;
using UnityEngine;

namespace Game.Map
{
    public class EnemySpawnPoint : MonoBehaviour
    {
        [SerializeField] private int _waveIndex;
        [SerializeField] private Enemy _enemyPrefabToSpawn;
        [SerializeField] private EnemyRagdoll _enemyRagdollPrefabToSpawn;
        
        public int WaveIndex()
        {
            return _waveIndex;
        }

        public Enemy EnemyPrefab()
        {
            return _enemyPrefabToSpawn;
        }
        
        public EnemyRagdoll EnemyRagdollPrefab()
        {
            return _enemyRagdollPrefabToSpawn;
        }
    }
}