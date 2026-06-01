using TMPro;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private AudioSource _musicAudioSource;
    [SerializeField] private AudioSource _ambienceAudioSource;
    [SerializeField] private AudioSource _sfxAudioSource;

    public bool IsReadingNote { get; private set; }

    private PlayerInputHandler _inputHandler;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySFX(AudioClip clip, float volume)
    {
        _sfxAudioSource.PlayOneShot(clip, volume);
    }
}
