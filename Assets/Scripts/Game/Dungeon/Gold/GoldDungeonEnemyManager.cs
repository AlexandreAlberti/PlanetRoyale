using System.Collections;
using System.Collections.Generic;
using Application.Data.Progression;
using Application.Data.Rewards;
using Game.Drop;
using Game.EnemyUnit;
using UnityEngine;

namespace Game.Dungeon.Gold
{
    public class GoldDungeonEnemyManager : EnemyManager
    {
        public override IEnumerator SpawnEnemyChoreography(Vector3 spawnPosition, Enemy enemyPrefab, EnemyRagdoll enemyRagdollPrefab, EnemyMarkSpawner spawnMarkVfxPrefab, EnemyRaySpawner spawnRayVfxPrefab, Tier tier,
            List<Enemy> enemyList = null, int creatorInstanceId = 0, bool isBoss = false, bool isMinion = false, bool useSpawnerVisuals = true, bool doInitializeWithAiMovement = true)
        {
            return base.SpawnEnemyChoreography(spawnPosition, enemyPrefab, enemyRagdollPrefab, spawnMarkVfxPrefab, spawnRayVfxPrefab, tier, enemyList, creatorInstanceId, isBoss, isMinion, false, doInitializeWithAiMovement);
        }

        protected override void ItemDrops(Enemy enemy)
        {
            GoldDropType goldDropType = GoldDropType.Small;
            int goldDropAmount = 0;
            LeagueData leagueData = LeaguesManager.Instance.GetCurrentLeagueData();
            GoldDungeonData goldDungeonData = leagueData.GoldDungeonRewardsData;

            switch (enemy.GetTier())
            {
                case Tier.Tier1:
                    goldDropAmount = goldDungeonData.GoldAmountForTier1Enemies;
                    break;
                case Tier.Tier2:
                    goldDropType = GoldDropType.Medium;
                    goldDropAmount = goldDungeonData.GoldAmountForTier2Enemies;
                    break;
                case Tier.Tier3:
                    goldDropType = GoldDropType.Large;
                    goldDropAmount = goldDungeonData.GoldAmountForTier3Enemies;
                    break;
                case Tier.None:
                    return;
            }

            GoldDropManager.Instance.GiveGoldDrop(goldDropType, goldDropAmount, enemy.transform.position, enemy.transform.rotation, enemy.transform.up);
        }
        
        // protected override void EnemyInitialization(Enemy enemyInstance)
        // {
        //     base.EnemyInitialization(enemyInstance);
        //     enemyInstance.DisableXpOnDeath();
        // }
    }
}