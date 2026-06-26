using System.Collections;
using UnityEngine;

public class DiggableObject : MonoBehaviour, IInteractable
{
    [Header("Configuración de Interacción")]
    [SerializeField] private string _digPrompt = "Cavar tierra";
    [SerializeField] private string _missingShovelPrompt = "Necesito una pala...";
    [SerializeField] private float _animationDuration = 2f;

    [Header("Recompensas (Asignar en Inspector)")]
    [SerializeField, Tooltip("La llave que soltará este pozo")]
    private GameObject _keyPrefab;
    [SerializeField, Tooltip("Nota a soltar (Dejar vacío si es la tumba)")]
    private GameObject _notePrefab;
    [SerializeField, Tooltip("Diálogo a activar (Dejar vacío si es la tumba)")]
    private GameObject _dialogueToEnable;

    [Header("Audio")]
    [SerializeField] private AudioClip _shovelSound;
    [SerializeField] private AudioClip _keySpawnSound;

    private bool _isDug = false;
    private bool _isDigging = false;

    public string GetInteractPrompt(GameObject interactor)
    {
        if (_isDug || _isDigging) return string.Empty;

        PlayerInventory inventory = interactor.GetComponent<PlayerInventory>();
        if (inventory != null && inventory.HasItem(PlayerInventory.ItemType.Shovel))
        {
            return _digPrompt; 
        }

        return _missingShovelPrompt; 
    }

    public void Interact(GameObject interactor)
    {
        if (_isDug || _isDigging) return;

        PlayerInventory inventory = interactor.GetComponent<PlayerInventory>();

        if (inventory != null && inventory.HasItem(PlayerInventory.ItemType.Shovel))
        {
            StartCoroutine(UseShovelRoutine(interactor));
        }
        else
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX(_keySpawnSound, 0.5f); // Sonido temporal de error
        }
    }

    private IEnumerator UseShovelRoutine(GameObject interactor)
    {
        _isDigging = true;

        if (_shovelSound != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFXAtPosition(_shovelSound, transform.position, 1f, 1f);
        }

        PlayerShovelVisual playerShovel = interactor.GetComponent<PlayerShovelVisual>();
        if (playerShovel != null) playerShovel.ShowAndDig();

        yield return new WaitForSeconds(_animationDuration * 0.5f);

        if (_notePrefab != null)
        {
            Instantiate(_notePrefab, transform.position + new Vector3(0f, 0.1f, 0f), Quaternion.identity);
        }

        if (_keyPrefab != null)
        {
            Instantiate(_keyPrefab, transform.position + new Vector3(0f, 0.3f, 0f), Quaternion.identity);
            if (_keySpawnSound != null && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFXAtPosition(_keySpawnSound, transform.position, 1f, 1f);
            }
        }

        if (_dialogueToEnable != null)
        {
            _dialogueToEnable.SetActive(true);
        }

        yield return new WaitForSeconds(_animationDuration * 0.5f);

        if (playerShovel != null) playerShovel.HideShovel();

        _isDigging = false;
        _isDug = true;

        gameObject.layer = LayerMask.NameToLayer("Default");
    }
}