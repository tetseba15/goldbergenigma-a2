using UnityEngine;

public class BatteryPickup : MonoBehaviour, IInteractable
{
    [Header("Configuración")]
    [SerializeField] private float _rechargeAmount = 25f;
    [SerializeField] private string _promptText = "[E] Tomar pilas";

    public string GetInteractPrompt(GameObject interactor) => _promptText;

    public void Interact(GameObject interactor)
    {

        PlayerFlashlight flashlight = interactor.GetComponent<PlayerFlashlight>();

        if (flashlight != null)
        {
            flashlight.RechargeBattery(_rechargeAmount);

            // FX HERE

            Debug.Log($"Linterna recargada con {_rechargeAmount}%");

            Destroy(gameObject);
        }

    }
}