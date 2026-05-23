using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerInputActions _inputActions;

    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }

    public bool IsGamepad { get; private set; }

    public bool IsSprinting { get; private set; }
    public bool IsCrouching { get; private set; }
    public bool IsInteracting { get; private set; }
    public bool CancelInput { get; private set; }
    public bool FlashlightInput { get; private set; }

    private void Awake()
    {
        _inputActions = new PlayerInputActions();

        //Suscribe events for one or mantain pressed buttons

        _inputActions.Gameplay.Sprint.performed += ctx => IsSprinting = true;
        _inputActions.Gameplay.Sprint.canceled += ctx => IsSprinting = false;

        _inputActions.Gameplay.Interact.performed += ctx => IsInteracting = true;
        _inputActions.Gameplay.Interact.canceled += ctx => IsInteracting = false;

        _inputActions.Gameplay.Crouch.performed += ctx => IsCrouching = true;
        _inputActions.Gameplay.Crouch.canceled += ctx => IsCrouching = false;
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
        LookInput = lookAction.ReadValue<Vector2>();

        if(lookAction.activeControl != null)
        {
            IsGamepad = lookAction.activeControl.device is Gamepad;
        }

        CancelInput = _inputActions.Gameplay.Cancel.WasPressedThisFrame();

        FlashlightInput = _inputActions.Gameplay.Flashlight.WasPressedThisFrame();
    }

    public void ConsumeInteractInput()
    {
        IsInteracting = false;
    }
}
