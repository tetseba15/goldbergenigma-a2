using UnityEngine;

public class EnemyAudioManager : MonoBehaviour
{
    [Header("Audio Channels")]
    [SerializeField, Tooltip("AudioSource for breathing, roars, and pain")]
    private AudioSource _voiceSource;
    [SerializeField, Tooltip("AudioSource strictly for movement and footsteps")]
    private AudioSource _footstepSource;

    [Header("Voice Clips")]
    [SerializeField] private AudioClip _idleBreathingLoop;
    [SerializeField] private AudioClip _attackRoar;
    [SerializeField] private AudioClip _hurtClip;
    [SerializeField] private AudioClip _enragedClip;

    [Header("Movement Clips")]
    [SerializeField] private AudioClip[] _footstepClips;

    [Header("Dynamic Distances")]
    [SerializeField, Tooltip("Max distance for intimate sounds like breathing")]
    private float _whisperMaxDistance = 12f;
    [SerializeField, Tooltip("Max distance for loud sounds like roars and screams")]
    private float _shoutMaxDistance = 35f;

    [Header("Protection Settings")]
    [SerializeField, Tooltip("Time in seconds before the hurt sound can play again")]
    private float _hurtCooldown = 1.5f;
    private float _lastHurtTime = -100f; 

    private void Start()
    {
        _voiceSource.maxDistance = _whisperMaxDistance;
        PlayIdleBreathing();
    }

    // --- VOICE CHANNEL ---

    public void PlayIdleBreathing()
    {
        if (_voiceSource.clip != _idleBreathingLoop)
        {
            _voiceSource.clip = _idleBreathingLoop;
            _voiceSource.loop = true;
            _voiceSource.maxDistance = _whisperMaxDistance; 
            _voiceSource.Play();
        }
    }

    public void PlayAttack()
    {
        _voiceSource.loop = false;
        _voiceSource.maxDistance = _shoutMaxDistance; 
        _voiceSource.PlayOneShot(_attackRoar);
    }

    public void PlayHurt()
    {
        // Anti-spam protection
        if (Time.time - _lastHurtTime < _hurtCooldown) return;

        _lastHurtTime = Time.time; 

        _voiceSource.loop = false;
        _voiceSource.maxDistance = _shoutMaxDistance; 
        _voiceSource.PlayOneShot(_hurtClip);
    }

    public void PlayEnraged()
    {
        _voiceSource.loop = false;
        _voiceSource.maxDistance = _shoutMaxDistance;
        _voiceSource.PlayOneShot(_enragedClip);
    }

    // --- MOVEMENT CHANNEL ---

    // Called via Animation Events
    public void PlayFootstep()
    {
        if (_footstepClips == null || _footstepClips.Length == 0) return;

        // Randomize pitch slightly for organic variation
        _footstepSource.pitch = Random.Range(0.9f, 1.1f);

        AudioClip randomStep = _footstepClips[Random.Range(0, _footstepClips.Length)];
        _footstepSource.PlayOneShot(randomStep);
    }
}