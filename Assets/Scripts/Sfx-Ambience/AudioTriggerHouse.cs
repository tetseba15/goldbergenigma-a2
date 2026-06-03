using UnityEngine;

public class AudioTriggerHouse : MonoBehaviour
{
    [Header("Controlador del Mixer")]
    
    [SerializeField] private ControllerAudioExt1 controladorAudio;

    private void OnTriggerEnter(Collider other)
    {
        // Cuando cruce el jugador el trigger
        if (other.CompareTag("Player"))
        {
            if (controladorAudio != null)
            {
                //llama la función
                controladorAudio.EntrarALaCasa();
            }
        }
    }

  
}
