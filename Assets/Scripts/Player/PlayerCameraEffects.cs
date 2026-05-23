using UnityEngine;

public class PlayerCameraEffects : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private PlayerInputHandler _inputHandler;

    [Header("Balanceo de Cámara (Headbob)")]
    [SerializeField] private float _walkBobFrequency = 10f; // Walk Speed
    [SerializeField] private float _walkBobAmplitude = 0.05f; // Up/Down
    [SerializeField] private float _sprintBobFrequency = 14f; // Run Speed
    [SerializeField] private float _sprintBobAmplitude = 0.1f; // Up/Down

    [Header("Compensación por Esprint (Sprint Lean)")]    
    [SerializeField] private float _sprintZOffset = 0.2f;  // Z offset while running
    [SerializeField] private float _leanSmoothSpeed = 5f; // Smoothness

    private float _bobTimer;
    private Vector3 _defaultLocalPosition;

    private float _currentZOffset;

    private void Awake()
    {
        _defaultLocalPosition = transform.localPosition;
    }

    private void Update()
    {
        if (UIManager.Instance != null && UIManager.Instance.IsReadingNote) return;

        HandleSprintLean();
        HandleHeadbob();
    }

    private void HandleSprintLean()
    {
        float targetZ = (_inputHandler.IsSprinting && _playerMovement.CurrentSpeed > 0.1f) ? _sprintZOffset : _defaultLocalPosition.z;

        _currentZOffset = Mathf.Lerp(_currentZOffset, targetZ, Time.deltaTime * _leanSmoothSpeed);
    }

    private void HandleHeadbob()
    {
        float speed = _playerMovement.CurrentSpeed;

        if (speed < 0.1f)
        {
            _bobTimer = 0f;
            Vector3 idlePos = _defaultLocalPosition;
            idlePos.z += _currentZOffset;
            transform.localPosition = Vector3.Lerp(transform.localPosition, idlePos, Time.deltaTime * _leanSmoothSpeed);
            return;
        }

        bool isSprinting = _inputHandler.IsSprinting;
        float frequency = isSprinting ? _sprintBobFrequency : _walkBobFrequency;
        float amplitude = isSprinting ? _sprintBobAmplitude : _walkBobAmplitude;

        _bobTimer += Time.deltaTime * frequency;

       
        float bobY = Mathf.Sin(_bobTimer) * amplitude;

        float bobX = Mathf.Cos(_bobTimer * 0.5f) * (amplitude * 0.5f);

        Vector3 finalPosition = _defaultLocalPosition;
        finalPosition.x += bobX;
        finalPosition.y += bobY;
        finalPosition.z += _currentZOffset; 

        transform.localPosition = finalPosition;
    }
}
