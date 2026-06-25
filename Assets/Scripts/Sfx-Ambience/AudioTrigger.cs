using UnityEngine;

public class AudioTrigger : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioClip _soundToPlay;

    [Header("Configuration")]
    [Tooltip("Volumen del sonido")]
    [SerializeField] private float _volume = 1f;

    [Tooltip("Distancia máxima a la que se escucha el sonido")]
    [SerializeField] private float _maxDistance = 20f;

    [Tooltip("Si está marcado, el sonido solo sonará la primera vez que entre al trigger.")]
    [SerializeField] private bool _playOnlyOnce = true;

    [Tooltip("Tag del objeto que activa el sonido (ej: Player)")]
    [SerializeField] private string _targetTag = "Player";

    private bool _hasPlayed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (_playOnlyOnce && _hasPlayed) return;
           
        if (other.CompareTag(_targetTag))
        {
            if (_soundToPlay != null)
            {
                AudioManager.Instance.PlaySFXAtPosition(_soundToPlay, transform.position, _volume, Random.Range(0.90f, 1.10f), _maxDistance);
                _hasPlayed = true;
            }
        }
    }
}
