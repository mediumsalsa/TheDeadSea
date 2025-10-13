using UnityEngine;

/// <summary>
/// Controls the behavior of a projectile, such as a throwing knife.
/// Handles movement, lifetime, and collision with enemies or the environment.
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [Tooltip("How long the projectile exists before being destroyed if it hits nothing.")]
    [SerializeField] private float lifetime = 3f;
    [Tooltip("The tag assigned to enemy GameObjects.")]
    [SerializeField] private string enemyTag = "Enemy";
    [Tooltip("The force of the knockback applied to enemies.")]
    [SerializeField] private float knockbackForce = 10f;
    [Tooltip("Should the projectile be destroyed when it hits any solid object?")]
    [SerializeField] private bool destroyOnCollision = true;

    void Start()
    {
        // Destroy the projectile after its lifetime expires as a fallback.
        Destroy(gameObject, lifetime);
    }

    /// <summary>
    /// Called when the projectile's collider enters another trigger collider.
    /// This is ideal for hitting enemies.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object we hit is an enemy.
        if (other.CompareTag(enemyTag))
        {
            Debug.Log("Projectile hit: " + other.name);

            // --- Apply Damage and Effects ---
            // e.g., other.GetComponent<EnemyHealth>().TakeDamage(10);

            // --- Apply Knockback ---
            EnemyKnockback enemyKnockback = other.GetComponent<EnemyKnockback>();
            if (enemyKnockback != null)
            {
                // Use the projectile's velocity to determine the direction of knockback.
                Rigidbody2D selfRb = GetComponent<Rigidbody2D>();
                if (selfRb != null)
                {
                    enemyKnockback.ApplyKnockback(selfRb.velocity.normalized, knockbackForce);
                }
            }

            // Destroy the projectile on impact with an enemy.
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Called when the projectile's collider makes contact with a solid (non-trigger) collider.
    /// Useful for hitting walls or other obstacles.
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // If destroyOnCollision is enabled, destroy the projectile when it hits any solid object.
        if (destroyOnCollision)
        {
            // We can add effects here later, like sparks or a "thud" sound.
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// This function is called by Unity when the object is no longer visible by any camera.
    /// It's an efficient way to clean up projectiles that fly off-screen.
    /// </summary>
    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}