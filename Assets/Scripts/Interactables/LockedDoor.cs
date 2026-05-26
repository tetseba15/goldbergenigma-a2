using UnityEngine;

public class LockedDoor : MonoBehaviour, IInteractable
{
    [Header("Settings")]
    [SerializeField] private PlayerInventory.ItemType _requiredKey = PlayerInventory.ItemType.MansionKey;
    [SerializeField] private string _lockedMessage = "Está cerrada con llave.";
    [SerializeField] private string _unlockedMessage = "Abrir puerta";
    [SerializeField] private string _closedMessage = "Cerrar puerta";

    private bool _isUnlocked = false;
    private bool _isOpen = false;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public string GetInteractPrompt(GameObject interactor)
    {
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
        if (_isUnlocked)
        {
            if (!_isOpen)
            {
                OpenDoor();
            }
            else
            {
                CloseDoor();
            }

            return;
        }

        PlayerInventory inventory = interactor.GetComponent<PlayerInventory>();
        if (inventory != null && inventory.HasItem(_requiredKey))
        {
            UnlockDoor();
        }
        else
        {
            Debug.Log("La puerta no cede...");
        }
    }

    private void UnlockDoor()
    {
        _isUnlocked = true;
        OpenDoor();
        Debug.Log("Puerta abierta. Bienvenido a la mansión.");
    }

    private void OpenDoor()
    {
        // Animation?
        animator.SetTrigger("Open");
        _isOpen = true;
        //transform.Rotate(0, -90, 0);
    }

    private void CloseDoor()
    {
        animator.SetTrigger("Close");
        _isOpen = false;
    }
}