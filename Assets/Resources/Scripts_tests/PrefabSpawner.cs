using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabSpawner : MonoBehaviour
{
    [Header("Prefab Settings")]
    [Tooltip("The prefab to spawn")]
    public GameObject prefabToSpawn;

    [Tooltip("Speed at which spawned objects will move forward")]
    public float movementSpeed = 5.0f;

    [Header("Spawn Settings")]
    [Tooltip("Time in seconds between spawns")]
    public float spawnFrequency = 2.0f;

    [Tooltip("Distance from spawn point at which prefabs are destroyed")]
    public float destructionDistance = 10.0f;

    [Tooltip("Whether to start spawning automatically")]
    public bool autoStart = true;

    // Reference to the active coroutine
    private Coroutine spawnCoroutine;

    // List to keep track of spawned objects
    private List<SpawnedObjectInfo> spawnedObjects = new List<SpawnedObjectInfo>();

    // Flag to track if we were spawning before being disabled
    private bool wasSpawningBeforeDisable = false;

    // Structure to hold information about each spawned object
    private class SpawnedObjectInfo
    {
        public GameObject gameObject;
        public Vector3 spawnDirection;

        public SpawnedObjectInfo(GameObject obj, Vector3 direction)
        {
            gameObject = obj;
            spawnDirection = direction;
        }
    }

    void Start()
    {
        if (autoStart)
        {
            StartSpawning();
        }
    }

    void OnEnable()
    {
        // Resume spawning if it was active before being disabled
        if (wasSpawningBeforeDisable)
        {
            StartSpawning();
        }
    }

    void OnDisable()
    {
        // Remember if we were spawning before being disabled
        wasSpawningBeforeDisable = spawnCoroutine != null;

        // Stop the spawning coroutine
        StopSpawning();
    }

    void Update()
    {
        // Move and check if any spawned objects need to be destroyed
        for (int i = spawnedObjects.Count - 1; i >= 0; i--)
        {
            if (i >= spawnedObjects.Count) continue; // Safety check

            SpawnedObjectInfo objectInfo = spawnedObjects[i];

            if (objectInfo.gameObject == null)
            {
                spawnedObjects.RemoveAt(i);
                continue;
            }

            // Move the object forward in the direction it was spawned
            objectInfo.gameObject.transform.position += objectInfo.spawnDirection * movementSpeed * Time.deltaTime;

            // Destroy objects that have moved beyond the destruction distance
            if (Vector3.Distance(transform.position, objectInfo.gameObject.transform.position) > destructionDistance)
            {
                Destroy(objectInfo.gameObject);
                spawnedObjects.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// Starts the spawning process
    /// </summary>
    public void StartSpawning()
    {
        if (spawnCoroutine == null && gameObject.activeInHierarchy)
        {
            spawnCoroutine = StartCoroutine(SpawnRoutine());
        }
    }

    /// <summary>
    /// Stops the spawning process
    /// </summary>
    public void StopSpawning()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    /// <summary>
    /// Coroutine that handles spawning prefabs at specified intervals
    /// </summary>
    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            // Wait for the specified frequency
            yield return new WaitForSeconds(spawnFrequency);

            // Check if the prefab is assigned
            if (prefabToSpawn == null)
            {
                Debug.LogWarning("PrefabSpawner: No prefab assigned to spawn!");
                continue;
            }

            // Spawn a prefab at this transform's position and rotation
            GameObject spawnedObject = Instantiate(prefabToSpawn, transform.position, transform.rotation);

            // Store the forward direction at spawn time
            Vector3 spawnDirection = transform.forward;

            // Add to our list for tracking
            spawnedObjects.Add(new SpawnedObjectInfo(spawnedObject, spawnDirection));
        }
    }

    /// <summary>
    /// Cleans up all spawned objects
    /// </summary>
    public void ClearAllSpawnedObjects()
    {
        foreach (SpawnedObjectInfo objectInfo in spawnedObjects)
        {
            if (objectInfo.gameObject != null)
            {
                Destroy(objectInfo.gameObject);
            }
        }
        spawnedObjects.Clear();
    }

    void OnDestroy()
    {
        ClearAllSpawnedObjects();
    }
}