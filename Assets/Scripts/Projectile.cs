using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The force applied to enemies upon impact.")]
    private float knockbackForce = 10f;
    [SerializeField]
    [Tooltip("Should the projectile be destroyed when it hits a solid object like a wall?")]
    private bool destroyOnCollision = true;

    // --- NEW: Component reference ---
    private Rigidbody2D rb;

    void Start()
    {
        // --- NEW: Get the Rigidbody2D component ---
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Projectile is missing a Rigidbody2D component!");
        }

        // Make sure the projectile is destroyed after some time in case it never goes off-screen
        Destroy(gameObject, 10f);
    }

    // --- NEW: Update method to handle rotation ---
    void Update()
    {
        // Check if the rigidbody exists and has a significant velocity
        if (rb != null && rb.velocity.sqrMagnitude > 0.1f)
        {
            // Calculate the angle from the velocity vector
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            // Set the rotation of the projectile to face its direction of movement
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    void OnBecameInvisible()
    {
        // Destroy the projectile once it's off-screen to save resources
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Apply knockback if it hits an enemy
        if (other.CompareTag("Enemy"))
        {
            EnemyKnockback enemy = other.GetComponent<EnemyKnockback>();
            if (enemy != null)
            {
                Vector2 knockbackDirection = (other.transform.position - transform.position).normalized;
                enemy.ApplyKnockback(knockbackDirection, knockbackForce);
            }
            Destroy(gameObject); // Destroy the knife after hitting an enemy
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Destroy if it hits a solid object (like a wall)
        if (destroyOnCollision)
        {
            Destroy(gameObject);
        }
    }
}