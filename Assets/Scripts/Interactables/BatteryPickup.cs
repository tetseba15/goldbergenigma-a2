using UnityEngine;

public class BatteryPickup : MonoBehaviour, IInteractable
{
    [Header("Settings")]
    [SerializeField] private int _batteriesToGive = 1;
    [SerializeField] private string _promptText = "[E] Tomar pilas";

    public string GetInteractPrompt(GameObject interactor) => _promptText;

    public void Interact(GameObject interactor)
    {

        PlayerInventory inventory = interactor.GetComponent<PlayerInventory>();

        if (inventory != null)
        {
            inventory.AddBatteries(_batteriesToGive);

            // TODO: FX de recoger ítem (sonido de guardar en el bolsillo)

            Destroy(gameObject);
        }

    }
}