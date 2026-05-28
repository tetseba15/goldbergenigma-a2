using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public enum ItemType 
    {
        // KEYS
        MansionKey,
        OfficeKey,
        QuinchoKey,

        // ITEMS
        Flashlight, 
        Crucifix, 
        Note, 
        Bottle, 
        Cross, 
        Shovel
    }

    private HashSet<ItemType> _items = new HashSet<ItemType>();

    public void AddItem(ItemType type)
    {
        if (!_items.Contains(type))
        {
            _items.Add(type);
            Debug.Log($"New Object: {type}");
        }
    }

    public bool HasItem(ItemType type)
    {
        return _items.Contains(type);
    }
}
