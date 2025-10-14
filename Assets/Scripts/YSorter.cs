using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class YSorter : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    [Tooltip("If checked, the sorting will only be calculated once on Start. Best for static objects like trees or rocks.")]
    [SerializeField] private bool runOnce = false;

    [Tooltip("A small, additional offset to fine-tune the sorting. A higher value makes the sprite appear more 'in front'.")]
    [SerializeField] private int sortingOrderOffset = 0;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (runOnce)
        {
            UpdateSortingOrder();
            // We disable the component after the first run for performance.
            this.enabled = false;
        }
    }

    // Using LateUpdate ensures this runs after all movement calculations for the frame are complete.
    void LateUpdate()
    {
        UpdateSortingOrder();
    }

    private void UpdateSortingOrder()
    {
        // This uses the bottom-most point of the sprite's bounding box in world space.
        // This works automatically, regardless of the sprite's pivot point.
        float groundYPosition = spriteRenderer.bounds.min.y;

        // We multiply by a large number (like 100) to get more precision from the float position,
        // then convert it to an integer for the sorting order.
        // We make it negative because a lower Y-position (higher on the screen) should have a
        // lower sorting order (be drawn behind).
        spriteRenderer.sortingOrder = -(int)(groundYPosition * 100) + sortingOrderOffset;
    }
}