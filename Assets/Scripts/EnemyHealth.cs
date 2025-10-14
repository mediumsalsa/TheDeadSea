using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;

    // Optional: Assign a particle effect or sound effect to play on death
    // [SerializeField] private GameObject deathEffectPrefab;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        // Add a visual hit flash or sound effect here if you want

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log(gameObject.name + " has been defeated!");

        // if (deathEffectPrefab != null)
        // {
        //     Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        // }

        Destroy(gameObject);
    }
}
