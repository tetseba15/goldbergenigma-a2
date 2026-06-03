using UnityEngine;

public class SoundChildrensStairs : MonoBehaviour
{
    [Header("Configuración de Audio")]
    public AudioSource AudioSource;
     

    private bool played = false; 

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player") && !played)
        {
            AudioSource.Play();      
            played = true;   
        }
    }
}
