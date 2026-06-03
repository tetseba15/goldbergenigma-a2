using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Mixer & Snapshots")]
    [SerializeField] private AudioMixer _mainMixer;
    [SerializeField] private AudioMixerSnapshot _normalSnapshot;
    [SerializeField] private AudioMixerSnapshot _chaseSnapshot;
    [SerializeField] private AudioMixerSnapshot _dialogueSnapshot;

    private bool _isCurrentlyChasing = false;

    [Header("Global Audio Sources (2D)")]
    [SerializeField] private AudioSource _musicAudioSource;
    [SerializeField] private AudioSource _ambienceAudioSource;
    [SerializeField] private AudioSource _sfxAudioSource;

    [Header("3D Audio Pool Settings")]
    [SerializeField, Tooltip("Cantidad de reproductores simultáneos permitidos")]
    private int _poolSize = 15;
    [SerializeField, Tooltip("Prefab vacío con un AudioSource en modo 3D")]
    private GameObject _audioSourcePrefab;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip _chaseMusicClip;
    [SerializeField] private AudioClip _explorationMusicClip;

    [Header("Chase Settings")]
    [SerializeField, Tooltip("Segundos a esperar tras perder de vista al jugador antes de calmar la música")]
    private float _chaseEndDelay = 4f;

    private Coroutine _chaseEndCoroutine;

    //Audio source pool
    private Queue<AudioSource> _sfxPool;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializePool();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        EnemyAI.OnChaseStateChanged += HandleChaseState;
    }

    private void OnDisable()
    {
        EnemyAI.OnChaseStateChanged -= HandleChaseState;
    }

    public void SetDialogueState(bool isTalking)
    {
        if (isTalking)
        {
            _dialogueSnapshot.TransitionTo(0.25f);
        }
        else
        {

            if (_isCurrentlyChasing)
            {
                _chaseSnapshot.TransitionTo(0.4f);
            }
            else
            {
                _normalSnapshot.TransitionTo(0.6f);
            }
        }
    }

    private void HandleChaseState(bool isChasing)
    {
        if (isChasing)
        {
            if (_chaseEndCoroutine != null)
            {
                StopCoroutine(_chaseEndCoroutine);
                _chaseEndCoroutine = null;
            }

            SetChaseState(true, 1.5f);
        }
        else
        {
            if (_chaseEndCoroutine == null && gameObject.activeInHierarchy)
            {
                _chaseEndCoroutine = StartCoroutine(DelayedChaseEndRoutine());
            }
        }
    }

    private void InitializePool()
    {
        _sfxPool = new Queue<AudioSource>();

        GameObject poolContainer = new GameObject("SFX_Pool");
        poolContainer.transform.SetParent(transform);

        for (int i = 0; i < _poolSize; i++)
        {
            GameObject obj = Instantiate(_audioSourcePrefab, poolContainer.transform);
            AudioSource source = obj.GetComponent<AudioSource>();
            source.playOnAwake = false;

            _sfxPool.Enqueue(source);
        }
    }

    /// <summary>
    /// Reproduce un sonido global/2D (Interfaz, agarrar llaves, leer notas)
    /// </summary>
    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip != null)
        {
            _sfxAudioSource.PlayOneShot(clip, volume);
        }
    }

    /// <summary>
    /// Reproduce un sonido posicional/3D (Puertas, pasos, enemigos, impactos)
    /// </summary>
    public void PlaySFXAtPosition(AudioClip clip, Vector3 position, float volume = 1f, float pitch = 1f)
    {
        if (clip == null || _sfxPool.Count == 0) return;

        AudioSource source = _sfxPool.Dequeue();

        source.transform.position = position;
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;

        source.Play();

        _sfxPool.Enqueue(source);
    }

    public void SetChaseState(bool isChasing, float transitionTime = 2f)
    {
        _isCurrentlyChasing = isChasing;

        if (isChasing)
        {
            _musicAudioSource.clip = _chaseMusicClip;
            _musicAudioSource.Play();

            _chaseSnapshot.TransitionTo(transitionTime);
        }
        else
        {
            _musicAudioSource.clip = _explorationMusicClip;
            _musicAudioSource.Play();

            _normalSnapshot.TransitionTo(transitionTime);
        }
    }

    private IEnumerator DelayedChaseEndRoutine()
    {
        yield return new WaitForSeconds(_chaseEndDelay);

        
        SetChaseState(false, 6f);

        _chaseEndCoroutine = null;
    }
}