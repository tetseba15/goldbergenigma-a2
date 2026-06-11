using UnityEngine;
using UnityEngine.UI;

public class HudItem : MonoBehaviour
{
    [SerializeField] private PlayerInventory.ItemType _itemType;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
    }

    private void Start()
    {
        Debug.Log("Bottle alpha: " + canvasGroup.alpha);
    }

    private void OnEnable()
    {
        ItemPickup.OnInteract += ShowItemInHUD;
    }

    private void OnDisable()
    {
        ItemPickup.OnInteract -= ShowItemInHUD;
    }

    private void ShowItemInHUD(PlayerInventory playerInventory, PlayerInventory.ItemType itemType)
    {
        if (playerInventory.HasItem(itemType) && itemType == _itemType)
        {
            canvasGroup.alpha = 1f; 
        }
    }
}
