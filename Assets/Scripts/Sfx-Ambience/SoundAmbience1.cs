using UnityEngine;

public class SoundAmbience1 : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip ambientAudio;

    void Start()
    {
        
        audioSource.clip = ambientAudio;
        audioSource.loop = true;
        audioSource.spatialBlend = 0f;

        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    
}
