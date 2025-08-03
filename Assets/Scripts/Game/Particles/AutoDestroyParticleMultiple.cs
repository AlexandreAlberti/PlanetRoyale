using Game.Pool;
using UnityEngine;

namespace Game.Particles
{
    public class AutoDestroyParticleMultiple : MonoBehaviour
    {
        [SerializeField] private ParticleSystem[] _particleSystems;
        
        private GameObject _prefab;

        public void Initialize(GameObject prefab)
        {
            _prefab = prefab;
        }
        
        private void Update()
        {
            foreach (ParticleSystem particle in _particleSystems)
            {
                if (particle.isPlaying)
                {
                    return;
                }
            }
            
            ObjectPool.Instance.Release(_prefab, gameObject);
        }
    }
}