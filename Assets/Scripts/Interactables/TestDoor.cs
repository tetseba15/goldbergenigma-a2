using UnityEngine;

public class TestDoor : MonoBehaviour, IInteractable
{
    private bool _isOpen = false;

    public string GetInteractPrompt(GameObject interactor)
    {
        return _isOpen ? "Cerrar puerta" : "Abrir puerta";
    }

    public void Interact(GameObject interactor)
    {
        _isOpen = !_isOpen;
        Debug.Log("Estado de la puerta modificado: " + _isOpen);

        // Animation or rotation logic...

    }
}