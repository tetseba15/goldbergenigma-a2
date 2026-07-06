using UnityEngine;

public class PurificationZone : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Objeto necesario para purificar la zona")]
    private PlayerInventory.ItemType _itemType;

    private bool _isPlayerInside;
    public bool IsPurified { get; private set; }

    private void OnTriggerEnter(Collider other)
    {
        _isPlayerInside = true;
    }

    private void OnTriggerExit(Collider other)
    {
        _isPlayerInside = false;
    }

    private void OnEnable()
    {
        CrossController.OnCrossUse += PurifyZone;
    }

    private void OnDisable()
    {
        CrossController.OnCrossUse -= PurifyZone;
    }

    private void PurifyZone(PlayerInventory.ItemType itemType)
    {
        Debug.Log("Purification item: " +  itemType);
        if (itemType == _itemType && _isPlayerInside) IsPurified = true;
    }
}
