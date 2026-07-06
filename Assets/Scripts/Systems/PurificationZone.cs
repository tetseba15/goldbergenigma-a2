using System;
using UnityEngine;

public class PurificationZone : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Objeto necesario para purificar la zona")]
    private PlayerInventory.ItemType _itemType;

    private bool _isPlayerInside;
    public bool IsPurified { get; private set; }

    public static event Action<Vector3> OnPurified;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _isPlayerInside = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
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
        if (itemType == _itemType && _isPlayerInside)
        {
            IsPurified = true;
            OnPurified?.Invoke(transform.position);
        }
    }
}
