using UnityEngine;
using System;

[RequireComponent(typeof(PlayerInputHandler))]
[RequireComponent(typeof(PlayerInventory))]
public class PlayerFlashlight : MonoBehaviour
{
    //                0.0 a 1.0
    public event Action<float> OnBatteryChanged;

    public bool IsIntensityHijacked { get; set; } = false;

    public float BaseIntensity { get; private set; }
    private float _basePointIntensity;


    [Header("Events / Tutorials")]
    [SerializeField] private FlashlightTutorial _tutorialSystem;

    [Header("References")]
    [SerializeField, Tooltip("The spotlight of the player")]
    private Light _lightComponent;
    [SerializeField, Tooltip("The pointlight of the player")]
    private Light _pointLightComponent;
    [SerializeField, Tooltip("Flashlight Mesh renderer")]
    private MeshRenderer _flashlightMeshRenderer;
    [SerializeField, Tooltip("Flashlight Canvas")]
    private Canvas _canvasIndicator;

    [SerializeField] private GameObject _flashlight;


    [Header("Batery Settings")]
    [SerializeField] private float _maxBattery = 100f;
    [SerializeField, Tooltip("Battery drain per second")]
    private float _drainRate = 1f;
    [SerializeField, Tooltip("% when the flashlights begins to malfunction")]
    private float _flickerThreshold = 20f;


    [Header("Audio")]
    [SerializeField, Tooltip("AudioSource for flashlight clicks")]
    private AudioSource _audioSource;
    [SerializeField, Tooltip("Sound when turning on")]
    private AudioClip _turnOnSound;
    [SerializeField, Tooltip("Sound when turning off")]
    private AudioClip _turnOffSound;


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

    private PlayerInputHandler _inputHandler;
    private PlayerInventory _inventory;

    private void Awake()
    {
        _inputHandler = GetComponent<PlayerInputHandler>();
        _inventory = GetComponent<PlayerInventory>();

        if (_lightComponent != null)
        {
            BaseIntensity = _lightComponent.intensity;
            _basePointIntensity = _pointLightComponent.intensity;

            _lightComponent.intensity = 0f;
            _pointLightComponent.intensity = 0f;

            _lightComponent.enabled = true;
            _pointLightComponent.enabled = true;
        }

        // Optimization      
        if (_flashlightMeshRenderer != null) _flashlightMeshRenderer.enabled = false;
        if (_canvasIndicator != null) _canvasIndicator.enabled = false;

        _currentBattery = _maxBattery;
    }

    private void Start()
    {
        _normalLocalRotation = Quaternion.Euler(_normalLocalRotationEuler);
        _inspectLocalRotation = Quaternion.Euler(_inspectLocalRotationEuler);

        //_lightComponent.intensity = 0f;
        //_pointLightComponent.intensity = 0f;

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

        if (!IsIntensityHijacked)
        {
            if (_currentBattery <= _flickerThreshold)
            {
                float noise = Mathf.PerlinNoise(Time.time * 10f, 0f);
                _lightComponent.intensity = Mathf.Lerp(0f, BaseIntensity, noise);
            }
            else
            {
                _lightComponent.intensity = BaseIntensity;
            }
        }
    }

    private void TurnOn()
    {
        _isOn = true;

        if (!IsIntensityHijacked)
        {
            _lightComponent.intensity = BaseIntensity;
            _pointLightComponent.intensity = _basePointIntensity;
        }

        if (_audioSource != null && _turnOnSound != null)
        {
            _audioSource.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
            _audioSource.PlayOneShot(_turnOnSound);
        }
    }

    private void TurnOff()
    {
        _isOn = false;



        _lightComponent.intensity = 0f;
        _pointLightComponent.intensity = 0f;

        if (_audioSource != null && _turnOffSound != null)
        {
            _audioSource.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
            _audioSource.PlayOneShot(_turnOffSound);
        }
    }

    public void RechargeBattery(float amount)
    {
        _currentBattery = Mathf.Clamp(_currentBattery + amount, 0f, _maxBattery);
    }

    public void PickupFlashlight()
    {
        if (_flashlightMeshRenderer != null) _flashlightMeshRenderer.enabled = true;
        if (_canvasIndicator != null) _canvasIndicator.enabled = true;

        if (_tutorialSystem != null)
            _tutorialSystem.TriggerTutorial();
    }

    private void HandleInspection()
    {
        if (_flashlightModel == null) return;

        if (!IsOn() && !_inputHandler.IsInspectingFlashlight) return;

        bool isInspecting = _inputHandler.IsInspectingFlashlight;

        // Destiny (transform & rotation) based on inspect bool
        Vector3 targetPosition = isInspecting ? _inspectLocalPosition : _normalLocalPosition;
        Quaternion targetRotation = isInspecting ? _inspectLocalRotation : _normalLocalRotation;

        _flashlightModel.localPosition = Vector3.Lerp(_flashlightModel.localPosition, targetPosition, Time.deltaTime * _inspectSpeed);
        _flashlightModel.localRotation = Quaternion.Lerp(_flashlightModel.localRotation, targetRotation, Time.deltaTime * _inspectSpeed);
    }

}