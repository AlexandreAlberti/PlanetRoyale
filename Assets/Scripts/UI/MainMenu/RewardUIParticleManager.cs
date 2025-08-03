using System;
using System.Collections;
using System.Collections.Generic;
using Application.Data.Equipment;
using Application.Sound;
using Game.Pool;
using UnityEngine;

namespace UI.MainMenu
{
    public class RewardUIParticleManager : MonoBehaviour
    {
        [SerializeField] private float _growDirectionSpeed;
        [SerializeField] private float _growTime;
        [SerializeField] private float _travelTime;
        [SerializeField] private RewardUIParticle _particle;
        [SerializeField] private Sprite _coinParticleSprite;
        [SerializeField] private Sprite _gemParticleSprite;
        [SerializeField] private Sprite _silverKeyParticleSprite;
        [SerializeField] private Sprite _goldenKeyParticleSprite;
        [SerializeField] private Sprite _trophyParticleSprite;
        [SerializeField] private Color _particleSpriteMaterialColor;

        public static RewardUIParticleManager Instance { get; private set; }

        private const int MaxEquipmentParticles = 10;
        private const int MaxMaterialParticles = 20;
        private const int MaxGoldParticles = 30;
        private const int MaxGemsParticles = 10;
        private const int MaxSilverKeyParticles = 4;
        private const int MaxGoldenKeyParticles = 2;
        private const int ParticlesSoundFrequency = 4;

        private const float TimeBetweenParticles = 0.02f;
        private const float GoldParticleGroup = 100.0f;
        private const float TrophiesParticleGroup = 5.0f;
        private const float GemsParticleGroup = 5.0f;

        public Action OnGoldParticleReached;
        public Action OnGemParticleReached;
        public Action OnKeyParticleReached;
        public Action OnTrophyParticleReached;
        public Action OnMaterialOrEquipmentParticleReached;

        private int _goldParticlesToCreate;
        private int _gemsParticlesToCreate;
        private int _silverKeysParticlesToCreate;
        private int _goldenKeysParticlesToCreate;
        private int _trophies;
        private int _trophiesParticlesToCreate;
        private List<EquipmentPieceType> _materialParticlesToCreate;
        private List<int> _equipmentParticlesToCreate;
        private List<RewardUIParticle> _createdGoldParticles;
        private List<RewardUIParticle> _createdGemParticles;
        private List<RewardUIParticle> _createdKeyParticles;
        private List<RewardUIParticle> _createdMaterialOrEquipmentParticles;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                _goldParticlesToCreate = 0;
                _gemsParticlesToCreate = 0;
                _silverKeysParticlesToCreate = 0;
                _goldenKeysParticlesToCreate = 0;
                _trophies = 0;
                _trophiesParticlesToCreate = 0;
                _materialParticlesToCreate = new List<EquipmentPieceType>();
                _equipmentParticlesToCreate = new List<int>();
                _createdGoldParticles = new List<RewardUIParticle>();
                _createdGemParticles = new List<RewardUIParticle>();
                _createdKeyParticles = new List<RewardUIParticle>();
                _createdMaterialOrEquipmentParticles = new List<RewardUIParticle>();
                DontDestroyOnLoad(this);
            }
        }

        public void AddRewardsForNextMainMenuScreen(int goldGained, int gemsGained, int silverKeysGained, int goldenKeysGained, List<int> equipmentsGained, List<EquipmentPieceType> materialsGained, int trophies)
        {
            _goldParticlesToCreate = Math.Min(MaxGoldParticles, _goldParticlesToCreate + Mathf.CeilToInt(goldGained / GoldParticleGroup));
            _trophies += trophies;
            _trophiesParticlesToCreate = Math.Min(MaxGoldParticles, _trophiesParticlesToCreate + Mathf.CeilToInt(trophies / TrophiesParticleGroup));
            _gemsParticlesToCreate = Math.Min(MaxGemsParticles, _gemsParticlesToCreate + Mathf.CeilToInt(gemsGained / GemsParticleGroup));
            _silverKeysParticlesToCreate = Math.Min(MaxSilverKeyParticles, _silverKeysParticlesToCreate + silverKeysGained);
            _goldenKeysParticlesToCreate = Math.Min(MaxGoldenKeyParticles, _goldenKeysParticlesToCreate + goldenKeysGained);
            
            for (int i = 0; i < equipmentsGained.Count && _equipmentParticlesToCreate.Count < MaxEquipmentParticles - 1; i++)
            {
                _equipmentParticlesToCreate.Add(equipmentsGained[i]);
            }

            for (int i = 0; i < materialsGained.Count && _materialParticlesToCreate.Count < MaxMaterialParticles - 1; i++)
            {
                _materialParticlesToCreate.Add(materialsGained[i]);
            }
        }

        public IEnumerator CreateEquipmentParticles(Vector3 particlesOrigin, Vector3 materialsDestination)
        {
            List<int> equipmentParticlesToCreate = new List<int>(_equipmentParticlesToCreate);
            _equipmentParticlesToCreate = new List<int>();

            foreach (int equipmentId in equipmentParticlesToCreate)
            {
                RewardUIParticle equipmentParticle = SpawnParticle(EquipmentDataManager.Instance.GetEquipmentPieceData(equipmentId).Sprite, particlesOrigin, materialsDestination, false);
                equipmentParticle.OnParticleReached += RewardUIParticle_OnMaterialOrEquipmentParticleReached;
                _createdMaterialOrEquipmentParticles.Add(equipmentParticle);
                yield return new WaitForSeconds(TimeBetweenParticles);
            }
        }

        public IEnumerator CreateMaterialParticles(Vector3 particlesOrigin, Vector3 materialsDestination)
        {
            List<EquipmentPieceType> materialParticlesToCreate = new List<EquipmentPieceType>(_materialParticlesToCreate);
            _materialParticlesToCreate = new List<EquipmentPieceType>();

            foreach (EquipmentPieceType materialSprite in materialParticlesToCreate)
            {
                RewardUIParticle materialParticle = SpawnParticle(MaterialsSpritesManager.Instance.GetSprite(materialSprite), particlesOrigin, materialsDestination, true);
                materialParticle.OnParticleReached += RewardUIParticle_OnMaterialOrEquipmentParticleReached;
                _createdMaterialOrEquipmentParticles.Add(materialParticle);
                yield return new WaitForSeconds(TimeBetweenParticles);
            }
        }

        public IEnumerator CreateTrophiesParticles(Vector3 particlesOrigin, Vector3 trophiesDestination)
        {
            int trophiesParticlesToCreate = _trophiesParticlesToCreate;
            _trophiesParticlesToCreate = 0;
            _trophies = 0;

            for (int i = 0; i < trophiesParticlesToCreate; ++i)
            {
                RewardUIParticle trophyParticle = SpawnParticle(_trophyParticleSprite, particlesOrigin, trophiesDestination, false);
                trophyParticle.OnParticleReached += RewardUIParticle_OnTrophyParticleReached;

                if (i % ParticlesSoundFrequency == 0)
                {
                    SoundManager.Instance.PlayRewardSound();
                }

                yield return new WaitForSeconds(TimeBetweenParticles);
            }
        }

        public IEnumerator CreateGoldParticles(Vector3 particlesOrigin, Vector3 coinsDestination)
        {
            int goldParticlesToCreate = _goldParticlesToCreate;
            _goldParticlesToCreate = 0;
            
            for (int i = 0; i < goldParticlesToCreate; ++i)
            {
                RewardUIParticle goldParticle = SpawnParticle(_coinParticleSprite, particlesOrigin, coinsDestination, false);
                goldParticle.OnParticleReached += RewardUIParticle_OnGoldParticleReached;
                _createdGoldParticles.Add(goldParticle);
                    
                if (i % ParticlesSoundFrequency == 0)
                {
                    SoundManager.Instance.PlayRewardSound();
                }

                yield return new WaitForSeconds(TimeBetweenParticles);
            }
        }

        public IEnumerator CreateGemsParticles(Vector3 particlesOrigin, Vector3 gemsDestination)
        {
            int gemsParticlesToCreate = _gemsParticlesToCreate;
            _gemsParticlesToCreate = 0;
                
            for (int i = 0; i < gemsParticlesToCreate; ++i)
            {
                RewardUIParticle gemParticle = SpawnParticle(_gemParticleSprite, particlesOrigin, gemsDestination, false);
                gemParticle.OnParticleReached += RewardUIParticle_OnGemParticleReached;
                _createdGemParticles.Add(gemParticle);
                    
                if (i % ParticlesSoundFrequency == 0)
                {
                    SoundManager.Instance.PlayRewardSound();
                }

                yield return new WaitForSeconds(TimeBetweenParticles);
            }
        }

        public IEnumerator CreateKeysParticles(Vector3 particlesOrigin, Vector3 keysDestination)
        {
            int silverKeysParticlesToCreate = _silverKeysParticlesToCreate;
            int goldenKeysParticlesToCreate = _goldenKeysParticlesToCreate;
            _silverKeysParticlesToCreate = 0;
            _goldenKeysParticlesToCreate = 0;
            
            for (int i = 0; i < silverKeysParticlesToCreate; ++i)
            {
                RewardUIParticle keyParticle = SpawnParticle(_silverKeyParticleSprite, particlesOrigin, keysDestination, false);
                keyParticle.OnParticleReached += RewardUIParticle_OnKeyParticleReached;
                _createdKeyParticles.Add(keyParticle);
                    
                if (i % ParticlesSoundFrequency == 0)
                {
                    SoundManager.Instance.PlayRewardSound();
                }

                yield return new WaitForSeconds(TimeBetweenParticles);
            }

            for (int i = 0; i < goldenKeysParticlesToCreate; ++i)
            {
                RewardUIParticle keyParticle = SpawnParticle(_goldenKeyParticleSprite, particlesOrigin, keysDestination, false);
                keyParticle.OnParticleReached += RewardUIParticle_OnKeyParticleReached;
                _createdKeyParticles.Add(keyParticle);
                    
                if (i % ParticlesSoundFrequency == 0)
                {
                    SoundManager.Instance.PlayRewardSound();
                }

                yield return new WaitForSeconds(TimeBetweenParticles);
            }
        }

        public int TrophiesWonAmount()
        {
            return _trophies;
        }
        
        public void ForceTrophiesWonAmount(int amount)
        {
            _trophies = amount;
        }

        public void ForceTrophiesParticles(int amount)
        {
            _trophiesParticlesToCreate = amount;
        }

        private void RewardUIParticle_OnGoldParticleReached(RewardUIParticle particle)
        {
            particle.OnParticleReached -= RewardUIParticle_OnGoldParticleReached;
            OnGoldParticleReached?.Invoke();
        }

        private void RewardUIParticle_OnGemParticleReached(RewardUIParticle particle)
        {
            particle.OnParticleReached -= RewardUIParticle_OnGemParticleReached;
            OnGemParticleReached?.Invoke();
        }

        private void RewardUIParticle_OnKeyParticleReached(RewardUIParticle particle)
        {
            particle.OnParticleReached -= RewardUIParticle_OnKeyParticleReached;
            OnKeyParticleReached?.Invoke();
        }

        private void RewardUIParticle_OnTrophyParticleReached(RewardUIParticle particle)
        {
            particle.OnParticleReached -= RewardUIParticle_OnTrophyParticleReached;
            OnTrophyParticleReached?.Invoke();
        }

        private void RewardUIParticle_OnMaterialOrEquipmentParticleReached(RewardUIParticle particle)
        {
            particle.OnParticleReached -= RewardUIParticle_OnMaterialOrEquipmentParticleReached;
            OnMaterialOrEquipmentParticleReached?.Invoke();
        }

        private RewardUIParticle SpawnParticle(Sprite spriteToUse, Vector3 initialPosition, Vector3 finalPosition, bool isMaterial)
        {
            RewardUIParticle newQuestParticle = ObjectPool.Instance.GetPooledObject(
                _particle.gameObject,
                transform.position,
                Quaternion.identity).GetComponent<RewardUIParticle>();
            newQuestParticle.transform.position = initialPosition;
            newQuestParticle.InitializeParticle(_particle, spriteToUse, _growDirectionSpeed, _growTime, finalPosition, _travelTime, isMaterial, _particleSpriteMaterialColor);
            return newQuestParticle;
        }

        public bool HasGoldParticlesToCreate()
        {
            return _goldParticlesToCreate > 0;
        }
        
        public bool HasGemsParticlesToCreate()
        {
            return _gemsParticlesToCreate > 0;
        }
        
        public bool HasKeyParticlesToCreate()
        {
            return _silverKeysParticlesToCreate > 0 || _goldenKeysParticlesToCreate > 0;
        }

        public bool HasMaterialParticlesToCreate()
        {
            return _materialParticlesToCreate.Count > 0;
        }
        
        public bool HasEquipmentParticlesToCreate()
        {
            return _equipmentParticlesToCreate.Count > 0;
        }
        
        public bool HasSomethingToReward()
        {
            return _goldParticlesToCreate > 0 || _gemsParticlesToCreate > 0 || _silverKeysParticlesToCreate > 0 || _goldenKeysParticlesToCreate > 0 || _trophies != 0 || _materialParticlesToCreate.Count > 0 || _equipmentParticlesToCreate.Count > 0;
        }

        public void RemoveAllParticles()
        {
            _createdGoldParticles.ForEach(RemoveParticleFromPool);
            _createdGoldParticles.Clear();
            _createdGemParticles.ForEach(RemoveParticleFromPool);
            _createdGemParticles.Clear();
            _createdKeyParticles.ForEach(RemoveParticleFromPool);
            _createdKeyParticles.Clear();
            _createdMaterialOrEquipmentParticles.ForEach(RemoveParticleFromPool);
            _createdMaterialOrEquipmentParticles.Clear();
        }

        private void RemoveParticleFromPool(RewardUIParticle particle)
        {
            if (particle.gameObject.activeInHierarchy)
            {
                particle.OnParticleReached -= RewardUIParticle_OnMaterialOrEquipmentParticleReached;
                particle.OnParticleReached -= RewardUIParticle_OnGoldParticleReached;
                particle.OnParticleReached -= RewardUIParticle_OnGemParticleReached;
                particle.ReleaseParticleFromPool();
            }
        }

        public float TrophiesDelayBeforeReachingTarget()
        {
            return _travelTime + _growTime;
        }
    }
}