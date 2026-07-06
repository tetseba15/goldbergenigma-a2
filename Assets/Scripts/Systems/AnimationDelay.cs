using System.Collections;
using UnityEngine;

public class AnimationDelay : MonoBehaviour
{
    [SerializeField] private float delay;

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        if (_animator != null) _animator.speed = 0f;
    }

    private void Start()
    {
        StartCoroutine(StartAnimationWithDelay());
    }

    private IEnumerator StartAnimationWithDelay()
    {
        yield return new WaitForSeconds(delay);
        if (_animator != null) _animator.speed = 1f;
    }
}
