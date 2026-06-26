using UnityEngine;

public class AudioWhispers : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip audioWhispers;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        
        if (!triggered && other.CompareTag("Player"))
        {
            triggered = true;

            if (!audioSource.isPlaying)
            {
                audioSource.clip = audioWhispers;
                audioSource.Play();
            }
        }
    }
}
