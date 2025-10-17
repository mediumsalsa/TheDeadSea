using UnityEngine;
using System.Collections;

public class EnemyMeleeAttacker : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField]
    [Tooltip("The point from which the attack is spawned.")]
    private Transform attackPoint;
    [SerializeField]
    [Tooltip("The amount of damage this attack deals.")]
    private int attackDamage = 10;
    [SerializeField]
    [Tooltip("How often the enemy can start a new attack (in seconds).")]
    private float attackCooldown = 2f;
    [SerializeField]
    [Tooltip("The radius of the attack.")]
    private float attackRadius = 0.5f;
    [SerializeField]
    [Tooltip("The layer the player is on, to ensure only the player is hit.")]
    private LayerMask playerLayer;

    [Header("Animation Timings")]
    [SerializeField]
    [Tooltip("The duration of the 'wind-up' before the attack hits.")]
    private float windUpDuration = 0.5f;
    [SerializeField]
    [Tooltip("The total duration of the attack animation from start to finish.")]
    private float totalAttackDuration = 1f;

    // References to other components
    private Animator animator;
    private EnemyAI enemyAI;

    private float lastAttackTime = -999f; // Start ready to attack

    void Start()
    {
        // Get references to components on the parent object
        animator = GetComponentInParent<Animator>();
        enemyAI = GetComponentInParent<EnemyAI>();
    }

    /// <summary>
    /// This is the public method that the AI script calls to initiate an attack.
    /// </summary>
    public void AttemptAttack()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            StartCoroutine(AttackSequence());
        }
    }

    private IEnumerator AttackSequence()
    {
        // 1. Tell the AI to stop moving and start the animation
        enemyAI.SetAttacking(true);
        animator.SetTrigger("attack");

        // 2. Wait for the wind-up period
        yield return new WaitForSeconds(windUpDuration);

        // 3. Perform the actual damage check
        PerformDamageCheck();

        // 4. Wait for the rest of the animation to complete
        yield return new WaitForSeconds(totalAttackDuration - windUpDuration);

        // 5. Tell the AI it can move again
        enemyAI.SetAttacking(false);
    }

    private void PerformDamageCheck()
    {
        Debug.Log(gameObject.name + " performs damage check!");

        // We no longer need to spawn a separate visual, as the animation is the visual.

        // Detect and damage the player
        Collider2D hitPlayer = Physics2D.OverlapCircle(attackPoint.position, attackRadius, playerLayer);
        if (hitPlayer != null)
        {
            Debug.Log("Hit the player for " + attackDamage + " damage!");
            // Example: hitPlayer.GetComponent<PlayerHealth>()?.TakeDamage(attackDamage);
        }
    }

    // This helps visualize the attack range in the editor
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}
