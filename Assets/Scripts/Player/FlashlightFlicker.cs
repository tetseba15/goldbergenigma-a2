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
    private Coroutine _interferenceCoroutine;  

    private bool _isInterfering = false;

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
        EnemyAI.OnFlashlightInterference += HandleInterference; 
    }

    private void OnDisable()
    {
        EnemyAI.OnEnemyRoaring -= StartFlicker;
        EnemyAI.OnFlashlightInterference -= HandleInterference;
    }

   
    private void HandleInterference(bool isInterfering)
    {
        if (_isInterfering == isInterfering) return;

        _isInterfering = isInterfering;

        if (_playerFlashlight != null && !_playerFlashlight.IsOn()) return;

        if (_isInterfering)
        {
            if (_interferenceCoroutine == null)
            {
                _interferenceCoroutine = StartCoroutine(InterferenceRoutine());
            }
        }
        else
        {
            if (_interferenceCoroutine != null)
            {
                StopCoroutine(_interferenceCoroutine);
                _interferenceCoroutine = null;

                if (_flickerCoroutine == null)
                {
                    RestoreFlashlight();
                }
            }
        }
    }

    private IEnumerator InterferenceRoutine()
    {
        _playerFlashlight.IsIntensityHijacked = true;

        while (true)
        {
            float realIntensity = _playerFlashlight.BaseIntensity;

            _lightComponent.intensity = Random.Range(0.1f, realIntensity * 0.6f);

            yield return new WaitForSeconds(Random.Range(0.02f, 0.08f));
        }
    }

   
    private void StartFlicker(float roarDuration, float invulnerabilityDuration)
    {
        if (_playerFlashlight != null && !_playerFlashlight.IsOn()) return;

        if (_flickerCoroutine != null)
        {
            StopCoroutine(_flickerCoroutine);
        }

        _flickerCoroutine = StartCoroutine(FlickerRoutine(invulnerabilityDuration));
    }

    private IEnumerator FlickerRoutine(float duration)
    {
        _playerFlashlight.IsIntensityHijacked = true;
        float elapsedTimer = 0f;

        while (elapsedTimer < duration)
        {
            float realIntensity = _playerFlashlight.BaseIntensity;

            _lightComponent.intensity = Random.value > 0.5f ? realIntensity : 0f;

            float randomDelay = Random.Range(_minDelay, _maxDelay);
            yield return new WaitForSeconds(randomDelay);
            elapsedTimer += randomDelay;
        }

        _flickerCoroutine = null;

        if (!_isInterfering)
        {
            RestoreFlashlight();
        }
    }

   
    private void RestoreFlashlight()
    {
        _playerFlashlight.IsIntensityHijacked = false;

        if (_lightComponent != null && _playerFlashlight != null)
        {
            _lightComponent.intensity = _playerFlashlight.BaseIntensity;
        }
    }
}