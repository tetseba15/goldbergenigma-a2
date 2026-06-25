using System.Collections;
using UnityEngine;

public class LightningController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Light _lightningLight;
    [SerializeField] private AudioClip[] _thunderClips;

    [Header("Configuración del Relámpago")]
    [SerializeField] private float _minTimeBetweenStrikes = 5f;
    [SerializeField] private float _maxTimeBetweenStrikes = 15f;
    [SerializeField] private float _maxIntensity = 5f;
    [SerializeField] private float _baseIntensity = 0f;

    [Header("Retraso del Sonido")]
    [SerializeField, Tooltip("Velocidad a la que viaja el sonido para simular distancia")]
    private float _soundDelayMultiplier = 0.5f;

    private Coroutine _lightningRoutine;

    private void Start()
    {
        if (_lightningLight != null)
        {
            _lightningLight.intensity = _baseIntensity;
            _lightningLight.enabled = false;
        }

        StartNextStrike();
    }

    private void StartNextStrike()
    {
        float waitTime = Random.Range(_minTimeBetweenStrikes, _maxTimeBetweenStrikes);
        _lightningRoutine = StartCoroutine(LightningRoutine(waitTime));
    }

    private IEnumerator LightningRoutine(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        if (_lightningLight != null)
        {
            _lightningLight.enabled = true;

            int flashCount = Random.Range(1, 4);
            for (int i = 0; i < flashCount; i++)
            {
                _lightningLight.intensity = Random.Range(_maxIntensity * 0.5f, _maxIntensity);
                yield return new WaitForSeconds(Random.Range(0.05f, 0.15f));

                _lightningLight.intensity = _baseIntensity;
                yield return new WaitForSeconds(Random.Range(0.05f, 0.2f));
            }

            _lightningLight.enabled = false;
        }

        if (_thunderClips.Length > 0 && AudioManager.Instance != null)
        {
            float soundDelay = Random.Range(0.5f, 2f) * _soundDelayMultiplier;
            yield return new WaitForSeconds(soundDelay);

            AudioClip clip = _thunderClips[Random.Range(0, _thunderClips.Length)];
            AudioManager.Instance.PlaySFX(clip, Random.Range(0.3f, 0.4f));
        }

        StartNextStrike();
    }
}