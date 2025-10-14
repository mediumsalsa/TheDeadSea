using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyKnockback : MonoBehaviour
{
    [SerializeField]
    [Tooltip("How long the AI is disabled after being hit.")]
    private float knockbackDuration = 0.2f;

    private Rigidbody2D rb;
    private EnemyAI enemyAI; // Reference to the AI script

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyAI = GetComponent<EnemyAI>(); // Get the AI component on this enemy
    }

    public void ApplyKnockback(Vector2 direction, float force)
    {
        // Stop the enemy's current movement before applying new force
        rb.velocity = Vector2.zero;
        rb.AddForce(direction * force, ForceMode2D.Impulse);

        // Tell the AI script to pause its logic
        if (enemyAI != null)
        {
            enemyAI.StartKnockback(knockbackDuration);
        }
    }
}
