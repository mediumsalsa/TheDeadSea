using UnityEngine;

public class EnemyAttackVisual : MonoBehaviour
{
    [SerializeField]
    [Tooltip("How long the visual effect lasts before disappearing.")]
    private float lifetime = 0.3f;

    void Start()
    {
        // Destroy this GameObject after the specified lifetime.
        Destroy(gameObject, lifetime);
    }
}
