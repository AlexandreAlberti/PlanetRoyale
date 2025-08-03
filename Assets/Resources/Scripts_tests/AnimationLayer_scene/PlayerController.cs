using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Jump Settings")]
    public float jumpForce = 10f;
    public float gravityScale = 2f; // Multiplies gravity during falling

    [Header("Ground Detection")]
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.3f;

    [Header("Animation")]
    public Animator animator; // Assign child animator here

    private Rigidbody rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false; // We'll handle gravity manually

        if (animator == null)
        {
            Debug.LogWarning("Animator not assigned! Please assign the Animator from the child object.");
        }
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        if (animator != null)
        {
            animator.SetBool("isGrounded", isGrounded);
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); // Reset vertical speed
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            if (animator != null)
            {
                animator.SetTrigger("Jump");
            }
        }
    }

    void FixedUpdate()
    {
        // Apply custom gravity
        rb.AddForce(Physics.gravity * gravityScale, ForceMode.Acceleration);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
