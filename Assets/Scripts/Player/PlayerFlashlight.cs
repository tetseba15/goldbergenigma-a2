using UnityEngine;

[RequireComponent(typeof(PlayerInputHandler))]
[RequireComponent(typeof(PlayerInventory))]
public class PlayerFlashlight : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Tooltip("The spotlight of the player")]
    private Light _lightComponent;

    [Header("Batery Settings")]
    [SerializeField] private float _maxBattery = 100f;
    [SerializeField, Tooltip("Battery drain per second")]
    private float _drainRate = 1f;
    [SerializeField, Tooltip("% when the flashlights begins to malfunction")]
    private float _flickerThreshold = 20f;

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

        _currentBattery = _maxBattery;
    }

    private void Update()
    {
        HandleToggle();

        if (_isOn)
        {
            DrainBattery();
        }
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
        // On SFX
    }

    private void TurnOff()
    {
        _isOn = false;
        _lightComponent.enabled = false;
        // Off SFX
    }

    // Recharge bateries later?
    public void RechargeBattery(float amount)
    {
        _currentBattery = Mathf.Clamp(_currentBattery + amount, 0f, _maxBattery);
    }
    
}