using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    [Tooltip("The player GameObject for the enemy to target.")]
    private Transform playerTransform;
    [SerializeField]
    [Tooltip("The rotating part of the enemy that holds the attack point.")]
    private Transform aimPivot;
    [SerializeField]
    [Tooltip("A reference to the melee attacker script, if this enemy has one.")]
    private EnemyMeleeAttacker meleeAttacker;
    private Animator animator; // Reference to the Animator component
    private SpriteRenderer spriteRenderer; // Reference to the sprite renderer for flipping

    [Header("AI Settings")]
    [SerializeField]
    [Tooltip("The distance at which the enemy will start chasing the player.")]
    private float chaseDistance = 10f;
    [SerializeField]
    [Tooltip("The distance at which the enemy will stop and attack.")]
    private float attackRange = 1.5f;
    [SerializeField]
    [Tooltip("How fast the enemy moves.")]
    private float moveSpeed = 3f;

    private Rigidbody2D rb;
    private bool isKnockedBack = false;
    private bool isAttacking = false; // New state to prevent movement during an attack
    private Vector2 directionToPlayer; // Store direction for aiming when idle

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (animator == null)
        {
            Debug.LogError("EnemyAI Error: Animator component not found on this object or its children!", this);
        }

        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
            else
            {
                Debug.LogError("EnemyAI Error: Player transform not found! Make sure the player has the 'Player' tag.", this);
                this.enabled = false;
            }
        }
    }

    void Update()
    {
        // AI logic is now paused if knocked back OR if in the middle of an attack
        if (playerTransform == null || isKnockedBack || isAttacking)
        {
            if (isAttacking) rb.velocity = Vector2.zero; // Ensure the enemy stops while attacking
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        directionToPlayer = (playerTransform.position - transform.position).normalized;

        if (distanceToPlayer <= chaseDistance)
        {
            AimAtPlayer(directionToPlayer);

            if (distanceToPlayer > attackRange)
            {
                rb.velocity = directionToPlayer * moveSpeed;
            }
            else
            {
                rb.velocity = Vector2.zero;
                if (meleeAttacker != null)
                {
                    // The AI's only job is to request an attack.
                    // The MeleeAttacker script will handle the timing and execution.
                    meleeAttacker.AttemptAttack();
                }
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
        }

        UpdateAnimationAndSpriteFlip();
    }

    private void UpdateAnimationAndSpriteFlip()
    {
        if (animator == null || spriteRenderer == null) return;

        bool isMoving = rb.velocity.sqrMagnitude > 0.1f;
        animator.SetBool("isMoving", isMoving);

        if (isMoving)
        {
            // --- Animate and flip based on MOVEMENT direction ---
            animator.SetFloat("moveX", rb.velocity.x);
            animator.SetFloat("moveY", rb.velocity.y);

            // Flip based on the actual direction of movement
            if (rb.velocity.x < -0.1f)
            {
                spriteRenderer.flipX = true;
            }
            else if (rb.velocity.x > 0.1f)
            {
                spriteRenderer.flipX = false;
            }
        }
        else
        {
            // --- When idle, flip based on AIM direction to face the player ---
            if (directionToPlayer.x < -0.01f)
            {
                spriteRenderer.flipX = true;
            }
            else if (directionToPlayer.x > 0.01f)
            {
                spriteRenderer.flipX = false;
            }
        }
    }

    private void AimAtPlayer(Vector2 direction)
    {
        if (aimPivot != null)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            aimPivot.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    /// <summary>
    /// A public method allowing other scripts (like the MeleeAttacker) to control the AI's state.
    /// </summary>
    public void SetAttacking(bool state)
    {
        isAttacking = state;
    }

    public void StartKnockback(float knockbackDuration)
    {
        StartCoroutine(KnockbackCoroutine(knockbackDuration));
    }

    private IEnumerator KnockbackCoroutine(float knockbackDuration)
    {
        isKnockedBack = true;
        yield return new WaitForSeconds(knockbackDuration);
        isKnockedBack = false;
    }
}


