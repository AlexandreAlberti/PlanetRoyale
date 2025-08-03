using System.Collections;
using UnityEngine;

public class EnemyMaterialSwapper : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Reference to the Skinned Mesh Renderer")]
    public SkinnedMeshRenderer skinnedMeshRenderer;

    [Header("Materials")]
    [Tooltip("The default material to use")]
    public Material defaultMaterial;

    [Tooltip("The material to swap to when hit")]
    public Material hitMaterial;

    [Header("Swap Settings")]
    [Tooltip("Duration in seconds to show the hit material before switching back")]
    public float hitMaterialDuration = 0.2f;

    [Tooltip("Should the material return to default after the duration?")]
    public bool returnToDefault = true;

    [Tooltip("Tag of projectiles that can trigger the material swap")]
    public string projectileTag = "Projectile";

    [Header("Effects")]
    [Tooltip("Optional particle effect to spawn when hit")]
    public GameObject hitEffect;

    [Tooltip("Should the hit effect be destroyed automatically?")]
    public bool autoDestroyEffect = true;

    [Tooltip("Time until the hit effect is destroyed")]
    public float effectLifetime = 1.0f;

    private Coroutine activeSwapCoroutine;

    private void Awake()
    {
        // Auto-find skinned mesh renderer if not assigned
        if (skinnedMeshRenderer == null)
        {
            skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

            if (skinnedMeshRenderer == null)
            {
                Debug.LogError("No SkinnedMeshRenderer found on " + gameObject.name + " or its children!");
            }
        }

        // Store the default material if not assigned
        if (defaultMaterial == null && skinnedMeshRenderer != null)
        {
            defaultMaterial = skinnedMeshRenderer.material;
        }
    }

    /// <summary>
    /// Call this method when the enemy is hit
    /// </summary>
    public void OnHit()
    {
        if (skinnedMeshRenderer == null || hitMaterial == null) return;

        // Spawn hit effect if specified
        if (hitEffect != null)
        {
            GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
            if (autoDestroyEffect)
            {
                Destroy(effect, effectLifetime);
            }
        }

        // Stop any active material swap coroutine
        if (activeSwapCoroutine != null)
        {
            StopCoroutine(activeSwapCoroutine);
        }

        // Start a new material swap
        activeSwapCoroutine = StartCoroutine(SwapMaterialRoutine());
    }

    /// <summary>
    /// Coroutine to handle the material swap
    /// </summary>
    private IEnumerator SwapMaterialRoutine()
    {
        // Apply the hit material
        Material[] materials = skinnedMeshRenderer.materials;
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i] = hitMaterial;
        }
        skinnedMeshRenderer.materials = materials;

        // If we should return to default material after a duration
        if (returnToDefault && defaultMaterial != null)
        {
            // Wait for the specified duration
            yield return new WaitForSeconds(hitMaterialDuration);

            // Return to the default material
            materials = skinnedMeshRenderer.materials;
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = defaultMaterial;
            }
            skinnedMeshRenderer.materials = materials;
        }

        // Clear the coroutine reference
        activeSwapCoroutine = null;
    }

    // This handles collisions with objects like projectiles
    private void OnTriggerEnter(Collider other)
    {
        // Automatically detect collisions with projectiles
        if (other.CompareTag(projectileTag))
        {
            OnHit();
        }
    }

    // For non-trigger collisions
    private void OnCollisionEnter(Collision collision)
    {
        // Automatically detect collisions with projectiles
        if (collision.gameObject.CompareTag(projectileTag))
        {
            OnHit();
        }
    }
}