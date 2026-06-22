using System;
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
        BathroomKey = 5,
        WorkshopKey = 6,

        ActLockKey = 99,
        // ITEMS AND TOOLS
        Flashlight = 100,
        Crucifix = 101,
        Bottle = 102,
        Cross = 103,
        Shovel = 104,
        OuijaBoard = 105,
        Lighter = 106,
        // NOTES
        Note = 200
    }


    private HashSet<ItemType> _items = new HashSet<ItemType>();
    private bool _hasSeenBatteryTutorial = false;
    public int BatteryCount { get; private set; } = 0;

    public static event Action<ItemType> OnItemCollected;
    public static event Action<int> OnBatteryCountChanged;

    public void AddItem(ItemType type)
    {
        if (!_items.Contains(type))
        {
            _items.Add(type);
            Debug.Log($"New Object: {type}");

            OnItemCollected?.Invoke(type);
        }
    }

    public bool HasItem(ItemType type)
    {
        return _items.Contains(type);
    }

    public IEnumerable<ItemType> GetCollectedItems()
    {
        return _items;
    }

    public void AddBatteries(int amount)
    {
        BatteryCount += amount;
        Debug.Log($"Baterías recogidas. Total: {BatteryCount}");

        OnBatteryCountChanged?.Invoke(BatteryCount);

        if (!_hasSeenBatteryTutorial)
        {
            //TutorialManager.Instance.ShowTutorial("Presiona [R] para recargar la linterna",
            //    () => GetComponent<PlayerFlashlight>().IsReloading());

            TutorialManager.RequestConditionTutorial?.Invoke("Presiona [R] para recargar la linterna", () => GetComponent<PlayerFlashlight>().IsReloading());
            _hasSeenBatteryTutorial = true;
        }
    }

    public void ConsumeBattery()
    {
        if (BatteryCount > 0)
        {
            BatteryCount--;

            OnBatteryCountChanged?.Invoke(BatteryCount);
        }
    }
}