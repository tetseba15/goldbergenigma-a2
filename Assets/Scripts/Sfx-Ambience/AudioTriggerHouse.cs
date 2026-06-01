using UnityEngine;

public class AudioTriggerHouse : MonoBehaviour
{
    [Header("Controlador del Mixer")]
    // Arrastra aquí tu objeto "GestorAudioNuevo" (el que tiene el Audio Mixer)
    [SerializeField] private ControllerAudioExt1 controladorAudio;

    private void OnTriggerEnter(Collider other)
    {
        // Si el jugador cruza la puerta hacia ADENTRO de la casa
        if (other.CompareTag("Player"))
        {
            if (controladorAudio != null)
            {
                // Llama a la función que cambia al snapshot de la casa (-80dB para la música)
                controladorAudio.EntrarALaCasa();
            }
        }
    }

  
}
