using Unity.VisualScripting;
using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractable
{
    [SerializeField] private PlayerInventory.ItemType _itemType;
    [SerializeField] private string _promptText = "Recoger objeto";
    [SerializeField] private AudioClip _pickUpClip;

    public string GetInteractPrompt(GameObject interactor) => _promptText;

    public void Interact(GameObject interactor)
    {
        PlayerInventory inventory = interactor.GetComponent<PlayerInventory>();

        if (inventory != null)
        {
            inventory.AddItem(_itemType);

            if (_pickUpClip != null)
            {
                AudioManager.Instance.PlaySFX(_pickUpClip, .5f);
            }

            if (_itemType == PlayerInventory.ItemType.Flashlight)
            {
                PlayerFlashlight flashlight = interactor.GetComponent<PlayerFlashlight>();
                
                if (flashlight != null)
                    flashlight.PickupFlashlight();
            }
            if (_itemType == PlayerInventory.ItemType.Bottle)
            {
                
                HolyWaterController waterController = interactor.GetComponent<HolyWaterController>();

                if (waterController != null)
                {
                    waterController.RefillBottle();
                }
            }

            Destroy(gameObject);
        }
    }
}
