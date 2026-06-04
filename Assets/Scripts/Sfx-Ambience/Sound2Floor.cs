using UnityEngine;

public class Sound2Floor : MonoBehaviour
{
   
    
        [Header("Componentes")]
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _soundToPlay;

        [Header("Configuración")]
        [Tooltip("Si está marcado, el sonido solo sonará la primera vez que entres.")]
        [SerializeField] private bool _playOnlyOnce = true;

        [Tooltip("Escribe el Tag del objeto que activa el sonido (ej: Player)")]
        [SerializeField] private string _targetTag = "Player";

        private bool _hasPlayed = false;

        private void OnTriggerEnter(Collider other)
        {
            
            if (_playOnlyOnce && _hasPlayed) return;

           
            if (other.CompareTag(_targetTag))
            {
                
                if (_audioSource != null && _soundToPlay != null)
                {
                    _audioSource.PlayOneShot(_soundToPlay);
                    _hasPlayed = true; 

                    
                }
            }
        }
    
}
