using UnityEngine;

/// <summary>
/// Manages physics-based knockback for an enemy character.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyKnockback : MonoBehaviour
{
    private Rigidbody2D rb;

    void Awake()
    {
        // Get the Rigidbody2D component.
        rb = GetComponent<Rigidbody2D>();
        // Set properties for a typical top-down physics object.
        rb.gravityScale = 0;
        rb.freezeRotation = true;
    }

    /// <summary>
    /// Applies a force to this Rigidbody to simulate knockback.
    /// </summary>
    /// <param name="direction">The direction the knockback should be applied in.</param>
    /// <param name="force">The magnitude of the knockback force.</param>
    public void ApplyKnockback(Vector2 direction, float force)
    {
        // We use Impulse mode to apply an instant force, creating a sudden knockback effect.
        rb.AddForce(direction * force, ForceMode2D.Impulse);
    }
}
