using UnityEngine;
using System;

[RequireComponent(typeof(PlayerInputHandler))]
[RequireComponent(typeof(PlayerInventory))]
public class PlayerFlashlight : MonoBehaviour
{
    //                0.0 a 1.0
    public event Action<float> OnBatteryChanged;

    [Header("References")]
    [SerializeField, Tooltip("The spotlight of the player")]
    private Light _lightComponent;
    [SerializeField, Tooltip("Flashlight Mesh renderer")]
    private MeshRenderer _flashlightMeshRenderer;


    [Header("Batery Settings")]
    [SerializeField] private float _maxBattery = 100f;
    [SerializeField, Tooltip("Battery drain per second")]
    private float _drainRate = 1f;
    [SerializeField, Tooltip("% when the flashlights begins to malfunction")]
    private float _flickerThreshold = 20f;


    [Header("Inspection Animation")]
    [SerializeField, Tooltip("Flashlight Model (child of camera)")]
    private Transform _flashlightModel;

    [SerializeField] private float _inspectSpeed = 8f;

    [Space(10)]
    [SerializeField] private Vector3 _normalLocalPosition;
    [SerializeField] private Vector3 _inspectLocalPosition;

    [Space(10)]
    [SerializeField] private Vector3 _normalLocalRotationEuler;
    [SerializeField] private Vector3 _inspectLocalRotationEuler;

    private Quaternion _normalLocalRotation;
    private Quaternion _inspectLocalRotation;



    private bool _isOn = false;
    public bool IsOn() => _isOn;

    private float _currentBattery;
    private float _baseIntensity;

    private PlayerInputHandler _inputHandler;
    private PlayerInventory _inventory;

    private void Awake()
    {
        _inputHandler = GetComponent<PlayerInputHandler>();
        _inventory = GetComponent<PlayerInventory>();

        if (_lightComponent != null)
        {
            _baseIntensity = _lightComponent.intensity;
            _lightComponent.enabled = false;
        }

        if (_flashlightMeshRenderer != null)
            _flashlightMeshRenderer.enabled = false;

        _currentBattery = _maxBattery;
    }

    private void Start()
    {
        _normalLocalRotation = Quaternion.Euler(_normalLocalRotationEuler);
        _inspectLocalRotation = Quaternion.Euler(_inspectLocalRotationEuler);

        if (_flashlightModel != null)
        {
            _normalLocalPosition = _flashlightModel.localPosition;
            _normalLocalRotationEuler = _flashlightModel.localEulerAngles;
            _normalLocalRotation = _flashlightModel.localRotation;
        }
    }

    private void Update()
    {
        HandleToggle();

        if (_isOn)
        {
            DrainBattery();
        }

        HandleInspection();
    }

    private void HandleToggle()
    {
        if (_inputHandler.FlashlightInput && _inventory.HasItem(PlayerInventory.ItemType.Flashlight))
        {
            if (!_isOn && _currentBattery > 0f)
            {
                TurnOn();
            }
            else if (_isOn)
            {
                TurnOff();
            }
        }
    }

    private void DrainBattery()
    {
        _currentBattery -= _drainRate * Time.deltaTime;
        _currentBattery = Mathf.Clamp(_currentBattery, 0f, _maxBattery);


        OnBatteryChanged?.Invoke(_currentBattery / _maxBattery);

        if (_currentBattery <= 0f)
        {
            _currentBattery = 0f;
            TurnOff();
            return;
        }

        if (_currentBattery <= _flickerThreshold)
        {
            // Mathf.PerlinNoise generates a soft curve, good for broken lights
            float noise = Mathf.PerlinNoise(Time.time * 10f, 0f);
            _lightComponent.intensity = Mathf.Lerp(0f, _baseIntensity, noise);
        }
        else
        {
            _lightComponent.intensity = _baseIntensity;
        }
    }

    private void TurnOn()
    {
        _isOn = true;
        _lightComponent.enabled = true;
        if (_flashlightMeshRenderer != null)
            _flashlightMeshRenderer.enabled = true;
        // On SFX
    }

    private void TurnOff()
    {
        _isOn = false;
        _lightComponent.enabled = false;
        if (_flashlightMeshRenderer != null)
            _flashlightMeshRenderer.enabled = false;
        // Off SFX
    }

    public void RechargeBattery(float amount)
    {
        _currentBattery = Mathf.Clamp(_currentBattery + amount, 0f, _maxBattery);
    }

    private void HandleInspection()
    {
        if (_flashlightModel == null) return;

        bool isInspecting = _inputHandler.IsInspectingFlashlight;

        // Destiny (transform & rotation) based on inspect bool
        Vector3 targetPosition = isInspecting ? _inspectLocalPosition : _normalLocalPosition;
        Quaternion targetRotation = isInspecting ? _inspectLocalRotation : _normalLocalRotation;

        _flashlightModel.localPosition = Vector3.Lerp(_flashlightModel.localPosition, targetPosition, Time.deltaTime * _inspectSpeed);
        _flashlightModel.localRotation = Quaternion.Lerp(_flashlightModel.localRotation, targetRotation, Time.deltaTime * _inspectSpeed);
    }

}