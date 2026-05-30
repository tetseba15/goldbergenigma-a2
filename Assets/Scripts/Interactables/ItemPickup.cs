using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractable
{
    [SerializeField] private PlayerInventory.ItemType _itemType;
    [SerializeField] private string _promptText = "Recoger objeto";

    public string GetInteractPrompt(GameObject interactor) => _promptText;

    public void Interact(GameObject interactor)
    {
        PlayerInventory inventory = interactor.GetComponent<PlayerInventory>();

        if (inventory != null)
        {
            inventory.AddItem(_itemType);

            if (_itemType == PlayerInventory.ItemType.Flashlight)
            {
                PlayerFlashlight flashlight = interactor.GetComponent<PlayerFlashlight>();
                
                if (flashlight != null)
                    flashlight.PickupFlashlight();
            }
            // Agrego para la recarga de la botella en la UI
            if (_itemType == PlayerInventory.ItemType.Bottle)
            {
                
                HolyWaterController waterController = interactor.GetComponent<HolyWaterController>();

                // Si la tiene, Llena la botella y la UI 
                if (waterController != null)
                {
                    waterController.RefillBottle();
                }
            }

            Destroy(gameObject);
        }
    }
}
