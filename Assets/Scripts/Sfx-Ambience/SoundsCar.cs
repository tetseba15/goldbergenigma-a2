using UnityEngine;

public class SoundsCar : MonoBehaviour
{
   

    [Header("Fuentes de Audio")]
    [SerializeField] private AudioSource audioSourceRadio;

    [Header("Fuentes de audio")]

    [SerializeField] private AudioClip radioClip;
    


    private bool onRadio = false;
    
    void Start()
    {
        // Configuración inicial del archivo de audio
        if (audioSourceRadio != null && radioClip != null)
        {
            audioSourceRadio.clip = radioClip;

        }
    }
    private void OnTriggerEnter(Collider other)
    {
       
        if (other.CompareTag("Player") && !onRadio)
        {
            EncenderRadioAutomatica();
        }
    }

    private void EncenderRadioAutomatica()
    {
        onRadio = true;
        audioSourceRadio.Play();
        

        
    }
   
}

