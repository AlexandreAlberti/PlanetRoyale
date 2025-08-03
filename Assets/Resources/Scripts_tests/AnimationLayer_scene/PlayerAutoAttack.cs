using UnityEngine;

public class PlayerAutoAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackRange = 10f;
    public float attackInterval = 1.5f;
    public string enemyTag = "Enemy";

    [Header("References")]
    public Animator animator;
    public Transform aimBone;              // The bone that rotates toward the enemy
    public Transform projectileSpawner;    // The spawn point for projectiles
    public GameObject projectilePrefab;

    private float attackTimer = 0f;
    private GameObject currentTarget;

    void Update()
    {
        FindClosestEnemy();

        if (currentTarget != null)
        {
            RotateBoneTowards(currentTarget.transform);

            animator.SetLayerWeight(animator.GetLayerIndex("Attack"), 1f);

            attackTimer += Time.deltaTime;
            if (attackTimer >= attackInterval)
            {
                Attack();
                attackTimer = 0f;
            }
        }
        else
        {
            animator.SetLayerWeight(animator.GetLayerIndex("Attack"), 0f);
        }
    }

    void FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float closestDistance = Mathf.Infinity;
        currentTarget = null;

        foreach (GameObject enemy in enemies)
        {
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < closestDistance && dist <= attackRange)
            {
                closestDistance = dist;
                currentTarget = enemy;
            }
        }
    }

    void RotateBoneTowards(Transform target)
    {
        Vector3 direction = target.position - aimBone.position;
        direction.y = 0; // Lock Y axis if rotation should happen only horizontally
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        aimBone.rotation = Quaternion.Slerp(aimBone.rotation, targetRotation, Time.deltaTime * 10f);
    }

    void Attack()
    {
        if (projectilePrefab != null && projectileSpawner != null && currentTarget != null)
        {
            GameObject proj = Instantiate(projectilePrefab, projectileSpawner.position, projectileSpawner.rotation);

            HomingProjectile homing = proj.GetComponent<HomingProjectile>();
            if (homing != null)
            {
                homing.target = currentTarget.transform;
            }
        }
    }
}
