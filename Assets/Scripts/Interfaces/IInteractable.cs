using UnityEngine;

public interface IInteractable
{
    string GetInteractPrompt(GameObject interactor);

    void Interact(GameObject interactor);
}
