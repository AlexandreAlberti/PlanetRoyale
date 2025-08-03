using UnityEngine;

public class HomingProjectile : MonoBehaviour
{
    [Header("Homing Settings")]
    public float speed = 10f;
    public Transform target;

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject); // Target is gone
            return;
        }

        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        transform.forward = direction;
    }

    private void OnTriggerEnter(Collider other)
    {
        {
            Destroy(gameObject);
        }
    }
}
