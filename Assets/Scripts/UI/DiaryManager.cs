using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class DiaryManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject _diaryPanel;
    [SerializeField] private TextMeshProUGUI _objectiveText;
    [SerializeField] private TextMeshProUGUI _inventoryText;

    [Header("Dependencies")]
    [SerializeField, Tooltip("Arrastra al jugador aquí para leer su inventario")]
    private PlayerInventory _playerInventory;

    private PlayerInputActions _inputActions;
    private bool _isOpen = false;

    private void Awake()
    {
        _inputActions = new PlayerInputActions();

        _inputActions.Gameplay.OpenDiary.performed += ctx => ToggleDiary();

        _inputActions.UI.Cancel.performed += ctx => { if (_isOpen) ToggleDiary(); };
    }

    private void OnEnable()
    {
        _inputActions.Enable();
    }

    private void OnDisable()
    {
        _inputActions.Disable();
    }

    public void ToggleDiary()
    {
        _isOpen = !_isOpen;
        _diaryPanel.SetActive(_isOpen);

        if (_isOpen)
        {
            UpdateDiaryContent();

            Time.timeScale = 0f;

            _inputActions.Gameplay.Disable();
            _inputActions.UI.Enable();
        }
        else
        {
            Time.timeScale = 1f;

            _inputActions.UI.Disable();
            _inputActions.Gameplay.Enable();
        }
    }

    private void UpdateDiaryContent()
    {
        if (ObjectiveManager.Instance != null)
        {
            _objectiveText.text = "Objetivo Actual:\n" + ObjectiveManager.Instance.GetCurrentObjective();
        }

        if (_playerInventory != null)
        {
            string inventoryString = "Inventario:\n\n";

            // Aquí le preguntas a tu script PlayerInventory qué llaves tiene. 
            // Suponiendo que tienes un método o revisas los booleanos/lista:
            if (_playerInventory.HasItem(PlayerInventory.ItemType.MansionKey))
            {
                inventoryString += "- Llave de la Mansión\n";
            }
            if (_playerInventory.HasItem(PlayerInventory.ItemType.PatioKey)) 
            {
                inventoryString += "- Llave del Patio\n";
            }

            // Agregamos consumibles como baterías
            // inventoryString += $"\nBaterías de Linterna: {_playerInventory.GetBatteryCount()}\n";

            if (inventoryString == "Inventario:\n\n")
            {
                inventoryString += "(Vacío)";
            }

            _inventoryText.text = inventoryString;
        }
    }
}