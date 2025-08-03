using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawnerWithPooling : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject enemyPrefab;
    public float enemyLifetime = 5f;
    public float respawnDelay = 3f;
    public int poolSize = 5;

    private Queue<GameObject> enemyPool = new Queue<GameObject>();
    private GameObject currentEnemy;

    void Start()
    {
        // Initialize the pool
        for (int i = 0; i < poolSize; i++)
        {
            GameObject enemy = Instantiate(enemyPrefab, transform); // parented to spawner
            enemy.SetActive(false);
            enemyPool.Enqueue(enemy);
        }

        StartCoroutine(SpawnEnemyLoop());
    }

    IEnumerator SpawnEnemyLoop()
    {
        while (true)
        {
            currentEnemy = GetEnemyFromPool();
            currentEnemy.transform.SetParent(transform); // re-parent just in case
            currentEnemy.transform.localPosition = Vector3.zero;
            currentEnemy.transform.localRotation = Quaternion.identity;
            currentEnemy.SetActive(true);

            yield return new WaitForSeconds(enemyLifetime);

            ReturnEnemyToPool(currentEnemy);
            currentEnemy = null;

            yield return new WaitForSeconds(respawnDelay);
        }
    }

    GameObject GetEnemyFromPool()
    {
        if (enemyPool.Count > 0)
        {
            return enemyPool.Dequeue();
        }
        else
        {
            GameObject newEnemy = Instantiate(enemyPrefab, transform);
            newEnemy.SetActive(false);
            return newEnemy;
        }
    }

    void ReturnEnemyToPool(GameObject enemy)
    {
        enemy.SetActive(false);
        enemyPool.Enqueue(enemy);
    }
}
