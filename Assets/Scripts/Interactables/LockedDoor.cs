using UnityEngine;

public class LockedDoor : MonoBehaviour, IInteractable
{
    [Header("Settings")]
    [SerializeField] private PlayerInventory.ItemType _requiredKey = PlayerInventory.ItemType.MansionKey;
    [SerializeField] private string _lockedMessage = "Está cerrada con llave.";
    [SerializeField] private string _unlockedMessage = "Abrir puerta";

    private bool _isOpened = false;

   

    public string GetInteractPrompt(GameObject interactor)
    {
        if (_isOpened) return "";

        PlayerInventory inventory = interactor.GetComponent<PlayerInventory>();
        if (inventory != null)
        {
            return inventory.HasItem(_requiredKey) ? _unlockedMessage : _lockedMessage;
        }

        return _lockedMessage;
    }

    public void Interact(GameObject interactor)
    {
        if (_isOpened) return;

        PlayerInventory inventory = interactor.GetComponent<PlayerInventory>();
        if (inventory != null && inventory.HasItem(_requiredKey))
        {
            OpenDoor();
        }
        else
        {
            Debug.Log("La puerta no cede...");
        }
    }

    private void OpenDoor()
    {
        _isOpened = true;
        Debug.Log("Puerta abierta. Bienvenido a la mansión.");

        // Animation?
        transform.Rotate(0, -90, 0); 
    }
}