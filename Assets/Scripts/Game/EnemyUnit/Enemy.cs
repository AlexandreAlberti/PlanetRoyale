using System;
using Application.Data.Progression;
using Application.Sound;
using Game.AI;
using Game.BaseUnit;
using Game.Damage;
using Game.Detectors;
using Game.Drop;
using Game.Enabler;
using Game.EnemyUnit.Attack;
using Game.Ftue;
using Game.PlayerUnit;
using UnityEngine;

namespace Game.EnemyUnit
{
    [RequireComponent(typeof(Unit))]
    [RequireComponent(typeof(PlayerDetector))]
    [RequireComponent(typeof(EnemyVisuals))]
    [RequireComponent(typeof(EnemyAttackManager))]
    public class Enemy : EnablerMonoBehaviour
    {
        [SerializeField] private int _tagXp;
        [SerializeField] private AIMovement _aiMovement;

        private Unit _unit;
        private EnemyVisuals _enemyVisuals;
        private PlayerDetector _playerDetector;
        public Action<int, DamageType, int> OnDamaged;
        public Action<Enemy, int> OnDead;
        private Enemy _enemyPrefab;
        private int _creatorInstanceId;
        private EnemyAttackManager _enemyAttackManager;
        private bool _enableAiMovement;
        private bool _isBoss;
        private bool _isXpGiveEnabled;
        private Tier _tier;

        private void Awake()
        {
            _unit = GetComponent<Unit>();
            _enemyVisuals = GetComponent<EnemyVisuals>();
            _playerDetector = GetComponent<PlayerDetector>();
            _enemyAttackManager = GetComponent<EnemyAttackManager>();
        }

        private void Update()
        {
            if (!_isEnabled)
            {
                return;
            }

            _enemyVisuals.SetSpeed(_aiMovement.GetCurrentSpeed());
        }

        private void Unit_OnDead(Unit unit, int heroIndex)
        {
            _enemyVisuals.DisableTargetedMark();
            _unit.OnDead -= Unit_OnDead;
            SoundManager.Instance.PlayEnemyDeathSound();
            gameObject.SetActive(false);
            OnDead?.Invoke(this, heroIndex);
            Disable();

            Player player = PlayerManager.Instance.GetPlayer(heroIndex);
            player.IncreaseKillCount();

            if (_isXpGiveEnabled)
            {
                player.GainXp(_tagXp);
            }
        }

        public void Initialize(Vector3 sphereCenter, float sphereRadius, Enemy enemyPrefab, int creatorInstanceId, bool doInitializeWithAiMovement, bool isBoss, Tier tier)
        {
            _unit.Initialize(healthMultiplier: LeaguesManager.Instance.GetCurrentLeagueData().EnemyHealthMultiplier);
            _unit.Disable();
            _unit.OnDead += Unit_OnDead;
            _unit.OnDamaged -= Unit_OnDamaged;
            _unit.OnDamaged += Unit_OnDamaged;
            _enableAiMovement = doInitializeWithAiMovement;
            _aiMovement.Initialize(sphereCenter, sphereRadius);
            _enemyVisuals.Initialize();
            _enemyVisuals.DisableTargetedMark();
            _playerDetector.Initialize(false, _unit.GetCenterAnchor());
            _enemyPrefab = enemyPrefab;
            _creatorInstanceId = creatorInstanceId;
            _enemyAttackManager.Initialize(sphereCenter, sphereRadius);
            _isBoss = isBoss;
            _tier = tier;
            _isXpGiveEnabled = true;
            Enable();
        }

        public void SetTier(Tier tier)
        {
            _tier = tier;
        }

        public Tier GetTier()
        {
            return _tier;
        }
        
        private void Unit_OnDamaged(int amount, int currentHealth, int maxHealth, DamageType damageType, int damagerIndex)
        {
            OnDamaged?.Invoke(amount, damageType, damagerIndex);
        }

        public override void Enable()
        {
            base.Enable();
            _unit.Enable();
            _enemyAttackManager.Enable();
            
            if (_enableAiMovement)
            {
                _aiMovement.Enable();
            }
        }

        public override void Disable()
        {
            base.Disable();
            _unit.Disable();
            _aiMovement.Disable();
            _enemyAttackManager.Disable();
        }

        public void Damage(int damage, DamageType damageType, DamageEffect damageEffect, int damagerIndex)
        {
            if (!_isEnabled)
            {
                return;
            }

            SoundManager.Instance.PlayEnemyHitSound();

            if (IsBoss() && damagerIndex != Player.GetHumanPlayerIndex() && FtueManager.Instance.HasClearedControlsTutorialLevel() && !FtueManager.Instance.HasClearedMatchTutorialLevel())
            {
                return;
            }
            
            _unit.Damage(damage, damageType, damageEffect, damagerIndex);
        }

        public void TargetEnemy()
        {
            _enemyVisuals.EnableTargetedMark();
        }

        public void UnTargetEnemy()
        {
            _enemyVisuals.DisableTargetedMark();
        }

        public Transform GetCenterAnchor()
        {
            return _unit.GetCenterAnchor();
        }

        public GameObject GetTargetedPlayerGameObject()
        {
            return _playerDetector.GetClosestPlayerGameObject();
        }

        public Enemy GetEnemyPrefab()
        {
            return _enemyPrefab;
        }

        public int GetCreatorInstanceId()
        {
            return _creatorInstanceId;
        }

        public int GetNavmeshAgentAreaMask()
        {
            return _aiMovement.GetNavmeshAgentAreaMask();
        }

        public void MakeInvincible()
        {
            _unit.MakeInvincible();
        }

        public void RemoveInvincible()
        {
            _unit.RemoveInvincible();
        }

        public void PushBack(float pushBackForce, Vector3 pushBackDirection)
        {
            if (_unit.IsAlive())
            {
                _aiMovement.PushBack(pushBackForce, pushBackDirection);
            }
        }

        public int TagXp()
        {
            return _tagXp;
        }

        public Unit GetUnit()
        {
            return _unit;
        }

        public bool IsBoss()
        {
            return _isBoss;
        }

        public void DisableXpOnDeath()
        {
            _isXpGiveEnabled = false;
        }
    }
}
