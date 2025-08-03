using Application.Data.Progression;
using Game.Damage;
using Game.PlayerUnit;
using UnityEngine;

namespace Game.EnemyUnit.Attack.Passive
{
    [RequireComponent(typeof(Enemy))]
    [RequireComponent(typeof(EnemyVisuals))]
    public class EnemyDamageReflector : EnemyPassiveAttack
    {
        [SerializeField] private DamageType _damageTypeOrigin;
        [Range(0.0f, 1.0f)][SerializeField] private float _reflectedDamagePercentage;
        [SerializeField] private GameObject _reflectOriginVfxPrefab;
        [SerializeField] private GameObject _reflectTargetVfxPrefab;
        
        private Enemy _enemy;
        private EnemyVisuals _enemyVisuals;
        private float _enemyDamageMultiplier;


        private void Awake()
        {
            _enemy = GetComponent<Enemy>();
            _enemyVisuals = GetComponent<EnemyVisuals>();
        }

        public override void Initialize()
        {
            _enemy.OnDamaged -= Enemy_OnDamaged;
            _enemy.OnDamaged += Enemy_OnDamaged;
            _enemyDamageMultiplier = LeaguesManager.Instance.GetCurrentLeagueData().EnemyDamageMultiplier;
        }

        private void Enemy_OnDamaged(int amount, DamageType damageType, int damagerIndex)
        {
            if (!_isEnabled)
            {
                return;
            }

            if (damageType != _damageTypeOrigin)
            {
                return;
            }

            Player targetPlayer = PlayerManager.Instance.GetPlayer(damagerIndex);
            targetPlayer.Damage(Mathf.CeilToInt(amount * _reflectedDamagePercentage * _enemyDamageMultiplier), DamageType.Reflected, DamageEffect.None);
            _enemyVisuals.SpawnVfx(targetPlayer.transform, _reflectTargetVfxPrefab);
            _enemyVisuals.SpawnVfx(transform, _reflectOriginVfxPrefab);
        }
    }
}