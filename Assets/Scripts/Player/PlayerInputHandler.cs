using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerInputActions _inputActions;
    private PlayerFlashlight _playerFlashlight;

    public event System.Action OnCancelTriggered;
    public event System.Action OnInteractTriggered;

    public event System.Action OnPauseTriggered;
    public event System.Action OnResumeTriggered;

    [Header("Configuración de Interacción Física")]
    [SerializeField, Range(0f, 1f), Tooltip("Qué tan rápido se mueve la cámara al arrastrar un objeto (0 = bloqueada, 1 = normal)")]
    private float _grabLookSensitivity = 0.15f;

    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public Vector2 RawLookInput { get; private set; }

    public bool IsGamepad { get; private set; }

    public bool IsSprinting { get; private set; }
    public bool IsCrouching { get; private set; }
    public bool IsInteracting { get; private set; }
    public bool IsPhysicsGrabbing { get; private set; }
    public bool IsInspectingFlashlight { get; private set; }
    public bool CancelInput { get; private set; }
    public bool FlashlightInput { get; private set; }

    private void Awake()
    {
        _inputActions = new PlayerInputActions();
        _playerFlashlight = GetComponent<PlayerFlashlight>();

        _inputActions.Gameplay.FlashlightToggle.performed += ctx =>
        {
            if (_playerFlashlight != null)
            {
                _playerFlashlight.ToggleFlashlight();
            }
        };


        _inputActions.Gameplay.InspectFlashlight.performed += ctx =>
        {
            IsInspectingFlashlight = true;
            if (_playerFlashlight != null) _playerFlashlight.SetInspectState(true); // <-- AVISAMOS A LA LINTERNA
        };

        _inputActions.Gameplay.InspectFlashlight.canceled += ctx =>
        {
            IsInspectingFlashlight = false;
            if (_playerFlashlight != null) _playerFlashlight.SetInspectState(false); // <-- AVISAMOS A LA LINTERNA
        };

        _inputActions.Gameplay.Reload.performed += ctx =>
        {
            if (_playerFlashlight != null)
            {
                _playerFlashlight.TryReload();
            }
        };

        _inputActions.UI.Cancel.performed += ctx => OnCancelTriggered?.Invoke();

        //---------------------------------------------------
        // Suscribe events for one or mantain pressed buttons
        //---------------------------------------------------

        _inputActions.Gameplay.Sprint.performed += ctx => IsSprinting = true;
        _inputActions.Gameplay.Sprint.canceled += ctx => IsSprinting = false;

        _inputActions.Gameplay.Interact.performed += ctx =>
        {
            IsInteracting = true;
            OnInteractTriggered?.Invoke(); 
        };

        _inputActions.Gameplay.Pause.performed += ctx => OnPauseTriggered?.Invoke();

        _inputActions.UI.Cancel.performed += ctx => OnResumeTriggered?.Invoke();

        _inputActions.Gameplay.Interact.canceled += ctx => IsInteracting = false;

        _inputActions.Gameplay.Crouch.performed += ctx => IsCrouching = true;
        _inputActions.Gameplay.Crouch.canceled += ctx => IsCrouching = false;

        _inputActions.Gameplay.InspectFlashlight.performed += ctx => IsInspectingFlashlight= true;
        _inputActions.Gameplay.InspectFlashlight.canceled += ctx => IsInspectingFlashlight = false;

        _inputActions.Gameplay.PhysicsGrab.performed += ctx => IsPhysicsGrabbing = true;
        _inputActions.Gameplay.PhysicsGrab.canceled += ctx => IsPhysicsGrabbing = false;
    }

    private void OnEnable()
    {
        _inputActions.Gameplay.Enable();
    }

    private void OnDisable()
    {
        _inputActions.Gameplay.Disable();
    }

    void Update()
    {
        MoveInput = _inputActions.Gameplay.Move.ReadValue<Vector2>();

        var lookAction = _inputActions.Gameplay.Look;

        RawLookInput = lookAction.ReadValue<Vector2>(); 

        if(lookAction.activeControl != null)
        {
            IsGamepad = lookAction.activeControl.device is Gamepad;
        }

        if (IsPhysicsGrabbing)
        {
            LookInput = RawLookInput * _grabLookSensitivity; 
        }
        else
        {
            LookInput = RawLookInput; 
        }

        if (lookAction.activeControl != null)
        {
            IsGamepad = lookAction.activeControl.device is Gamepad;
        }

        CancelInput = _inputActions.Gameplay.Cancel.WasPressedThisFrame();

        FlashlightInput = _inputActions.Gameplay.FlashlightToggle.WasPressedThisFrame();
    }

    public void EnableUIControls()
    {
        _inputActions.Gameplay.Disable();
        _inputActions.UI.Enable();
    }

    public void EnableGameplayControls()
    {
        _inputActions.UI.Disable();
        _inputActions.Gameplay.Enable();
    }

    public void ConsumeInteractInput()
    {
        IsInteracting = false;
    }
}
