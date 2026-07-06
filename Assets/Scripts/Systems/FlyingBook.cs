using UnityEngine;

public class FlyingBook : MonoBehaviour
{
    [SerializeField] private float _stopDistance = 10f;

    private void OnEnable()
    {
        PurificationZone.OnPurified += StopBook;
    }

    private void OnDisable()
    {
        PurificationZone.OnPurified += StopBook;
    }

    private void StopBook(Vector3 purificationZonePosition)
    {
        float distanceWithZone = Vector3.Distance(transform.position, purificationZonePosition);
        if (distanceWithZone > _stopDistance) return;

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
