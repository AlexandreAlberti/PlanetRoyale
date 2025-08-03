using UnityEngine;

public class PrefabCollisionDestroyer : MonoBehaviour
{
    [Tooltip("Tag of the enemy object to check for collision")]
    public string enemyTag = "Enemy";

    [Tooltip("Whether to destroy on trigger enter (for trigger colliders)")]
    public bool destroyOnTrigger = true;

    [Tooltip("Whether to destroy on normal collision (for non-trigger colliders)")]
    public bool destroyOnCollision = false;

    [Tooltip("Optional particle effect to spawn when destroyed")]
    public GameObject destroyEffect;

    [Tooltip("Should the effect be destroyed automatically?")]
    public bool autoDestroyEffect = true;

    [Tooltip("Time until the effect is destroyed (if autoDestroyEffect is true)")]
    public float effectLifetime = 1.0f;

    private void OnTriggerEnter(Collider other)
    {
        if (destroyOnTrigger && other.CompareTag(enemyTag))
        {
            HandleDestruction();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (destroyOnCollision && collision.gameObject.CompareTag(enemyTag))
        {
            HandleDestruction();
            Debug.Log("arrow");
        }
    }

    private void HandleDestruction()
    {
        // Spawn effect if one is specified
        if (destroyEffect != null)
        {
            GameObject effect = Instantiate(destroyEffect, transform.position, transform.rotation);

            // Automatically destroy the effect after a delay if specified
            if (autoDestroyEffect)
            {
                Destroy(effect, effectLifetime);
            }
        }

        // Destroy this game object
        Destroy(gameObject);
    }
}