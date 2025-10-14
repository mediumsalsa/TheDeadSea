using UnityEngine;

public class SwordSwingVisual : MonoBehaviour
{
    [SerializeField] private float swingDuration = 0.25f;
    [SerializeField] private float knockbackForce = 15f;

    // Public variable to hold a reference to the player who initiated the swing
    public Transform playerTransform;

    void Start()
    {
        // Destroy the swing visual after its duration
        Destroy(gameObject, swingDuration);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // We only apply knockback if we hit an enemy AND the playerTransform has been set
        if (other.CompareTag("Enemy") && playerTransform != null)
        {
            EnemyKnockback enemy = other.GetComponent<EnemyKnockback>();
            if (enemy != null)
            {
                // Calculate the direction from the PLAYER's center to the enemy's center.
                // This ensures the knockback is always away from the player.
                Vector2 knockbackDirection = (other.transform.position - playerTransform.position).normalized;

                // Apply the knockback using the new, reliable direction
                enemy.ApplyKnockback(knockbackDirection, knockbackForce);
            }
        }
    }
}
