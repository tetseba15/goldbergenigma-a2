using UnityEngine;

public interface IPurificable
{
    PurificableData PurificationData { get; }

    void Purify(PlayerInventory.ItemType itemType, Vector3 purifySourcePosition);
}
