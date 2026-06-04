using System;
using UnityEngine;

public class BatteryPickup : MonoBehaviour, IInteractable
{
    [Header("Settings")]
    [SerializeField] private int _batteriesToGive = 1;
    [SerializeField] private string _promptText = "[E] Tomar pilas";
    [SerializeField] private AudioClip _batteryPickupSFX;

    public string GetInteractPrompt(GameObject interactor) => _promptText;

    public void Interact(GameObject interactor)
    {

        PlayerInventory inventory = interactor.GetComponent<PlayerInventory>();

        if (inventory != null)
        {
            inventory.AddBatteries(_batteriesToGive);

            if (_batteryPickupSFX != null)
                AudioManager.Instance.PlaySFX(_batteryPickupSFX, .5f);

            Destroy(gameObject);
        }

    }
}