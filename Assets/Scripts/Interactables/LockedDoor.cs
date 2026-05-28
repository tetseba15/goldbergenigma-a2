using UnityEngine;
using System.Collections;

public class LockedDoor : MonoBehaviour, IInteractable
{
    [Header("Door State")]
    [SerializeField, Tooltip("If false, the door opens without needing a key.")]
    private bool _requiresKey = true;

    [Header("Key Settings (Ignored if Requires Key is false)")]
    [SerializeField] private PlayerInventory.ItemType _requiredKey = PlayerInventory.ItemType.MansionKey;
    [SerializeField] private string _lockedMessage = "Está cerrada con llave.";

    [Header("Interaction Prompts")]
    [SerializeField] private string _unlockedMessage = "Abrir puerta";
    [SerializeField] private string _closedMessage = "Cerrar puerta";

    [Header("Animation Settings")]
    [SerializeField, Tooltip("How long the open/close animation takes to finish")]
    private float _animationDuration = 1.2f;

    private bool _isUnlocked = false;
    private bool _isOpen = false;

    private Animator _animator;
    private bool _isAnimating = false;

    private void Awake()
    {
        _animator = GetComponent<Animator>();

        if (!_requiresKey)
        {
            _isUnlocked = true;
        }
    }

    public string GetInteractPrompt(GameObject interactor)
    {
        if (_isAnimating) return string.Empty;

        if (_isOpen && _isUnlocked) return _closedMessage;
        if (!_isOpen && _isUnlocked) return _unlockedMessage;

        PlayerInventory inventory = interactor.GetComponent<PlayerInventory>();
        if (inventory != null)
        {
            return inventory.HasItem(_requiredKey) ? _unlockedMessage : _lockedMessage;
        }

        return _lockedMessage;
    }

    public void Interact(GameObject interactor)
    {
        if (_isAnimating) return;

        if (_isUnlocked)
        {
            StartCoroutine(DoorAnimationRoutine());
            return;
        }

        PlayerInventory inventory = interactor.GetComponent<PlayerInventory>();
        if (inventory != null && inventory.HasItem(_requiredKey))
        {
            UnlockDoor();
        }
        else
        {
            // TODO: Play "Rattle handle" locked sound effect
            Debug.Log("La puerta no cede...");
        }
    }
    
    private void UnlockDoor()
    {
        _isUnlocked = true;
        // TODO: Play "Unlock" sound effect

        StartCoroutine(DoorAnimationRoutine());
    }

    private IEnumerator DoorAnimationRoutine()
    {
        _isAnimating = true;

        if (!_isOpen)
        {
            _animator.SetTrigger("Open");
            _isOpen = true;
        }
        else
        {
            _animator.SetTrigger("Close");
            _isOpen = false;
        }

        yield return new WaitForSeconds(_animationDuration);

        _isAnimating = false;
    }
}