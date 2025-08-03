using System.Collections;
using System.Collections.Generic;
using Game.Pool;
using UnityEngine;

namespace Game.Drop
{
    public class DropManager : MonoBehaviour
    {
        [Header("Hp Drops")]
        [Range(0.0f, 1.0f)][SerializeField] private float _hpDropSpawnChance;
        [Range(0.0f, 1.0f)][SerializeField] private float _hpDropHealingPercentage;
        [SerializeField] private HpDropConfig _hpDropConfig;
        [Header("Xp Drops")]
        [SerializeField] private XpDropConfig[] _xpDrops;
        [SerializeField] private float _scatteredTagsDelayBetweenDrops;
        [Min(1)]
        [SerializeField] private int _minAmountOfDroppedTags;

        public static DropManager Instance;

        private Dictionary<XpDropType, XpDrop> _xpDropDictionary;

        private void Awake()
        {
            Instance = this;
        }

        public void Initialize()
        {
            _xpDropDictionary = new Dictionary<XpDropType, XpDrop>();

            foreach (XpDropConfig tagConfig in _xpDrops)
            {
                _xpDropDictionary.Add(tagConfig.XpDropType, tagConfig.XpDropPrefab);
                ObjectPool.Instance.RegisterPrefab(tagConfig.XpDropPrefab.gameObject, tagConfig.XpDropPrewarmCount);
                ObjectPool.Instance.RegisterPrefab(tagConfig.XpDropCollectedPrefab.gameObject, tagConfig.XpDropPrewarmCount);
                ObjectPool.Instance.RegisterPrefab(tagConfig.XpDropCollectedVfx, tagConfig.XpDropPrewarmCount);
            }
            
            ObjectPool.Instance.RegisterPrefab(_hpDropConfig.HpDropPrefab.gameObject, _hpDropConfig.HpDropPrewarmCount);
            ObjectPool.Instance.RegisterPrefab(_hpDropConfig.HpDropCollectedPrefab.gameObject, _hpDropConfig.HpDropPrewarmCount);
            ObjectPool.Instance.RegisterPrefab(_hpDropConfig.HpDropCollectedVfx, _hpDropConfig.HpDropPrewarmCount);
        }

        public void SpawnHpDrop(Vector3 position, Quaternion rotation, Vector3 upDirection)
        {
            GameObject hpDropInstance = ObjectPool.Instance.GetPooledObject(_hpDropConfig.HpDropPrefab.gameObject, position, rotation);
            hpDropInstance.GetComponent<HpDrop>().Initialize(_hpDropConfig.HpDropPrefab.gameObject, upDirection, true);
        }
        
        private void SpawnXpDrop(XpDropType xpDropType, Vector3 position, Quaternion rotation, Vector3 upDirection, bool useRandomScatteredMovement)
        {
            XpDrop xpDropPrefab = _xpDropDictionary[xpDropType];
            GameObject xpDropInstance = ObjectPool.Instance.GetPooledObject(xpDropPrefab.gameObject, position, rotation);
            xpDropInstance.GetComponent<XpDrop>().Initialize(xpDropPrefab.gameObject, upDirection, useRandomScatteredMovement);
        }

        public IEnumerator GiveXpDropPrefabs(int totalTagsXp, Vector3 position, Quaternion rotation, Vector3 upDirection, bool trySingleSpawn = true)
        {
            if (totalTagsXp <= 0)
            {
                yield break;
            }

            if (trySingleSpawn)
            {
                yield return StartCoroutine(MinAmountOfTagsSpawn(totalTagsXp, position, rotation, upDirection));
            }
            else
            {
                yield return StartCoroutine(MinDropOfTagsSpawn(totalTagsXp, position, rotation, upDirection));
            }
        }

        private IEnumerator MinDropOfTagsSpawn(int totalTagsXp, Vector3 position, Quaternion rotation, Vector3 upDirection)
        {
            int tagXpToGive = 0;
            XpDropType xpDropType = XpDropType.Type001Xp;
            int dropsXp = Mathf.FloorToInt((float)totalTagsXp / _minAmountOfDroppedTags);

            foreach (KeyValuePair<XpDropType, XpDrop> tagEntry in _xpDropDictionary)
            {
                int tagXp = tagEntry.Value.TagXp();

                if (tagXp > tagXpToGive && tagXp <= dropsXp)
                {
                    tagXpToGive = tagXp;
                    xpDropType = tagEntry.Key;
                }
            }

            int remainingXp = totalTagsXp - tagXpToGive * _minAmountOfDroppedTags;
            bool haveRemainingXpToGive = remainingXp > 0;

            for (int i = 0; i < _minAmountOfDroppedTags; i++)
            {
                yield return new WaitForSeconds(_scatteredTagsDelayBetweenDrops);
                SpawnXpDrop(xpDropType, position, rotation, upDirection, true);
            }
            
            if (haveRemainingXpToGive)
            {
                yield return new WaitForSeconds(_scatteredTagsDelayBetweenDrops);
                yield return StartCoroutine(MinAmountOfTagsSpawn(remainingXp, position, rotation, upDirection));
            }
        }

        private IEnumerator MinAmountOfTagsSpawn(int totalTagsXp, Vector3 position, Quaternion rotation, Vector3 upDirection)
        {
            int tagXpToGive = 0;
            XpDropType xpDropType = XpDropType.Type001Xp;

            foreach (KeyValuePair<XpDropType, XpDrop> tagEntry in _xpDropDictionary)
            {
                int tagXp = tagEntry.Value.TagXp();

                if (tagXp > tagXpToGive && tagXp <= totalTagsXp)
                {
                    tagXpToGive = tagXp;
                    xpDropType = tagEntry.Key;
                }
            }

            int remainingXp = totalTagsXp - tagXpToGive;
            bool haveRemainingXpToGive = remainingXp > 0;
            SpawnXpDrop(xpDropType, position, rotation, upDirection, haveRemainingXpToGive);

            if (haveRemainingXpToGive)
            {
                yield return new WaitForSeconds(_scatteredTagsDelayBetweenDrops);
                yield return StartCoroutine(MinAmountOfTagsSpawn(remainingXp, position, rotation, upDirection));
            }
        }

        public float GetHpDropSpawnChance()
        {
            return _hpDropSpawnChance;
        }

        public float GetHpDropHealingPercentage()
        {
            return _hpDropHealingPercentage;
        }

        public void Disable()
        {
            foreach (Drop drop in FindObjectsOfType<Drop>())
            {
                drop.Disable();
            }
            
            foreach (DropCollected dropCollected in FindObjectsOfType<DropCollected>())
            {
                dropCollected.Disable();
            }
        }
    }
}