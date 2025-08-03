using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Game.Pool
{
    public class ObjectPool : MonoBehaviour
    {
        [SerializeField] List<PoolConfigObject> _pooledPrefabsList;
        
        private Dictionary<GameObject, ObjectPool<GameObject>> _pooledObjects;
        
        public static ObjectPool Instance;

        void Awake()
        {
            Instance = this;
        }

        public void Initialize()
        {
            _pooledObjects = new Dictionary<GameObject, ObjectPool<GameObject>>();
            
            foreach (PoolConfigObject configObject in _pooledPrefabsList)
            {
                RegisterPrefabInternal(configObject.Prefab, configObject.PrewarmCount);
            }
        }

        public void RegisterPrefab(GameObject prefab, int prewarmCount)
        {
            RegisterPrefabInternal(prefab, prewarmCount);
        }
        
        void RegisterPrefabInternal(GameObject prefab, int prewarmCount)
        {
            if (_pooledObjects.ContainsKey(prefab))
            {
                return;
            }
            
            GameObject CreateFunction()
            {
                return Instantiate(prefab);
            }

            void ActionOnGet(GameObject instance)
            {
                instance.SetActive(true);
            }

            void ActionOnRelease(GameObject instance)
            {
                instance.SetActive(false);
            }

            void ActionOnDestroy(GameObject instance)
            {
                Destroy(instance);
            }

            _pooledObjects[prefab] = new ObjectPool<GameObject>(CreateFunction, ActionOnGet, ActionOnRelease, ActionOnDestroy, defaultCapacity: prewarmCount);

            List<GameObject> prewarmNetworkObjects = new List<GameObject>();
            
            for (var i = 0; i < prewarmCount; i++)
            {
                GameObject pooledObj = _pooledObjects[prefab].Get();
                string holder = pooledObj.name + "Holder";
                Transform holderForPooledObject = transform.Find(holder);

                if (!holderForPooledObject)
                {
                    holderForPooledObject = new GameObject().transform;
                    holderForPooledObject.name = holder;
                    holderForPooledObject.SetParent(transform);
                }
                
                pooledObj.transform.SetParent(holderForPooledObject);
                prewarmNetworkObjects.Add(pooledObj);
            }
            
            foreach (GameObject networkObject in prewarmNetworkObjects)
            {
                _pooledObjects[prefab].Release(networkObject);
            }
        }
        
        public GameObject GetPooledObject(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            _pooledObjects[prefab].Get(out GameObject instance);
            instance.transform.position = position;
            instance.transform.rotation = rotation;
            string holder = instance.name + "Holder";
            Transform holderForPooledObject = transform.Find(holder);

            if (!holderForPooledObject)
            {
                holderForPooledObject = new GameObject().transform;
                holderForPooledObject.name = holder;
                holderForPooledObject.SetParent(transform);
            }
                
            instance.transform.SetParent(holderForPooledObject);
            return instance;
        }

        public void Release(GameObject prefab, GameObject instance)
        {
            _pooledObjects[prefab].Release(instance);
        }
    }
}
