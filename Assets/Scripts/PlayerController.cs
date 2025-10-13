using UnityEngine;
/// <summary>
/// Handles player movement, input, and interaction with physics objects.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField]
    [Tooltip("The speed at which the player moves.")]
    private float moveSpeed = 5f;

    [SerializeField]
    [Tooltip("The force applied to pushable objects.")]
    private float pushForce = 10f;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        rb.gravityScale = 0;
    }

    void Update()
    {
        // --- Input Handling ---
        // Get input from WASD or arrow keys.
        // This returns a value between -1 and 1 for each axis.
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput.Normalize();
    }

    void FixedUpdate()
    {
        // --- Movement ---
        // Apply movement to the Rigidbody in FixedUpdate for physics consistency.
        rb.velocity = moveInput * moveSpeed;
    }

    /// <summary>
    /// This method is called when the player's collider makes contact with another collider.
    /// </summary>
    /// <param name="collision">Information about the collision.</param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the object we collided with has a Rigidbody2D.
        Rigidbody2D hitRb = collision.collider.attachedRigidbody;

        // Ensure we are not applying force to ourselves or static objects.
        if (hitRb != null && hitRb != rb)
        {
            // Calculate the direction from the player to the object.
            Vector2 direction = (collision.transform.position - transform.position).normalized;

            // Apply a force to the object to push it.
            // The ForceMode2D.Impulse applies the force instantly.
            hitRb.AddForce(direction * pushForce, ForceMode2D.Impulse);

            // If the object is an enemy, try to apply knockback.
            EnemyKnockback enemy = collision.gameObject.GetComponent<EnemyKnockback>();
            if (enemy != null)
            {
                enemy.ApplyKnockback(direction, pushForce);
            }
        }
    }
}
