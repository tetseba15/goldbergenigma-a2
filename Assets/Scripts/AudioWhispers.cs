using UnityEngine;

public class AudioWhispers : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip audioWhispers;

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {
            
            if (!audioSource.isPlaying)
            {
                audioSource.clip = audioWhispers;
                audioSource.Play();
            }
        }
    }
}
