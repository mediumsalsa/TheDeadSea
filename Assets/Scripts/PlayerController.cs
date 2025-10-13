using UnityEngine;
using System.Collections;

/// <summary>
/// Handles player movement, combat, and input.
/// Manages aiming, rolling, melee attacks, and ranged attacks.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField]
    [Tooltip("The standard speed at which the player moves.")]
    private float moveSpeed = 5f;
    [SerializeField]
    [Tooltip("The force applied to pushable objects.")]
    private float pushForce = 10f;

    [Header("Roll/Dash Settings")]
    [SerializeField]
    [Tooltip("The speed multiplier during a roll.")]
    private float rollSpeed = 15f;
    [SerializeField]
    [Tooltip("How long the roll lasts in seconds.")]
    private float rollDuration = 0.25f;
    [SerializeField]
    [Tooltip("Cooldown time between rolls in seconds.")]
    private float rollCooldown = 1f;

    [Header("Combat Settings")]
    [SerializeField]
    [Tooltip("Transform that holds the weapon and points towards the mouse.")]
    private Transform aimPivot;
    [SerializeField]
    [Tooltip("The prefab for the projectile (throwing knife).")]
    private GameObject knifePrefab;
    [SerializeField]
    [Tooltip("The point from where the knife is thrown.")]
    private Transform knifeSpawnPoint;
    [SerializeField]
    [Tooltip("The force with which the knife is thrown.")]
    private float knifeThrowForce = 20f;
    [SerializeField]
    [Tooltip("Cooldown for throwing knives.")]
    private float knifeThrowCooldown = 0.5f;

    [Header("Melee Attack")]
    [SerializeField]
    [Tooltip("The visual prefab for the sword swing.")]
    private GameObject swordSwingVisualPrefab; // NEW FIELD
    [SerializeField]
    [Tooltip("Cooldown for melee attacks.")]
    private float meleeAttackCooldown = 0.4f;

    // Private state variables
    private Rigidbody2D rb;
    private Camera mainCamera;
    private Vector2 moveInput;
    private Vector2 mousePosition;

    // Rolling state
    private bool isRolling = false;
    private float rollCooldownTimer = 0f;

    // Combat state
    private float knifeCooldownTimer = 0f;
    private float meleeCooldownTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;

        rb.freezeRotation = true;
        rb.gravityScale = 0;
    }

    void Update()
    {
        // --- Cooldown Timers ---
        if (rollCooldownTimer > 0) rollCooldownTimer -= Time.deltaTime;
        if (knifeCooldownTimer > 0) knifeCooldownTimer -= Time.deltaTime;
        if (meleeCooldownTimer > 0) meleeCooldownTimer -= Time.deltaTime;

        // Player input is ignored while rolling
        if (isRolling) return;

        // --- Input Handling ---
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput.Normalize();

        mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        // --- Action Inputs ---
        if (Input.GetKeyDown(KeyCode.Space) && rollCooldownTimer <= 0)
        {
            StartCoroutine(Roll());
        }

        if (Input.GetMouseButtonDown(0) && meleeCooldownTimer <= 0) // Left Click
        {
            MeleeAttack();
        }

        if (Input.GetMouseButtonDown(1) && knifeCooldownTimer <= 0) // Right Click
        {
            RangedAttack();
        }
    }

    void FixedUpdate()
    {
        if (isRolling) return; // Movement is handled by the Roll coroutine

        // --- Aiming Logic ---
        // Rotates the aimPivot to point at the mouse cursor.
        Vector2 aimDirection = mousePosition - (Vector2)aimPivot.position;
        float aimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        aimPivot.rotation = Quaternion.Euler(0, 0, aimAngle);

        // --- Movement ---
        rb.velocity = moveInput * moveSpeed;
    }

    private IEnumerator Roll()
    {
        isRolling = true;
        rollCooldownTimer = rollCooldown;

        // Use movement input for roll direction, or aim direction if standing still
        Vector2 rollDirection = moveInput != Vector2.zero ? moveInput : ((Vector2)(aimPivot.right)).normalized;
        rb.velocity = rollDirection * rollSpeed;

        yield return new WaitForSeconds(rollDuration);

        isRolling = false;
    }

    private void MeleeAttack()
    {
        meleeCooldownTimer = meleeAttackCooldown;
        Debug.Log("Swing Sword!");

        if (swordSwingVisualPrefab != null && aimPivot != null)
        {
            // Instantiate the sword swing visual at the aimPivot's position and rotation
            // The SwordSwingVisual script will handle its own lifetime and hit detection.
            GameObject swing = Instantiate(swordSwingVisualPrefab, aimPivot.position, aimPivot.rotation);
            // Parent it to the aimPivot so it moves with the player and maintains its rotation relative to the aim.
            swing.transform.parent = aimPivot;
        }
        else
        {
            Debug.LogWarning("Sword Swing Visual Prefab or Aim Pivot is not assigned!");
        }
    }

    private void RangedAttack()
    {
        knifeCooldownTimer = knifeThrowCooldown;
        Debug.Log("Throw Knife!");

        if (knifePrefab != null && knifeSpawnPoint != null)
        {
            GameObject knife = Instantiate(knifePrefab, knifeSpawnPoint.position, aimPivot.rotation);
            Rigidbody2D knifeRb = knife.GetComponent<Rigidbody2D>();
            if (knifeRb != null)
            {
                knifeRb.AddForce(aimPivot.right * knifeThrowForce, ForceMode2D.Impulse);
            }
        }
    }

    /// <summary>
    /// This method is called when the player's collider makes contact with another collider.
    /// It's primarily used for pushing physics-based objects now.
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isRolling) return; // Don't push things while rolling

        Rigidbody2D hitRb = collision.collider.attachedRigidbody;

        if (hitRb != null && hitRb != rb)
        {
            Vector2 direction = (collision.transform.position - transform.position).normalized;
            hitRb.AddForce(direction * pushForce, ForceMode2D.Impulse);
        }
    }
}
