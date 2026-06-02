using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Global Audio Sources (2D)")]
    [SerializeField] private AudioSource _musicAudioSource;
    [SerializeField] private AudioSource _ambienceAudioSource;
    [SerializeField] private AudioSource _sfxAudioSource;

    [Header("3D Audio Pool Settings")]
    [SerializeField, Tooltip("Cantidad de reproductores simultáneos permitidos")]
    private int _poolSize = 15;
    [SerializeField, Tooltip("Prefab vacío con un AudioSource en modo 3D")]
    private GameObject _audioSourcePrefab;

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
}