using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class ItemPickup : MonoBehaviour, IInteractable
{
    [SerializeField] private PlayerInventory.ItemType _itemType;
    [SerializeField] private string _promptText = "Recoger objeto";
    [SerializeField] private AudioClip _pickUpClip;

    [Header("Dialogo al recoger")]
    [SerializeField] private bool _hasPickupDialogue = false;
    [SerializeField, TextArea(2, 5)] private string _pickupDialogue;

    [Header("Activa objeto al recoger")]
    [SerializeField] private GameObject _objectToActivate;

    private ObjectiveReporter objectiveReporter;

    public static event Action<PlayerInventory, PlayerInventory.ItemType> OnInteract;

    private void Start()
    {
        objectiveReporter = GetComponent<ObjectiveReporter>();
    }

    public string GetInteractPrompt(GameObject interactor)
    {
        return _promptText;
    }

    public void Interact(GameObject interactor)
    {
        OuijaBoard ouija = OuijaBoard.Instance;

        PlayerInventory inventory = interactor.GetComponent<PlayerInventory>();
        if (inventory != null)
        {
            inventory.AddItem(_itemType);

            if (_hasPickupDialogue && !string.IsNullOrEmpty(_pickupDialogue))
            {
                DialogueManager.Instance.ShowDialogue(_pickupDialogue);
            }

            if (_objectToActivate != null)
                _objectToActivate.SetActive(true);

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

            if (objectiveReporter != null) objectiveReporter.ReportObjective();
            OnInteract?.Invoke(interactor.GetComponent<PlayerInventory>(), _itemType);
            Destroy(gameObject);
        }
    }
}