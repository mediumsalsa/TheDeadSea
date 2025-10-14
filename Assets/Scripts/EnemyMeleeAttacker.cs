using UnityEngine;

public class EnemyMeleeAttacker : MonoBehaviour
{
    [Header("Attack Parameters")]
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float attackRadius = 0.5f;

    [Header("References")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private Transform attackPoint;
    [SerializeField]
    [Tooltip("The visual prefab to spawn when attacking.")]
    private GameObject attackVisualPrefab; // The new visual prefab

    private float lastAttackTime = -999f;

    public void PerformAttack(Transform target)
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            Debug.Log(gameObject.name + " performs a melee attack!");

            // --- NEW: Spawn the visual effect ---
            if (attackVisualPrefab != null && attackPoint != null)
            {
                // Calculate the direction to the player to orient the attack visual
                Vector2 direction = (target.position - attackPoint.position).normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                Quaternion rotation = Quaternion.Euler(0, 0, angle);

                // Create the visual at the attack point with the correct rotation
                Instantiate(attackVisualPrefab, attackPoint.position, rotation);
            }

            // The damage logic remains the same
            Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, playerLayer);

            foreach (Collider2D playerCollider in hitPlayers)
            {
                Debug.Log("Hit " + playerCollider.name);
                // Future step: Get a PlayerHealth component and deal damage
                // PlayerHealth playerHealth = playerCollider.GetComponent<PlayerHealth>();
                // if(playerHealth != null) { playerHealth.TakeDamage(attackDamage); }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}
