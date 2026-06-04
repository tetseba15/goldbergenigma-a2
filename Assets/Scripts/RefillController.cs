using UnityEngine;

public class RefillController : MonoBehaviour, IInteractable
{
    [Header("Configuración de UI")]
    [SerializeField, Tooltip("El mensaje que aparecerá en el HUD al mirar el lavamanos")]
    private string _sinkPrompt = "Presiona [E] para rellenar la botella";

    [Header("Audio de Agua (Opcional)")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _refillSound;

    
    public string GetInteractPrompt(GameObject player)
    {
        PlayerInventory inventory = player.GetComponent<PlayerInventory>();

        if (inventory != null && inventory.HasItem(PlayerInventory.ItemType.Bottle))
        {
            return _sinkPrompt; 
        }

        return string.Empty; 
    }

    
    public void Interact(GameObject player)
    {
        
        PlayerInventory inventory = player.GetComponent<PlayerInventory>();
        if (inventory == null || !inventory.HasItem(PlayerInventory.ItemType.Bottle)) return;

        
        HolyWaterController holyWater = player.GetComponentInChildren<HolyWaterController>();

        if (holyWater != null)
        {
            holyWater.RefillBottle();

            if (_audioSource != null && _refillSound != null)
            {
                _audioSource.PlayOneShot(_refillSound);
            }
        }
    }
}