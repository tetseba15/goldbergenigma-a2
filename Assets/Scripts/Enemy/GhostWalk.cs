using System.Collections;
using UnityEngine;

public class GhostWalk : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SkinnedMeshRenderer _meshRenderer;

    [Header("Settings")]
    [SerializeField] private float _dissapearTime = 2f;
    [SerializeField] private float _speed = 1f;
    [SerializeField] private float _triggerDistance = 5f;

    [Header("Audio")]
    [SerializeField] private AudioClip _audioClip;

    private Rigidbody _rigidBody;
    private Animator _animator;

    private Coroutine _disappearCoroutine;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        LockedDoor.OnOpenDoor += AppearWalking;
    }

    private void OnDisable()
    {
        LockedDoor.OnOpenDoor -= AppearWalking;
    }

    public void AppearWalking()
    {
        float distanceWithPlayer = Vector3.Distance(transform.position, PlayerTarget.Instance.PlayerTransform.position);

        if (distanceWithPlayer > _triggerDistance) return;

        if (_disappearCoroutine != null)
            StopCoroutine(_disappearCoroutine);

        _meshRenderer.enabled = true;

        if (_audioClip) AudioManager.Instance.PlaySFXAtPosition(_audioClip, transform.position, 1f, Random.Range(0.8f, 1.05f), 30f, transform);
        if (_rigidBody != null) _rigidBody.linearVelocity = new(_speed, 0, 0);
        if (_animator != null) _animator.SetFloat("Speed", _speed);

        _disappearCoroutine = StartCoroutine(Dissapear());
    }

    private IEnumerator Dissapear()
    {
        yield return new WaitForSeconds(_dissapearTime);

        Destroy(gameObject);
    }
}
