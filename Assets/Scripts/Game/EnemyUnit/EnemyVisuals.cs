using System.Collections;
using Game.Pool;
using Game.Vfx;
using UnityEngine;

namespace Game.EnemyUnit
{
    public class EnemyVisuals : MonoBehaviour
    {
        [SerializeField] private Transform _visuals;
        [SerializeField] private AnimationClip _appearAnimation;
        [SerializeField] private AnimationClip _attackAnimation;
        [SerializeField] private GameObject _enemyTargetedMark;
        [SerializeField] private GameObject _dieVfxPrefab;

        private static readonly int IdleState = Animator.StringToHash("Idle");
        private static readonly int HiddenState = Animator.StringToHash("Hidden");
        private static readonly int AppearTrigger = Animator.StringToHash("Appear");
        private static readonly int AttackTrigger = Animator.StringToHash("Attack");
        private static readonly int Attack2Trigger = Animator.StringToHash("Attack2");
        private static readonly int Attack3Trigger = Animator.StringToHash("Attack3");
        private static readonly int DeathTrigger = Animator.StringToHash("Death");
        private static readonly int JumpTrigger = Animator.StringToHash("Jump");
        private static readonly int LandTrigger = Animator.StringToHash("Land");
        private static readonly int Speed = Animator.StringToHash("Speed");
        private const float AutoAimRotationDuration = 0.4f;

        private Animator _animator;

        private void Awake()
        {
            _animator = _visuals.GetComponentInChildren<Animator>();
        }

        public void Initialize()
        {
        }

        public void PlayAppearAnimation()
        {
            _animator.SetTrigger(AppearTrigger);
        }

        public void PlayAttackAnimation(Vector3 directionToEnemy, Vector3 upDirection, int attackIndex = 0)
        {
            StartCoroutine(ProgressiveTurn(transform.rotation, directionToEnemy, upDirection, AutoAimRotationDuration));
            PlayAttackAnimation(attackIndex);
        }

        private IEnumerator ProgressiveTurn(Quaternion startRotation, Vector3 directionToEnemy, Vector3 upDirection, float duration)
        {
            Quaternion lookRotation = Quaternion.LookRotation(directionToEnemy, upDirection);
            float timer = 0.0f;

            while (timer < duration)
            {
                transform.rotation = Quaternion.Slerp(startRotation, lookRotation, timer / duration);
                timer += Time.deltaTime;
                yield return null;
            }

            transform.rotation = lookRotation;
        }

        private void PlayAttackAnimation(int attackIndex = 0)
        {
            switch (attackIndex)
            {
                case 0:
                    _animator.SetTrigger(AttackTrigger);
                    break;
                case 1:
                    _animator.SetTrigger(Attack2Trigger);
                    break;
                case 2:
                    _animator.SetTrigger(Attack3Trigger);
                    break;
            }
        }

        public void PlayDieAnimation()
        {
            _animator.SetTrigger(DeathTrigger);
            
            if (!_dieVfxPrefab)
            {
                return;
            }

            SpawnVfx(transform, _dieVfxPrefab);
        }

        public GameObject SpawnVfx(Transform target, GameObject vfxPrefab)
        {
            return SpawnVfx(target, vfxPrefab, Vector3.one, Vector3.zero);
        }

        public GameObject SpawnVfx(Transform target, GameObject vfxPrefab, Vector3 scale, Vector3 upDirection)
        {
            GameObject vfxInstance = ObjectPool.Instance.GetPooledObject(vfxPrefab, target.position, target.rotation);
            vfxInstance.GetComponent<AutoDestroyPooledObject>().Initialize(vfxPrefab);
            vfxInstance.transform.localScale = scale;

            if (upDirection != Vector3.zero)
            {
                vfxInstance.transform.up = upDirection;
            }
            
            return vfxInstance;
        }

        public void SetSpeed(float speed)
        {
            _animator.SetInteger(Speed, (int)speed);
        }

        public float GetAppearAnimationLength()
        {
            return _appearAnimation.length;
        }

        public float GetAttackAnimationLength()
        {
            return _attackAnimation.length;
        }

        public void EnableTargetedMark()
        {
            _enemyTargetedMark.SetActive(true);
        }

        public void DisableTargetedMark()
        {
            _enemyTargetedMark.SetActive(false);
        }

        public void PlayIdleAnimation()
        {
            _animator.Play(IdleState);
        }

        public void DisableVisuals()
        {
            _visuals.gameObject.SetActive(false);
        }

        public void EnableVisuals()
        {
            _visuals.gameObject.SetActive(true);
        }
        
        public void PlayJumpAnimation()
        {
            _animator.SetTrigger(JumpTrigger);
        }

        public void PlayLandAnimation()
        {
            _animator.SetTrigger(LandTrigger);
        }
    }
}