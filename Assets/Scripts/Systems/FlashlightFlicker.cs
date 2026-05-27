using System.Collections;
using UnityEngine;

public class FlashlightFlicker : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Tooltip("The actual Light component of the flashlight")]
    private Light _lightComponent;
    [SerializeField, Tooltip("Reference to the main flashlight script to check its state")]
    private PlayerFlashlight _playerFlashlight;

    [Header("Flicker Settings")]
    [SerializeField, Tooltip("Minimum time between flickers")]
    private float _minDelay = 0.05f;
    [SerializeField, Tooltip("Maximum time between flickers")]
    private float _maxDelay = 0.15f;

    private float _originalIntensity;
    private Coroutine _flickerCoroutine;

    private void Start()
    {
        if (_lightComponent != null)
        {
            _originalIntensity = _lightComponent.intensity;
        }
    }

    private void OnEnable()
    {
        EnemyAI.OnEnemyRoaring += StartFlicker;
    }

    private void OnDisable()
    {
        EnemyAI.OnEnemyRoaring -= StartFlicker;
    }

    private void StartFlicker(float duration)
    {
        if (_playerFlashlight != null && !_playerFlashlight.IsOn()) return;

        if (_flickerCoroutine != null)
        {
            StopCoroutine(_flickerCoroutine);
        }

        _flickerCoroutine = StartCoroutine(FlickerRoutine(duration));
    }

    private IEnumerator FlickerRoutine(float duration)
    {
        _playerFlashlight.IsIntensityHijacked = true;

        float elapsedTimer = 0f;

        while (elapsedTimer < duration)
        {
            _lightComponent.intensity = Random.value > 0.5f ? _originalIntensity : 0f;

            float randomDelay = Random.Range(_minDelay, _maxDelay);
            yield return new WaitForSeconds(randomDelay);
            elapsedTimer += randomDelay;
        }

        _lightComponent.intensity = _originalIntensity;
        _playerFlashlight.IsIntensityHijacked = false;
    }
}