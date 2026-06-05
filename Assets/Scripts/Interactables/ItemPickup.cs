using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class ItemPickup : MonoBehaviour, IInteractable
{
    [SerializeField] private PlayerInventory.ItemType _itemType;
    [SerializeField] private string _promptText = "Recoger objeto";
    [SerializeField] private AudioClip _pickUpClip;

    [Header("Requiere Ouija")]
    [SerializeField] private bool _requiresOuijaAct = false;
    [SerializeField] private bool _requiresAct2Ouija = false;

    [Header("Dialogo al recoger")]
    [SerializeField] private bool _hasPickupDialogue = false;
    [SerializeField, TextArea(2, 5)] private string _pickupDialogue;

    public static event Action<PlayerInventory, PlayerInventory.ItemType> OnInteract;

    public string GetInteractPrompt(GameObject interactor)
    {
        if (_requiresAct2Ouija)
        {
            OuijaBoard ouija = OuijaBoard.Instance;
            if (ouija != null && !ouija.HasUsedAct2Ouija)
                return string.Empty;
        }
        return _promptText;
    }

    public void Interact(GameObject interactor)
    {
        if (_requiresAct2Ouija)
        {
            OuijaBoard ouija = OuijaBoard.Instance;
            if (ouija != null && !ouija.HasUsedAct2Ouija) return;
        }

        PlayerInventory inventory = interactor.GetComponent<PlayerInventory>();
        if (inventory != null)
        {
            inventory.AddItem(_itemType);

            if (_hasPickupDialogue && !string.IsNullOrEmpty(_pickupDialogue))
            {
                DialogueManager.Instance.ShowDialogue(_pickupDialogue);
            }

            if (_pickUpClip != null)
            {
                AudioManager.Instance.PlaySFX(_pickUpClip, .35f);
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
            if (_itemType == PlayerInventory.ItemType.WorkshopKey)
            {
                OuijaBoard.Instance.AdvanceToNextAct();
                OuijaBoard.Instance.ResetCooldown();
            }

            OnInteract?.Invoke(interactor.GetComponent<PlayerInventory>(), _itemType);
            Destroy(gameObject);
        }
    }
}