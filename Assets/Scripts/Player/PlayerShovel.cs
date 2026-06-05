using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShovelController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private PlayerInventory _inventory;
    [SerializeField] private GameObject _shovelVisual;
    [SerializeField] private Animator _shovelAnimator;
    [SerializeField] private Camera _mainCamera;

    [Header("Audio de la Pala")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _shovelSound;

    [Header("Llaves")]
    [SerializeField, Tooltip("Llave chimenea")]
    private GameObject _chimneyKeyPrefab;
    [SerializeField, Tooltip("Llave tumba")]
    private GameObject _graveKeyPrefab;
    [SerializeField, Tooltip("Sonido de llave")]
    private AudioClip _keySpawnSound;

    [Header("NotaMadre chimenea")]
    [SerializeField, Tooltip("Nota madre chimenea")]
    private GameObject _notaMadrePrefab;

    [Header("Dialogos")]
    [SerializeField] private GameObject _chimneyExitDialogue;

    [Header("Configuración")]
    [SerializeField] private float _animationDuration;
    [SerializeField] private float _interactionDistance;
    [SerializeField] private LayerMask _interactableMask;

    private bool _isUsing = false;

    void Start()
    {
        if (_shovelVisual != null)
        {
            _shovelVisual.SetActive(false);
        }
        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;
        }
    }

    void Update()
    {
        if (Keyboard.current == null) return;
        if (Keyboard.current.gKey.wasPressedThisFrame)
        {
            bool tienePala = _inventory.HasItem(PlayerInventory.ItemType.Shovel);
            if (tienePala && !_isUsing)
            {
                TryInteractWithShovel();
            }
        }
    }

    private void TryInteractWithShovel()
    {
        Ray ray = _mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, _interactionDistance, _interactableMask))
        {
            if (hit.collider.CompareTag("Chimney") || hit.collider.CompareTag("Grave"))
            {
                StartCoroutine(UseShovelRoutine(hit.collider.gameObject));
            }
        }
    }

    private IEnumerator UseShovelRoutine(GameObject targetObject)
    {
        _isUsing = true;
        _shovelVisual.SetActive(true);

        if (_audioSource != null && _shovelSound != null)
        {
            _audioSource.PlayOneShot(_shovelSound);
        }
        if (_shovelAnimator != null)
        {
            _shovelAnimator.SetTrigger("Dig");
        }

        yield return new WaitForSeconds(_animationDuration * 0.5f);

        GameObject llave = null;
        GameObject nota = null;

        if (targetObject.CompareTag("Chimney"))
        {
            llave = _chimneyKeyPrefab;
            nota = _notaMadrePrefab;
            if (_chimneyExitDialogue != null)
                _chimneyExitDialogue.SetActive(true);
        }
        else if (targetObject.CompareTag("Grave"))
        {
            llave = _graveKeyPrefab;
        }

        if (nota != null)
        {
            Vector3 spawnPosition = targetObject.transform.position + new Vector3(0f, 0.1f, 0f);
            Instantiate(nota, spawnPosition, Quaternion.identity);
        }

        if (llave != null)
        {
            Vector3 spawnPosition = targetObject.transform.position + new Vector3(0f, 0.1f, 0f);
            Instantiate(llave, spawnPosition, Quaternion.identity);
            if (_audioSource != null && _keySpawnSound != null)
            {
                _audioSource.PlayOneShot(_keySpawnSound);
            }
        }

        targetObject.tag = "Untagged";
        yield return new WaitForSeconds(_animationDuration * 0.5f);
        _shovelVisual.SetActive(false);
        _isUsing = false;
    }
}