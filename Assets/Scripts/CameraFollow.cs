using UnityEngine;

/// <summary>
/// Makes the camera smoothly follow a target Transform (like the player).
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [Header("Camera Settings")]
    [Tooltip("The target for the camera to follow.")]
    public Transform target;

    [SerializeField]
    [Tooltip("How quickly the camera catches up to the target. Lower values are slower.")]
    private float smoothSpeed = 0.125f;

    [SerializeField]
    [Tooltip("The offset from the target's position.")]
    private Vector3 offset = new Vector3(0, 0, -10);

    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        // Ensure we have a target to follow.
        if (target == null)
        {
            Debug.LogWarning("Camera Follow: Target is not set!");
            return;
        }

        // --- Camera Following Logic ---
        // We run this in LateUpdate to ensure the camera moves
        // after the player has finished moving for the frame.

        Vector3 desiredPosition = target.position + offset;

        // Smoothly move the camera from its current position to the desired position.
        // Vector3.SmoothDamp gradually changes a vector towards a desired goal over time.
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothSpeed);
    }
}
