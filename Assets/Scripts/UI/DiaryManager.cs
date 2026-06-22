using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DiaryManager : MonoBehaviour
{
    public static DiaryManager Instance { get; private set; }

    // --- Out events ---
    public static event Action<bool> OnDiaryStateChanged;
    public static event Action<string, string, string> OnDiaryDataUpdated; // Objectives, Keys, Batteries

    [Header("Dependencies")]
    [SerializeField] private PlayerInventory _playerInventory;

    private PlayerInputActions _inputActions;
    private bool _isOpen = false;
    public bool IsOpen() => _isOpen;

    // ADD KEYS HERE
    private readonly Dictionary<PlayerInventory.ItemType, string> _itemNames = new Dictionary<PlayerInventory.ItemType, string>
    {
        { PlayerInventory.ItemType.BathroomKey, "Llave del baño" },
        { PlayerInventory.ItemType.MansionKey, "Llave de la Mansión" },
        { PlayerInventory.ItemType.PatioKey, "Llave del Patio" },
        { PlayerInventory.ItemType.QuinchoKey, "Llave del Quincho" },
        { PlayerInventory.ItemType.OfficeKey, "Llave de la Oficina" },
        { PlayerInventory.ItemType.WorkshopKey, "Llave del Taller" }
    };

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        _inputActions = new PlayerInputActions();
        _inputActions.Gameplay.OpenDiary.performed += ctx => ToggleDiary();
        _inputActions.UI.Cancel.performed += ctx => { if (_isOpen) ToggleDiary(); };
    }

    private void OnEnable() => _inputActions.Enable();
    private void OnDisable() => _inputActions.Disable();

    public void ToggleDiary()
    {
        _isOpen = !_isOpen;

        OnDiaryStateChanged?.Invoke(_isOpen);

        if (_isOpen)
        {
            UpdateDiaryData();
        }
    }

    public void UpdateDiaryData()
    {
        if (!_isOpen) return;

        // 1. Obtener Objetivos (asumiendo que tienes una función similar, la puliremos en el paso 2)
        string objectiveText = ObjectiveManager.Instance != null ? ObjectiveManager.Instance.GetCurrentObjective() : "";

        string batteryString = $"Baterías:\n\n {_playerInventory.BatteryCount}";

        string inventoryString = "Llaves:\n\n";
        bool hasKeys = false;

        foreach (var item in _playerInventory.GetCollectedItems())
        {
            if (_itemNames.TryGetValue(item, out string itemName))
            {
                inventoryString += $"- {itemName}\n";
                hasKeys = true;
            }
        }

        if (!hasKeys) inventoryString += "(Vacío)";

        OnDiaryDataUpdated?.Invoke(objectiveText, inventoryString, batteryString);
    }
}