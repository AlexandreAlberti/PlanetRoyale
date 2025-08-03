using System.Collections;
using System.Collections.Generic;
using DamageNumbersPro;
using Game.Camera;
using Game.Pool;
using Game.UI;
using Game.Vfx;
using UnityEngine;

namespace Game.BaseUnit
{
    [RequireComponent(typeof(UnitDamageFlash))]
    public class UnitVisuals : MonoBehaviour
    {
        [SerializeField] private Renderer[] _unitRenderers;
        [SerializeField] private HealthBar _healthBar;
        [SerializeField] private GameObject _deadVfx;
        [SerializeField] private DamageNumber _damageParticlePrefab;
        [SerializeField] private DamageNumber _healParticlePrefab;
        [SerializeField] private DamageNumber _criticalParticlePrefab;
        [SerializeField] private DamageNumber _reflectedDamageParticlePrefab;
        [SerializeField] private DamageNumber _poisonDamageParticlePrefab;
        [SerializeField] private DamageNumber _dodgeParticlePrefab;
        [SerializeField] private GameObject _poisonIndicator;
        [SerializeField] private GameObject _slowdownIndicator;
        [SerializeField] private Transform _damageParticleSpawnPoint;
        [Range(0, 1)] 
        [SerializeField] private float _damageParticleMultipleSpawnDelay;
        [SerializeField] private GameObject[] _iceHazardHideableObjects;
        
        private UnitDamageFlash _unitDamageFlash;
        private List<Material> _originalMaterials;
        private bool _damageParticlesWarmedUp;
        private bool _canShowDeathAnimation;

        private void Awake()
        {
            _unitDamageFlash = GetComponent<UnitDamageFlash>();
            SaveOriginalRendererMaterials();
            StartCoroutine(InitializeDamageParticlesPool());
        }

        public void Initialize(int currentHealth)
        {
            _canShowDeathAnimation = true;
            _healthBar.Initialize(currentHealth);
            SaveOriginalRendererMaterials();
            _unitDamageFlash.Initialize();
            HidePoisonedStatus();
            HideSlowdownStatus();
            EnableHealthBar();
        }

        private IEnumerator InitializeDamageParticlesPool()
        {
            if (_damageParticlesWarmedUp)
            {
                yield break;
            }

            _damageParticlesWarmedUp = true;
            yield return new WaitForEndOfFrame();
            _damageParticlePrefab.PrewarmPool();
        }

        private void SaveOriginalRendererMaterials()
        {
            _originalMaterials = new List<Material>();

            for (int i = 0; i < _unitRenderers.Length; i++)
            {
                for (int j = 0; j < _unitRenderers[i].materials.Length; j++)
                {
                    _originalMaterials.Add(_unitRenderers[i].materials[j]);
                }
            }
        }

        public void EnableHealthBar()
        {
            _healthBar.gameObject.SetActive(true);
        }

        public void DisableHealthBar()
        {
            _healthBar.gameObject.SetActive(false);
        }

        public void EnableDamageFlash()
        {
            _unitDamageFlash.Flash();
        }

        public void ChangeRenderersMaterial(Material material)
        {
            foreach (Renderer unitRenderer in _unitRenderers)
            {
                Material[] materials = unitRenderer.materials;

                for (int i = 0; i < materials.Length; i++)
                {
                    materials[i] = material;
                }

                unitRenderer.materials = materials;
            }
        }

        public void RestoreOriginalRenderersMaterial()
        {
            int materialCount = 0;

            for (int i = 0; i < _unitRenderers.Length; i++)
            {
                Material[] materials = _unitRenderers[i].materials;

                for (int j = 0; j < materials.Length; j++)
                {
                    materials[j] = _originalMaterials[materialCount++];
                }

                _unitRenderers[i].materials = materials;
            }
        }

        public void SpawnDamageTextParticle(int damageAmount, int amountOfParticles)
        {
            if (amountOfParticles <= 1)
            {
                SpawnSimpleDamageParticle(damageAmount);
                return;
            }

            StartCoroutine(SpawnMultipleDamageParticles(damageAmount, amountOfParticles));
        }

        private IEnumerator SpawnMultipleDamageParticles(int damageAmount, int amountOfParticles)
        {
            int damageAlreadySpawnInParticles = 0;
            int averageAmount = damageAmount / amountOfParticles;

            for (int i = 1; i < amountOfParticles; i++)
            {
                SpawnSimpleDamageParticle(averageAmount);
                damageAlreadySpawnInParticles += averageAmount;
                yield return new WaitForSeconds(_damageParticleMultipleSpawnDelay);
            }

            SpawnSimpleDamageParticle(damageAmount - damageAlreadySpawnInParticles);
            yield return null;
        }

        private void SpawnSimpleDamageParticle(int damageAmount)
        {
            if (damageAmount == 0)
            {
                return;
            }
            
            DamageNumber damageNumberPartial = _damageParticlePrefab.Spawn(_damageParticleSpawnPoint.position, damageAmount);
            damageNumberPartial.cameraOverride = CameraManager.Instance.Camera().transform;
        }

        public void SpawnHealTextParticle(int healAmount)
        {
            if (healAmount == 0)
            {
                return;
            }

            DamageNumber damageNumber = _healParticlePrefab.Spawn(_damageParticleSpawnPoint.position, healAmount);
            damageNumber.cameraOverride = CameraManager.Instance.Camera().transform;
        }

        public void SpawnCriticalTextParticle(int damageAmount)
        {
            if (damageAmount == 0)
            {
                return;
            }
            
            DamageNumber damageNumber = _criticalParticlePrefab.Spawn(_damageParticleSpawnPoint.position, damageAmount);
            damageNumber.cameraOverride = CameraManager.Instance.Camera().transform;
        }

        public void SpawnReflectedDamageTextParticle(int damageAmount)
        {
            if (damageAmount == 0)
            {
                return;
            }
            
            DamageNumber damageNumber = _reflectedDamageParticlePrefab.Spawn(_damageParticleSpawnPoint.position, damageAmount, _damageParticleSpawnPoint);
            damageNumber.cameraOverride = CameraManager.Instance.Camera().transform;
        }

        public void SpawnPoisonDamageTextParticle(int damageAmount)
        {
            if (damageAmount == 0)
            {
                return;
            }

            DamageNumber damageNumber = _poisonDamageParticlePrefab.Spawn(_damageParticleSpawnPoint.position, damageAmount, _damageParticleSpawnPoint);
            damageNumber.cameraOverride = CameraManager.Instance.Camera().transform;
        }

        public void PlayDeathAnimation()
        {
            SpawnDeadVfx();
        }

        private void SpawnDeadVfx()
        {
            if (!_canShowDeathAnimation)
            {
                return;
            }

            GameObject ragdollInstance = ObjectPool.Instance.GetPooledObject(_deadVfx, transform.position, transform.rotation);
            ragdollInstance.GetComponent<UnitRagdoll>().PlayDeathAnimation();
            ragdollInstance.GetComponent<AutoDestroyPooledObject>().Initialize(_deadVfx);
        }

        public void DisableDeathAnimation()
        {
            _canShowDeathAnimation = false;
        }

        public void ShowPoisonedStatus()
        {
            _poisonIndicator.SetActive(true);
        }

        public void HidePoisonedStatus()
        {
            _poisonIndicator.SetActive(false);
        }

        public void ShowSlowdownStatus()
        {
            _slowdownIndicator.SetActive(true);
        }

        public void HideSlowdownStatus()
        {
            _slowdownIndicator.SetActive(false);
        }

        public void SpawnDodgeTextParticle()
        {
            DamageNumber dodgeNumberPartial = _dodgeParticlePrefab.Spawn(_damageParticleSpawnPoint.position, _damageParticleSpawnPoint);
            dodgeNumberPartial.cameraOverride = CameraManager.Instance.Camera().transform;
        }

        public void HideIceHazardHideableObjects()
        {
            foreach (GameObject hideableObject in _iceHazardHideableObjects)
            {
                hideableObject.SetActive(false);
            }
        }
        
        public void ShowIceHazardHideableObjects()
        {
            foreach (GameObject hideableObject in _iceHazardHideableObjects)
            {
                hideableObject.SetActive(true);
            }
        }
    }
}