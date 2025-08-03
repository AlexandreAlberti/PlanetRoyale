using System.Collections;
using Game.EnemyUnit;
using UnityEngine;

namespace Game.Projectile
{
    public class ProjectileMortarSpiderQueenEnemy : ProjectileMortarEnemy
    {
        [SerializeField] private EnemyVisuals _enemyVisuals;
        
        protected override void OnMortarEndedMovement(ProjectileSphericalMortarMovement instance)
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
            _enemyVisuals.PlayLandAnimation();
            StartCoroutine(WaitAndDestroy());
        }

        private IEnumerator WaitAndDestroy()
        {
            yield return new WaitForSeconds(0.15f);
            
            Destroy();
            Disable();
        }
    }
}