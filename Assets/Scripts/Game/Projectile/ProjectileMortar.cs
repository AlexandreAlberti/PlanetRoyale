using Game.Damage;
using UnityEngine;

namespace Game.Projectile
{
    [RequireComponent(typeof(ProjectileSphericalMortarMovement))]
    public abstract class ProjectileMortar : ProjectileBase
    {
        [SerializeField] protected GameObject _landingMark;
        [SerializeField] protected ProjectileMortarExplosion _explosionDamagePrefab;
        [SerializeField] protected DamageArea _damageAreaPrefab;
        [Range (0.0f, 10.0f)]
        [SerializeField] protected float _areaOfEffectDamageFactor;

        private ProjectileSphericalMortarMovement _mortarMovement;
        
        protected override void Awake()
        {
            base.Awake();
            _mortarMovement = GetComponent<ProjectileSphericalMortarMovement>();
            
            if (_landingMark)
            {
                _landingMark.SetActive(false);
            }
        }
     
        public override void Initialize(Vector3 position, Vector3 direction, float distanceToTarget, Vector3 sphereCenter, float sphereRadius, int damage, GameObject projectilePrefab, int playerIndex)
        {
            base.Initialize(position, direction, distanceToTarget, sphereCenter, sphereRadius, damage, projectilePrefab, playerIndex);
            _mortarMovement.OnMortarEndedMovement += OnMortarEndedMovement;

            if (_landingMark)
            {
                _landingMark.transform.parent = null;
                _landingMark.transform.position = _targetTransformPosition;
                _landingMark.transform.rotation = _targetTransformRotation;
                _landingMark.SetActive(true);
            }
        }

        public void PlayLandingMarkAnimation(string animationName)
        {
            if (!_landingMark)
            {
                return;
            }

            Animator animator = _landingMark.GetComponent<Animator>();

            if (!animator)
            {
                return;
            }
            
            animator.Play(animationName);
        }
        
        protected virtual void OnMortarEndedMovement(ProjectileSphericalMortarMovement instance)
        {
            if (!_isEnabled)
            {
                return;
            }

            if (_landingMark)
            {
                _landingMark.SetActive(false);
                _landingMark.transform.parent = transform;
            }

            SpawnExplosionDamageAndAreaOfEffect();
            Destroy();
            Disable();
        }

        protected abstract void SpawnExplosionDamageAndAreaOfEffect();
    }
}