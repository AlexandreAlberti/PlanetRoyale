using System.Collections;
using Game.Pool;
using UnityEngine;

namespace Game.Vfx
{
    public class AutoDestroyPooledObject : MonoBehaviour
    {
        [SerializeField] private float _autoDestroyTime;

        private GameObject _prefab;
        private Coroutine _releaseToPoolCoroutine;

        public void Initialize(GameObject prefab)
        {
            _prefab = prefab;
            _releaseToPoolCoroutine = StartCoroutine(WaitAndReleaseFromPool());
        }

        private IEnumerator WaitAndReleaseFromPool()
        {
            yield return new WaitForSeconds(_autoDestroyTime);
            ObjectPool.Instance.Release(_prefab, gameObject);
        }

        public void CancelAutoDestroy()
        {
            if (_releaseToPoolCoroutine != null)
            {
                StopCoroutine(_releaseToPoolCoroutine);
            }
        }

        public void ForceAutoDestroyTime(float time)
        {
            _autoDestroyTime = time;
        }
    }
}