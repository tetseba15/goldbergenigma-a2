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
    [SerializeField, Tooltip("Gasp for jumpscares/spotting")]
    private AudioClip _initialGaspClip;
    [SerializeField, Tooltip("Looping panting during chase")]
    private AudioClip _chasePantingLoop;
    [SerializeField, Tooltip("Sigh of relief when chase ends")]
    private AudioClip _reliefSighClip;
    [SerializeField, Tooltip("Heartbeat loop")]
    private AudioClip _heartbeatClip;

    private Coroutine _panicCoroutine;
    private Coroutine _jumpscareAudioCoroutine;
    private Coroutine _fadeHeartbeatCoroutine;

    private bool _isChasing = false;



    private void OnEnable()
    {
        EnemyAI.OnEnemyRoaring += TriggerJumpscare;
        EnemyAI.OnChaseStateChanged += HandleChaseState;
    }

    private void OnDisable()
    {
        EnemyAI.OnEnemyRoaring -= TriggerJumpscare;
        EnemyAI.OnChaseStateChanged -= HandleChaseState;
    }


    public void TriggerJumpscare(float roarDuration, float invulnerabilityDuration)
    {
        if (_panicCoroutine != null) StopCoroutine(_panicCoroutine);
        if (_jumpscareAudioCoroutine != null) StopCoroutine(_jumpscareAudioCoroutine);

        float stunDuration = roarDuration / 2f;

        _panicCoroutine = StartCoroutine(PanicRoutine(stunDuration));
        _jumpscareAudioCoroutine = StartCoroutine(JumpscareAudioSequence(stunDuration));
    }

    private IEnumerator JumpscareAudioSequence(float stunDuration)
    {
        if (_heartbeatSource && _heartbeatClip)
        {
            if (_fadeHeartbeatCoroutine != null) StopCoroutine(_fadeHeartbeatCoroutine);
            _heartbeatSource.clip = _heartbeatClip;
            _heartbeatSource.loop = true;
            _heartbeatSource.volume = 1f;
            if (!_heartbeatSource.isPlaying) _heartbeatSource.Play();
        }

        if (_voiceSource && _initialGaspClip)
        {
            _voiceSource.pitch = Random.Range(0.95f, 1.05f);
            _voiceSource.PlayOneShot(_initialGaspClip); 
        }

        yield return new WaitForSeconds(stunDuration);

        
        if (!_isChasing)
        {
            _fadeHeartbeatCoroutine = StartCoroutine(FadeOutHeartbeat());
        }
    }

    private void HandleChaseState(bool isChasing)
    {
        _isChasing = isChasing;

        if (isChasing)
        {
            
            if (_voiceSource && _chasePantingLoop)
            {
                _voiceSource.clip = _chasePantingLoop;
                _voiceSource.loop = true;
                _voiceSource.pitch = Random.Range(0.95f, 1.05f);
                if (!_voiceSource.isPlaying) _voiceSource.Play();
            }
        }
        else
        {
            if (_voiceSource)
            {
                _voiceSource.loop = false;
                _voiceSource.Stop();
            }

            if (_voiceSource && _reliefSighClip)
            {
                _voiceSource.pitch = Random.Range(0.95f, 1.05f);
                _voiceSource.PlayOneShot(_reliefSighClip);
            }

            _fadeHeartbeatCoroutine = StartCoroutine(FadeOutHeartbeat());
        }
    }

    private IEnumerator FadeOutHeartbeat()
    {
        if (_heartbeatSource == null || !_heartbeatSource.isPlaying) yield break;

        float fadeDuration = 1.5f;
        float startVolume = _heartbeatSource.volume;

        while (_heartbeatSource.volume > 0)
        {
            _heartbeatSource.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        _heartbeatSource.Stop();
        _heartbeatSource.volume = 1f; 
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