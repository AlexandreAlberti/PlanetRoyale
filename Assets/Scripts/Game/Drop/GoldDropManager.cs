using System.Collections.Generic;
using Game.Pool;
using UnityEngine;

namespace Game.Drop
{
    public class GoldDropManager : MonoBehaviour
    {
        [Header("Gold Drops")]
        [SerializeField] private GoldDropConfig[] _goldDrops;

        public static GoldDropManager Instance;

        private Dictionary<GoldDropType, GoldDrop> _goldDropDictionary;

        private void Awake()
        {
            Instance = this;
        }

        public void Initialize()
        {
            _goldDropDictionary = new Dictionary<GoldDropType, GoldDrop>();

            foreach (GoldDropConfig goldDropConfig in _goldDrops)
            {
                _goldDropDictionary.Add(goldDropConfig.GoldDropType, goldDropConfig.GoldDropPrefab);
                ObjectPool.Instance.RegisterPrefab(goldDropConfig.GoldDropPrefab.gameObject, goldDropConfig.GoldDropPrewarmCount);
                ObjectPool.Instance.RegisterPrefab(goldDropConfig.GoldDropCollectedPrefab.gameObject, goldDropConfig.GoldDropPrewarmCount);
                ObjectPool.Instance.RegisterPrefab(goldDropConfig.GoldDropCollectedVfx, goldDropConfig.GoldDropPrewarmCount);
            }
        }
        
        public void GiveGoldDrop(GoldDropType goldDropType, int goldDropAmount, Vector3 position, Quaternion rotation, Vector3 upDirection)
        {
            if (goldDropAmount == 0)
            {
                return;
            }
            
            GoldDrop goldDropPrefab = _goldDropDictionary[goldDropType];
            GameObject goldDropInstance = ObjectPool.Instance.GetPooledObject(goldDropPrefab.gameObject, position, rotation);
            goldDropInstance.GetComponent<GoldDrop>().Initialize(goldDropPrefab.gameObject, upDirection, goldDropAmount);
        }
        
        public void Disable()
        {
            foreach (GoldDrop drop in FindObjectsOfType<GoldDrop>())
            {
                drop.Disable();
            }
            
            foreach (GoldDropCollected dropCollected in FindObjectsOfType<GoldDropCollected>())
            {
                dropCollected.Disable();
            }
        }
    }
}