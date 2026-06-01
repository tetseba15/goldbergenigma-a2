using UnityEngine;

public class SoundAmbience1 : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip ambientAudio;

    void Start()
    {
        // Configuramos el audio para que repita en bucle y sea envolvente (2D)
        audioSource.clip = ambientAudio;
        audioSource.loop = true;
        audioSource.spatialBlend = 0f;
    }

    // Se activa cuando el jugador entra al espacio exterior
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop(); // Apaga el sonido exterior de golpe
            }
        }
    }
}
