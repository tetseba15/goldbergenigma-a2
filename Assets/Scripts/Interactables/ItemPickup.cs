using System;
using Unity.VisualScripting;
using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractable
{
    [SerializeField] private PlayerInventory.ItemType _itemType;
    [SerializeField] private string _promptText = "Recoger objeto";
    [SerializeField] private AudioClip _pickUpClip;

    [Header("Requiere Ouija")]
    [SerializeField] private bool _requiresOuijaAct = false;
    [SerializeField] private int _requiredAct = 2;

    public static event Action<PlayerInventory, PlayerInventory.ItemType> OnInteract;

    public string GetInteractPrompt(GameObject interactor)
    {
        if (_requiresOuijaAct)
        {
            OuijaBoard ouija = FindObjectOfType<OuijaBoard>();
            if (ouija != null && ouija.CurrentAct < _requiredAct)
                return string.Empty;
        }
        return _promptText;
    }

    public void Interact(GameObject interactor)
    {
        if (_requiresOuijaAct)
        {
            OuijaBoard ouija = FindObjectOfType<OuijaBoard>();
            if (ouija != null && ouija.CurrentAct < _requiredAct) return;
        }

        PlayerInventory inventory = interactor.GetComponent<PlayerInventory>();
        if (inventory != null)
        {
            inventory.AddItem(_itemType);
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
            OnInteract?.Invoke(interactor.GetComponent<PlayerInventory>(), _itemType);
            Destroy(gameObject);
        }
    }
}