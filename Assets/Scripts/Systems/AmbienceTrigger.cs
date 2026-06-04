using UnityEngine;

public class AmbienceTrigger : MonoBehaviour
{
    public enum TriggerMode { SimpleChange, TemporaryImpact }

    [Header("Tipo de Comportamiento")]
    [SerializeField] private TriggerMode _mode = TriggerMode.SimpleChange;

    [Header("Configuración Base")]
    [SerializeField] private AudioClip _primaryAmbience;
    [SerializeField] private float _fadeDuration = 3f;
    [SerializeField] private bool _triggerOnlyOnce = false;

    [Header("Configuración de Impacto (Susto / Jumpscare)")]
    [Tooltip("El track tranquilo al que irá el juego después de que termine el susto")]
    [SerializeField] private AudioClip _calmFallbackAmbience;
    [Tooltip("Cuánto tiempo durará el ambiente estridente antes de empezar a calmarse")]
    [SerializeField] private float _impactDuration = 5f;
    [SerializeField] private float _joltIntroFade = 0.3f;

    private bool _hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (_hasTriggered && _triggerOnlyOnce) return;

        if (other.CompareTag("Player"))
        {
            if (AudioManager.Instance == null) return;

            switch (_mode)
            {
                case TriggerMode.SimpleChange:
                    AudioManager.Instance.ChangeAmbience(_primaryAmbience, _fadeDuration);
                    break;

                case TriggerMode.TemporaryImpact:
                    AudioManager.Instance.PlayTemporaryAmbience(
                        _primaryAmbience,
                        _calmFallbackAmbience,
                        _impactDuration,
                        _joltIntroFade,
                        _fadeDuration
                    );
                    break;
            }

            _hasTriggered = true;
        }
    }
}