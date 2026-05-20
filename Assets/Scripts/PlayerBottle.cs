using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class HolyWaterController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private PlayerInventory _inventory;
    [SerializeField] private GameObject _bottleVisual;
    [SerializeField] private Animator _bottleAnimator;

    [Header("Configuración")]
    [SerializeField] private float _animationDuration;

    // 🟢 NUEVO: Distancia máxima a la que el agua bendita salpica y asusta al enemigo
    [SerializeField] private float _effectDistance;

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

        if (_bottleAnimator != null)
        {
            _bottleAnimator.SetTrigger("Throw");
        }

        // 🟢 NUEVO: Buscamos al enemigo en el mapa mediante su etiqueta (Tag)
        GameObject enemyObject = GameObject.FindWithTag("Enemy");
        if (enemyObject != null)
        {
            // Calculamos qué tan lejos está el enemigo de nosotros
            float distanceToEnemy = Vector3.Distance(transform.position, enemyObject.transform.position);

            // Si está dentro de nuestro rango de salpicadura
            if (distanceToEnemy <= _effectDistance)
            {
                // Conseguimos el componente de IA del enemigo
                EnemyAI enemyAI = enemyObject.GetComponent<EnemyAI>();
                if (enemyAI != null)
                {
                    // ¡Activamos el efecto en el enemigo!
                    enemyAI.HolyWaterImpact();
                }
            }
        }

        yield return new WaitForSeconds(_animationDuration);
        _bottleVisual.SetActive(false);
        _isUsing = false;
    }
}
