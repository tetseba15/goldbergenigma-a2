using System;
using UnityEngine;

[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerLook : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Tooltip("Empty object at eyes (Not the camera itself)")]
    private Transform _cameraRoot;

    [Header("Cam Config")]
    [field: SerializeField, Range(1f, 100f  ), Tooltip("Base Sensitivity")]
    public float LookSensitivity { get; private set; } = 15f;

    [field: SerializeField, Range(100f, 500f), Tooltip(" Gamepad Base Sensitivity")]
    public float GamepadMultiplier { get; private set; } = 300f;


    [field: SerializeField, Tooltip("Neck limit")]
    public float UpDownLimit { get; private set; } = 85f;

    private PlayerInputHandler _inputHandler;
    private float _xRotation = 0f;

    private void Awake()
    {
        _inputHandler = GetComponent<PlayerInputHandler>();

        //Block mouse at the center of the screen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    void Update()
    {
        if (UIManager.Instance != null && UIManager.Instance.IsReadingNote) return;

        HandleCamera();
    }

    private void HandleCamera()
    {
        Vector2 lookInput = _inputHandler.LookInput;

        float internalScaler = 0.05f;

        float mouseX = lookInput.x * LookSensitivity * internalScaler;
        float mouseY = lookInput.y * LookSensitivity * internalScaler;

        //GAMEPAD FIX 
        if (_inputHandler.IsGamepad)
        {
            mouseX *= Time.deltaTime * GamepadMultiplier;
            mouseY *= Time.deltaTime * GamepadMultiplier;
        }

        //Warning: Positive rotations in X axis makes you look down,
        //thats why must be substracted (-) the Y movement

        _xRotation -= mouseY;

        //Clamp angles
        _xRotation = Mathf.Clamp(_xRotation, -UpDownLimit, UpDownLimit);

        //Up-Down: Rotate "neck" (Camera Root)
        _cameraRoot.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);

        //Left-Right: Rotate "body"
        transform.Rotate(Vector3.up * mouseX);
    }
}
