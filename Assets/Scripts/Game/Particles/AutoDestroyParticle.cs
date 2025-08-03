using Game.Pool;
using UnityEngine;

namespace Game.Particles
{
    public class AutoDestroyParticle : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _particleSystem;
        
        private GameObject _prefab;

        public void Initialize(GameObject prefab)
        {
            _prefab = prefab;
        }
        
        private void Update()
        {
            if (!_particleSystem.isPlaying)
            {
                ObjectPool.Instance.Release(_prefab, gameObject);
            }
        }
    }
}