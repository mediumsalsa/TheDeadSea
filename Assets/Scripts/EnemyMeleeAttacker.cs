using UnityEngine;

public class EnemyMeleeAttacker : MonoBehaviour
{
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float attackRadius = 0.5f;
    [SerializeField] private LayerMask playerLayer; // Set this in the inspector to your player's layer
    [SerializeField] private Transform attackPoint; // An empty GameObject in front of the enemy

    private float lastAttackTime = -999f;

    public void PerformAttack(Transform target)
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            Debug.Log(gameObject.name + " performs a melee attack!");

            // Play attack animation here

            // Detect player in range
            Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, playerLayer);

            foreach (Collider2D playerCollider in hitPlayers)
            {
                // We'd create a PlayerHealth script similar to EnemyHealth
                // For now, we'll just log it.
                Debug.Log("Hit " + playerCollider.name);
                // PlayerHealth playerHealth = playerCollider.GetComponent<PlayerHealth>();
                // if(playerHealth != null)
                // {
                //     playerHealth.TakeDamage(attackDamage);
                // }
            }
        }
    }

    // Optional: Draw a gizmo in the editor to visualize the attack range
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}
