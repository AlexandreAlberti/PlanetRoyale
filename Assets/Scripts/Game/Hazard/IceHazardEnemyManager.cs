using Application.Utils;
using Game.EnemyUnit;
using Game.PlayerUnit;
using UnityEngine;

namespace Game.Hazard
{
    public class IceHazardEnemyManager : EnemyManager
    {
        protected override void EnemyInitialization(Enemy enemyInstance)
        {
            Player player = PlayerManager.Instance.GetHumanPlayer();
            float distanceToPlayer = player.transform.position.DistanceInSphereSurface(enemyInstance.transform.position, _sphereRadius);
            float fogFieldOfView = IceHazardManager.Instance.GetFogFieldOfView();

            if (distanceToPlayer > fogFieldOfView + enemyInstance.GetUnit().GetUnitRadius())
            {
                enemyInstance.GetUnit().HideIceHazardHideableObjects();
            }
        }

        protected override void InitializeRagdoll(EnemyRagdoll enemyRagdollInstance, Vector3 spawnPosition, Vector3 upDirection)
        {
            base.InitializeRagdoll(enemyRagdollInstance, spawnPosition, upDirection);
            
            Player player = PlayerManager.Instance.GetHumanPlayer();
            float distanceToPlayer = player.transform.position.DistanceInSphereSurface(spawnPosition, _sphereRadius);
            float fogFieldOfView = IceHazardManager.Instance.GetFogFieldOfView();

            if (distanceToPlayer > fogFieldOfView)
            {
                enemyRagdollInstance.GetUnitVisuals().HideIceHazardHideableObjects();
            }

        }
    }
}
