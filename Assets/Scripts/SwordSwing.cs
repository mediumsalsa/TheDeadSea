using UnityEngine;
using System.Collections;

/// <summary>
/// Controls the visual and hit detection for a sword swing.
/// </summary>
public class SwordSwingVisual : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    [Tooltip("How long the sword swing visual lasts before being destroyed.")]
    private float swingDuration = 0.2f;

    [SerializeField]
    [Tooltip("The tag assigned to enemy GameObjects.")]
    private string enemyTag = "Enemy";

    [SerializeField]
    [Tooltip("The force applied to enemies upon being hit by the sword.")]
    private float knockbackForce = 15f;

    private Collider2D swordCollider; // Reference to the trigger collider on this object

    void Awake()
    {
        swordCollider = GetComponent<Collider2D>();
        if (swordCollider == null)
        {
            Debug.LogError("SwordSwingVisual requires a Collider2D component (set to Is Trigger) on its GameObject!");
        }
    }

    void Start()
    {
        // Destroy the sword swing visual after its duration.
        Destroy(gameObject, swingDuration);
    }

    /// <summary>
    /// Called when the sword swing's trigger collider enters another collider.
    /// </summary>
    /// <param name="other">The collider of the object that was entered.</param>
    void OnTriggerEnter2D(Collider2D other)
    {
        // Only apply effects if the swing is active (not yet destroyed)
        // and if the other object is an enemy.
        if (gameObject.activeSelf && other.CompareTag(enemyTag))
        {
            Debug.Log("Sword hit: " + other.name);

            // Get the EnemyKnockback component and apply force.
            EnemyKnockback enemyKnockback = other.GetComponent<EnemyKnockback>();
            if (enemyKnockback != null)
            {
                // Calculate direction from the sword's position to the enemy.
                // We use the player's facing direction more accurately.
                Vector2 knockbackDir = (other.transform.position - transform.position).normalized;
                enemyKnockback.ApplyKnockback(knockbackDir, knockbackForce);
            }

            // Optional: Call a damage function on the enemy.
            // EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
            // if (enemyHealth != null)
            // {
            //     enemyHealth.TakeDamage(20);
            // }

            // To prevent multiple hits from one swing (e.g., if multiple enemies are in range,
            // or if the enemy has multiple colliders), you might want to disable the collider
            // after the first hit or use a list of hit enemies.
            // For now, it will hit all enemies within the trigger area.
        }
    }
}
