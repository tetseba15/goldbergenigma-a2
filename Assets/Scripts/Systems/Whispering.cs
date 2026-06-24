using UnityEngine;

public class Whispering : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _clip;

    [Header("Stun duration")]
    [SerializeField] private float _stunDuration = 5f;

    [Header("Game event type")]
    [SerializeField] private GenericEventsTrigger.GameEventType _gameEventType;

    private float time;

    private void Awake()
    {
        if( _audioSource != null && _clip != null)
        {
            _audioSource.clip = _clip;
        }
    }

    private void OnEnable()
    {
        GenericEventsTrigger.OnTriggerEvent += StartWhispers;
    }

    private void OnDisable()
    {
        GenericEventsTrigger.OnTriggerEvent -= StartWhispers;
    }

    private void Update()
    {
        if (_audioSource.isPlaying)
        {
            time += Time.deltaTime;
        }

        if (time >= _stunDuration)
        {
            _audioSource.Stop();
        }
    }

    private void StartWhispers(GenericEventsTrigger.GameEventType gameEventType)
    {
        if (gameEventType == _gameEventType)
        {
            if (_audioSource != null)
            {
                _audioSource.Play();
            }

            EnemyAI.TriggerRoar(_stunDuration, _stunDuration);
        }
    }
}
