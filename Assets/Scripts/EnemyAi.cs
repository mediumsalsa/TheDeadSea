using UnityEngine;
using System.Collections; // Needed for Coroutines

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAI : MonoBehaviour
{
    private enum State { Idle, Chasing, Attacking }
    private State currentState;

    [Header("AI Parameters")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float chaseRange = 8f;
    [SerializeField] private float attackRange = 1.5f;

    [Header("References")]
    [SerializeField]
    [Tooltip("The attack action component for this enemy.")]
    private EnemyMeleeAttacker meleeAttacker;

    private Transform playerTransform;
    private Rigidbody2D rb;
    private bool isKnockedBack = false; // Flag to pause AI during knockback

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Player not found! Make sure the player has the 'Player' tag.");
            this.enabled = false;
        }

        currentState = State.Idle;
    }

    void Update()
    {
        // If knocked back, the AI does nothing until the coroutine finishes
        if (isKnockedBack || playerTransform == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        // State Transition Logic
        switch (currentState)
        {
            case State.Idle:
                if (distanceToPlayer < chaseRange)
                {
                    currentState = State.Chasing;
                }
                break;
            case State.Chasing:
                if (distanceToPlayer <= attackRange)
                {
                    currentState = State.Attacking;
                }
                else if (distanceToPlayer > chaseRange)
                {
                    currentState = State.Idle;
                }
                break;
            case State.Attacking:
                if (distanceToPlayer > attackRange)
                {
                    currentState = State.Chasing;
                }
                break;
        }

        // State Action Logic
        switch (currentState)
        {
            case State.Chasing:
                ChasePlayer();
                break;
            case State.Attacking:
                AttackPlayer();
                break;
            case State.Idle:
            default:
                StopMovement();
                break;
        }
    }

    // This public method will be called by EnemyKnockback
    public void StartKnockback(float duration)
    {
        if (!isKnockedBack)
        {
            StartCoroutine(KnockbackCoroutine(duration));
        }
    }

    private IEnumerator KnockbackCoroutine(float duration)
    {
        isKnockedBack = true;
        // The knockback force is applied by the other script
        yield return new WaitForSeconds(duration);
        isKnockedBack = false;
    }

    private void ChasePlayer()
    {
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        rb.velocity = direction * moveSpeed;
    }

    private void AttackPlayer()
    {
        rb.velocity = Vector2.zero;
        if (meleeAttacker != null)
        {
            meleeAttacker.PerformAttack(playerTransform);
        }
    }

    private void StopMovement()
    {
        rb.velocity = Vector2.zero;
    }
}
