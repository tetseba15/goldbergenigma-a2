using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class HolyWaterController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private PlayerInventory _inventory;
    [SerializeField] private GameObject _bottleVisual;
    [SerializeField] private Animator _bottleAnimator;

    
    [Header("Audio")]
    [SerializeField] private AudioSource _audioSource;   
    [SerializeField] private AudioClip _throwSound;     

    [Header("Configuración")]
    [SerializeField] private float _animationDuration;
    [SerializeField] private float _effectDistance = 6f;

    private bool _isUsing = false;

    void Start()
    {
        if (_bottleVisual != null)
        {
            _bottleVisual.SetActive(false);
        }
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            bool tieneItem = _inventory.HasItem(PlayerInventory.ItemType.Bottle);

            if (tieneItem && !_isUsing)
            {
                StartCoroutine(UseHolyWaterRoutine());
            }
        }
    }

    private IEnumerator UseHolyWaterRoutine()
    {
        _isUsing = true;
        _bottleVisual.SetActive(true);

        
        if (_audioSource != null && _throwSound != null)
        {
            _audioSource.PlayOneShot(_throwSound);
        }

        if (_bottleAnimator != null)
        {
            _bottleAnimator.SetTrigger("Throw");
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
                    enemyAI.HolyWaterImpact();
                }
            }
        }

        yield return new WaitForSeconds(_animationDuration);
        _bottleVisual.SetActive(false);
        _isUsing = false;
    }
}
