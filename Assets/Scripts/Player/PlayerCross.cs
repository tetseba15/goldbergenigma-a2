using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CrossController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private PlayerInventory _inventory;
    [SerializeField] private GameObject _crossVisual;
    [SerializeField] private Animator _crossAnimator;

    [Header("Audio")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _crossSound;

    [Header("Configuración")]
    [SerializeField] private float _animationDuration;
    [SerializeField] private float _effectDistance;
    
    [SerializeField] private float _stunDuration;

    private bool _isUsing = false;

    void Start()
    {
        if (_crossVisual != null)
        {
            _crossVisual.SetActive(false);
        }
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        
        if (Keyboard.current.cKey.wasPressedThisFrame)
        {
            bool tieneItem = _inventory.HasItem(PlayerInventory.ItemType.Cross);

            if (tieneItem && !_isUsing)
            {
                StartCoroutine(UseCrossRoutine());
            }
        }
    }

    private IEnumerator UseCrossRoutine()
    {
        _isUsing = true;
        _crossVisual.SetActive(true);

        if (_audioSource != null && _crossSound != null)
        {
            _audioSource.PlayOneShot(_crossSound);
        }

        if (_crossAnimator != null)
        {
            _crossAnimator.SetTrigger("Cross");
        }

        GameObject enemyObject = GameObject.FindWithTag("Enemy");
        if (enemyObject != null)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemyObject.transform.position);

            if (distanceToEnemy <= _effectDistance)
            {
                EnemyAI enemyAI = enemyObject.GetComponent<EnemyAI>();
                if (enemyAI != null)
                {
                    enemyAI.CrossImpact(_stunDuration); //volver a poner en caso de emergencia
                }
            }
        }

        yield return new WaitForSeconds(_animationDuration);
        _crossVisual.SetActive(false);
        _isUsing = false;
    }
}
