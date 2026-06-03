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
    [SerializeField] private float _effectDistance;

    //para el UI
    [Header("Ajustes de Agua Bendita")]
    [SerializeField] private float _maxWater = 3f;
    private float _currentWater = 0f;
    private bool _isUsing = false;

    void Start()
    {
        if (_bottleVisual != null)
        {
            _bottleVisual.SetActive(false);
        }
        GameEvent.holyWater(_currentWater, _maxWater);//para llamar el event
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            bool tieneItem = _inventory.HasItem(PlayerInventory.ItemType.Bottle);

            if (tieneItem && !_isUsing && _currentWater > 0)//para verificar que quede agua
            {
                StartCoroutine(UseHolyWaterRoutine());
            }
        }
    }
    public void RefillBottle()
    {
        _currentWater = _maxWater; // Se llena al máximo (3)
        GameEvent.holyWater(_currentWater, _maxWater); // Actualiza la UI instantáneamente
    }

    private IEnumerator UseHolyWaterRoutine()
    {
        _isUsing = true;
        _bottleVisual.SetActive(true);

        
        _currentWater--;//UI descuenta un uso
        GameEvent.holyWater(_currentWater, _maxWater);//para el event UI
        // Descuenta un uso y dispara el evento
        if (_currentWater <= 0) yield break;//Para eliminar el sonido si esta vacia


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
                  enemyAI.HolyWaterImpact(); //volver a poner en caso de emergencia
                }
            }
        }

        yield return new WaitForSeconds(_animationDuration);
        _bottleVisual.SetActive(false);
        _isUsing = false;
    }
}
