using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.Cinemachine; 

public class PlayerPanicReaction : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Tooltip("Reference to the player's movement script or speed variable")]
    private PlayerMovement _playerMovementScript; 

    [Header("Visual & Camera Effects")]
    [SerializeField, Tooltip("A local URP Volume dedicated to panic effects")]
    private Volume _panicVolume;
    [SerializeField, Tooltip("Cinemachine Impulse Source to shake the camera")]
    private CinemachineImpulseSource _impulseSource;

    [Header("Settings")]
    [SerializeField, Range(0f, 1f), Tooltip("How much to slow down the player (0.2 = 20% speed)")]
    private float _speedMultiplier = 0.2f;
    [SerializeField, Tooltip("How fast the blur fades in and out")]
    private float _visualFadeSpeed = 5f;

    [Header("Audio: Panic Sequence")]
    [SerializeField, Tooltip("Source exclusively for voice (gasps/panting)")]
    private AudioSource _voiceSource;
    [SerializeField, Tooltip("Source exclusively for the heartbeat")]
    private AudioSource _heartbeatSource;

    [Space(5)]
    [SerializeField] private AudioClip _initialGaspClip;
    [SerializeField] private AudioClip _pantingClip;
    [SerializeField] private AudioClip _heartbeatClip;

    private Coroutine _audioSequenceCoroutine;

    private Coroutine _panicCoroutine;

    private void OnEnable()
    {
        EnemyAI.OnEnemyRoaring += TriggerPanic;
    }

    private void OnDisable()
    {
        EnemyAI.OnEnemyRoaring -= TriggerPanic;
    }

    private void TriggerPanic(float roarDuration, float invulnerabilityDuration)
    {
        if (_panicCoroutine != null) StopCoroutine(_panicCoroutine);
        if (_audioSequenceCoroutine != null) StopCoroutine(_audioSequenceCoroutine);

        float stunDuration = roarDuration / 2f;

        _panicCoroutine = StartCoroutine(PanicRoutine(stunDuration));

        _audioSequenceCoroutine = StartCoroutine(PanicAudioSequence(stunDuration));
    }

    private IEnumerator PanicAudioSequence(float stunDuration)
    {
        // Hearth beating starts
        if (_heartbeatSource != null && _heartbeatClip != null)
        {
            _heartbeatSource.clip = _heartbeatClip;
            _heartbeatSource.loop = true; 
            _heartbeatSource.volume = 1f;
            _heartbeatSource.Play();
        }

        // Then the player breaths in
        float gaspDuration = 0f;
        if (_voiceSource != null && _initialGaspClip != null)
        {
            _voiceSource.pitch = Random.Range(0.95f, 1.05f);
            _voiceSource.PlayOneShot(_initialGaspClip);
            gaspDuration = _initialGaspClip.length;
        }

        
        // *0.8 to overlap vfx
        yield return new WaitForSeconds(gaspDuration * 0.8f);

        // next  ispanting
        if (_voiceSource != null && _pantingClip != null)
        {
            _voiceSource.pitch = Random.Range(0.95f, 1.05f);
            _voiceSource.PlayOneShot(_pantingClip);
        }

        
        float remainingTime = stunDuration - (gaspDuration * 0.8f);
        if (remainingTime > 0)
        {
            yield return new WaitForSeconds(remainingTime);
        }

        // Small Fade-out
        if (_heartbeatSource != null)
        {
            float fadeDuration = 1f;
            float startVolume = _heartbeatSource.volume;

            while (_heartbeatSource.volume > 0)
            {
                _heartbeatSource.volume -= startVolume * Time.deltaTime / fadeDuration;
                yield return null;
            }

            _heartbeatSource.Stop();
            _heartbeatSource.volume = startVolume; 
        }
    }

    private IEnumerator PanicRoutine(float stunDuration)
    {

        _playerMovementScript.SpeedMultiplier *= _speedMultiplier;

        if (_impulseSource != null)
        {
            _impulseSource.GenerateImpulse();
        }

        // FADE IN VISUAL DISTORTION (URP Volume)
        if (_panicVolume != null)
        {
            while (_panicVolume.weight < 1f)
            {
                _panicVolume.weight = Mathf.MoveTowards(_panicVolume.weight, 1f, Time.deltaTime * _visualFadeSpeed);
                yield return null;
            }
        }

        yield return new WaitForSeconds(stunDuration);


        _playerMovementScript.SpeedMultiplier = 1.0f;

        if (_panicVolume != null)
        {
            while (_panicVolume.weight > 0f)
            {
                _panicVolume.weight = Mathf.MoveTowards(_panicVolume.weight, 0f, Time.deltaTime * (_visualFadeSpeed / 2f)); // Fades out slower for effect
                yield return null;
            }
        }
    }
}