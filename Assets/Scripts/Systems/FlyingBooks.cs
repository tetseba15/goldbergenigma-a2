using System.Collections.Generic;
using UnityEngine;

public class FlyingBooks : MonoBehaviour
{
    [SerializeField] private PurificationZone _purificationZone;
    [SerializeField] private List<GameObject> _books;

    private bool _booksStopped;

    private void Update()
    {
        if (_purificationZone != null && _purificationZone.IsPurified && !_booksStopped)
        {
            StopBooks();
            _booksStopped = true;
        }

    }

    private void StopBooks()
    {
        foreach (GameObject book in _books)
        {
            Animator animator = book.GetComponent<Animator>();
            Rigidbody rb = book.GetComponent<Rigidbody>();

            Vector3 _bookAnimationPosition = rb.transform.position;

            if (animator != null)
            {
                animator.speed = 0f;
                animator.enabled = false;
            }

            book.transform.position = _bookAnimationPosition;

            if (rb != null) rb.useGravity = true;
        }
    }
}
