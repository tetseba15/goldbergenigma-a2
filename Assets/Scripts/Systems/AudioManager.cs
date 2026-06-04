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

    [Header("Ambience Crossfade")]
    [SerializeField, Tooltip("Reproductor de ambiente A")]
    private AudioSource _ambienceSource1;
    [SerializeField, Tooltip("Reproductor de ambiente B")]
    private AudioSource _ambienceSource2;
    [SerializeField, Tooltip("Volumen máximo del ambiente")]
    private float _maxAmbienceVolume = 1f;

    private Coroutine _temporaryAmbienceRoutine;

    private bool _isSource1Active = true;
    private Coroutine _crossfadeRoutine;

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

    public void ChangeAmbience(AudioClip newAmbienceClip, float fadeDuration = 3f)
    {
        AudioSource activeSource = _isSource1Active ? _ambienceSource1 : _ambienceSource2;
        if (activeSource.clip == newAmbienceClip) return;

        if (_temporaryAmbienceRoutine != null)
        {
            StopCoroutine(_temporaryAmbienceRoutine);
            _temporaryAmbienceRoutine = null;
        }

        if (_crossfadeRoutine != null) StopCoroutine(_crossfadeRoutine);

        _crossfadeRoutine = StartCoroutine(CrossfadeRoutine(newAmbienceClip, fadeDuration));
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

    /// <summary>
    /// Cambia a un ambiente estridente/temporal (Susto/Inicio de persecución) 
    /// y luego de un tiempo transiciona automáticamente a un ambiente de calma.
    /// </summary>
    public void PlayTemporaryAmbience(AudioClip temporaryClip, AudioClip fallbackClip, float temporaryDuration, float introFade = 0.5f, float outroFade = 4f)
    {
        // Cancelamos cualquier transición previa para que no haya solapamiento
        if (_temporaryAmbienceRoutine != null) StopCoroutine(_temporaryAmbienceRoutine);
        if (_crossfadeRoutine != null) StopCoroutine(_crossfadeRoutine);

        _temporaryAmbienceRoutine = StartCoroutine(TemporaryAmbienceRoutine(temporaryClip, fallbackClip, temporaryDuration, introFade, outroFade));
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

    private IEnumerator CrossfadeRoutine(AudioClip newClip, float duration)
    {
        AudioSource fadingOutSource = _isSource1Active ? _ambienceSource1 : _ambienceSource2;
        AudioSource fadingInSource = _isSource1Active ? _ambienceSource2 : _ambienceSource1;

        fadingInSource.clip = newClip;
        fadingInSource.volume = 0f;
        fadingInSource.Play();

        float time = 0f;
        float startFadeOutVol = fadingOutSource.volume;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            fadingOutSource.volume = Mathf.Lerp(startFadeOutVol, 0f, t);
            fadingInSource.volume = Mathf.Lerp(0f, _maxAmbienceVolume, t);

            yield return null;
        }

        fadingOutSource.volume = 0f;
        fadingOutSource.Stop();
        fadingInSource.volume = _maxAmbienceVolume;

        _isSource1Active = !_isSource1Active;
        _crossfadeRoutine = null;
    }

    private IEnumerator TemporaryAmbienceRoutine(AudioClip tempClip, AudioClip fallbackClip, float duration, float intro, float outro)
    {
        // 1. Fundido rápido hacia el sonido estridente (ej. 0.5 segundos para dar impacto)
        yield return StartCoroutine(CrossfadeRoutine(tempClip, intro));

        // 2. Mantenemos el ambiente de tensión el tiempo solicitado
        yield return new WaitForSeconds(duration);

        // 3. Volvemos suavemente a la calma
        yield return StartCoroutine(CrossfadeRoutine(fallbackClip, outro));

        _temporaryAmbienceRoutine = null;
    }
}