using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public enum ItemType 
    {
        // KEYS
        MansionKey = 1,
        OfficeKey = 2,
        QuinchoKey = 3,
        PatioKey = 4,

        // ITEMS AND TOOLS
        Flashlight = 100, 
        Crucifix = 101, 
        Bottle = 102, 
        Cross = 103, 
        Shovel = 104,

        // NOTES
        Note = 200 
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
