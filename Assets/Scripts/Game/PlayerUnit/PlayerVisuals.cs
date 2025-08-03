using Game.Pool;
using Game.Ranking;
using Game.Vfx;
using TMPro;
using UnityEngine;

namespace Game.PlayerUnit
{
    public class PlayerVisuals : MonoBehaviour
    {
        [SerializeField] private Transform _visuals;
        [SerializeField] private Animator _animator;
        [SerializeField] private GameObject _levelUpVfx;
        [SerializeField] private Transform _detectionCircle;
        [SerializeField] private float _detectionCircleSafeUpFactor;
        [SerializeField] private GameObject _crownIcon;
        [SerializeField] private Transform[] _itemsAffectedByAttackRangeIncrease;
        [SerializeField] private TextMeshProUGUI _playerName;
        [SerializeField] private GameObject _bossPowerVfx;
        [SerializeField] private Transform _directionalArrow;
        
        private static readonly int AttackTrigger = Animator.StringToHash("Attack");
        private static readonly int LevelUpTrigger = Animator.StringToHash("LevelUp");
        private static readonly int DeathState = Animator.StringToHash("Death");
        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int BossPowerTrigger = Animator.StringToHash("BossPower");

        private float _sphereRadius;
        private int _playerIndex;
        private float _initialAttackRange;
        private bool _isDirectionalArrowEnabled;
        private Vector3 _directionalArrowFollowPosition;
        
        public void Initialize(float sphereRadius, float playerDetectedDistance, int playerIndex, string playerName)
        {
            _playerIndex = playerIndex;
            _sphereRadius = sphereRadius;
            _initialAttackRange = playerDetectedDistance;
            IncrementAttackRange(playerDetectedDistance);
            _crownIcon.SetActive(_playerIndex == Player.GetHumanPlayerIndex());
            RankingManager.Instance.OnScoreChanged += RankingManager_OnScoreChanged;
            _playerName.text = playerName;
            _isDirectionalArrowEnabled = false;
            _directionalArrow.gameObject.SetActive(false);
        }

        private void RankingManager_OnScoreChanged()
        {
            _crownIcon.SetActive(RankingManager.Instance.GetRankByPlayerIndex(_playerIndex) == 0);
        }

        private void UpdateDirectionalArrowAngle(Vector3 forwardDirection, Vector3 upDirection, float visualsSignedAngle)
        {
            Vector3 directionToFollowTarget = (_directionalArrowFollowPosition - transform.position).normalized;
            float signedAngle = Vector3.SignedAngle(forwardDirection, directionToFollowTarget, upDirection) - visualsSignedAngle;
            _directionalArrow.localRotation = Quaternion.AngleAxis(signedAngle, Vector3.up);
        }
        
        public void UpdateMoveDirection(Vector3 forwardDirection, Vector3 movementDirection, Vector3 upDirection)
        {
            float signedAngle = Vector3.SignedAngle(forwardDirection, movementDirection, upDirection);
            _visuals.localRotation = Quaternion.AngleAxis(signedAngle, Vector3.up);
            
            if (_isDirectionalArrowEnabled)
            {
                UpdateDirectionalArrowAngle(forwardDirection, upDirection, signedAngle);
            }
        }

        public void PlayAttackAnimation(Vector3 forwardDirection, Vector3 directionToEnemy, Vector3 upDirection)
        {
            float signedAngle = Vector3.SignedAngle(forwardDirection, directionToEnemy, upDirection);
            _visuals.localRotation = Quaternion.AngleAxis(signedAngle, Vector3.up);
            _animator.SetTrigger(AttackTrigger);
        }

        public void PlayDieAnimation()
        {
            _animator.Play(DeathState);
        }

        public void SetSpeed(float speed)
        {
            _animator.SetInteger(Speed, (int)speed);
        }

        public void IncrementAttackRange(float newAttackRangeRadius)
        {
            if (_sphereRadius < newAttackRangeRadius)
            {
                return;
            }
            
            _detectionCircle.localPosition = new Vector3(0.0f, DetectionCircleVerticalOffsetBasedOnCircleRadius(newAttackRangeRadius), 0.0f);
            float newAttackRange = 2 * newAttackRangeRadius;
            _detectionCircle.localScale = new Vector3(newAttackRange, newAttackRange, newAttackRange);
            
            foreach (Transform itemToScale in _itemsAffectedByAttackRangeIncrease)
            {
                float newScale = newAttackRangeRadius / _initialAttackRange;
                itemToScale.localScale = new Vector3(newScale, newScale, newScale);
            }
        }

        private float DetectionCircleVerticalOffsetBasedOnCircleRadius(float radius)
        {
            return _detectionCircleSafeUpFactor * -(_sphereRadius - Mathf.Sqrt(_sphereRadius * _sphereRadius - radius * radius));
        }

        public void LevelUp()
        {
            _animator.SetTrigger(LevelUpTrigger);
            GameObject vfxInstance = ObjectPool.Instance.GetPooledObject(_levelUpVfx, transform.position, transform.rotation);
            vfxInstance.transform.SetParent(transform);
            vfxInstance.transform.localPosition = Vector3.zero;
            vfxInstance.GetComponent<AutoDestroyPooledObject>().Initialize(_levelUpVfx);
        }

        public void ActivateBossPower()
        {
            _animator.ResetTrigger(LevelUpTrigger);
            _animator.SetTrigger(BossPowerTrigger);
            _bossPowerVfx.SetActive(true);
        }

        public void DeactivateBossPower()
        {
            _bossPowerVfx.SetActive(false);
        }

        public void EnableDirectionalArrow(Vector3 targetPosition)
        {
            _isDirectionalArrowEnabled = true;
            _directionalArrow.gameObject.SetActive(true);
            _directionalArrowFollowPosition = targetPosition;
        }

        public void DisableDirectionalArrow()
        {
            _isDirectionalArrowEnabled = false;
            _directionalArrow.gameObject.SetActive(false);
        }
    }
}