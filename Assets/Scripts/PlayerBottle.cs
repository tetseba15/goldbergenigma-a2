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

        
        yield return new WaitForSeconds(_animationDuration);
        _bottleVisual.SetActive(false);
        _isUsing = false;
    }
}
