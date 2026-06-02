using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class DiaryManager : MonoBehaviour
{
    public static DiaryManager Instance;

    [Header("UI Elements")]
    [SerializeField] private GameObject _diaryPanel;
    [SerializeField] private TextMeshProUGUI _objectiveText;
    [SerializeField] private TextMeshProUGUI _keysText;
    [SerializeField] private TextMeshProUGUI _batteryText;

    [Header("Dependencies")]
    [SerializeField, Tooltip("Arrastra al jugador aquí para leer su inventario")]
    private PlayerInventory _playerInventory;

    private PlayerInputActions _inputActions;
    private bool _isOpen = false;
    public bool IsOpen() => _isOpen;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

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
            _objectiveText.text = ObjectiveManager.Instance.GetCurrentObjective();
        }

        if (_playerInventory != null)
        {
            string inventoryString = "Llaves:\n\n";
            string batteryInventoryString = "Baterías: \n\n";

            
            if (_playerInventory.HasItem(PlayerInventory.ItemType.MansionKey))
            {
                inventoryString += "- Llave de la Mansión\n";
            }
            if (_playerInventory.HasItem(PlayerInventory.ItemType.PatioKey)) 
            {
                inventoryString += "- Llave del Patio\n";
            }

            if (_playerInventory.HasItem(PlayerInventory.ItemType.QuinchoKey))
            {
                inventoryString += "- Llave del Quincho\n";
            }

            if (_playerInventory.HasItem(PlayerInventory.ItemType.OfficeKey))
            {
                inventoryString += "- Llave de la oficina\n";
            }

            batteryInventoryString += $"\n {_playerInventory.BatteryCount}\n";

            if (inventoryString == "Llaves:\n\n")
            {
                inventoryString += "(Vacío)";
            }

            _keysText.text = inventoryString;
            _batteryText.text = batteryInventoryString;
        }
    }
}