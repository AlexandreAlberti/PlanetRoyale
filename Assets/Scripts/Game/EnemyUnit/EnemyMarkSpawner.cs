using UnityEngine;

namespace Game.EnemyUnit
{
    public class EnemyMarkSpawner : MonoBehaviour
    {
        [SerializeField] private Animator _markAnimator;
        [SerializeField] private AnimationClip _enemySpawnAnimation;

        public void Initialize()
        {
            _markAnimator.Rebind();
        }

        public float GetSpawnDuration()
        {
            return _enemySpawnAnimation.length;
        }
    }
}
