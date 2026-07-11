using UnityEngine;

public class FlyingBook : MonoBehaviour, IPurificable
{
    [Header("Purification Settings")]
    [SerializeField] private PurificableData _purificationData;

    public PurificableData PurificationData => _purificationData;

    private void OnEnable()
    {
        CrossController.OnCrossUse += Purify;
    }

    private void OnDisable()
    {
        CrossController.OnCrossUse -= Purify;
    }

    public void Purify(PlayerInventory.ItemType itemType, Vector3 purifySourcePosition)
    {
        float distanceWithZone = Vector3.Distance(transform.position, purifySourcePosition);
        if (distanceWithZone > _purificationData._purificationDistance || itemType != _purificationData._purificationItem) return;

        Animator animator = GetComponent<Animator>();
        Rigidbody rb = GetComponent<Rigidbody>();

        Vector3 _bookAnimationPosition = transform.position;

        if (animator != null)
        {
            animator.speed = 0f;
            animator.enabled = false;
        }

        transform.position = _bookAnimationPosition;

        if (rb != null) rb.useGravity = true;
    }
}
