using UnityEngine;
using UnityEngine.Audio;

public class ControladorAmbienteCuadros : MonoBehaviour
{
    [Header("Fuentes de Audio (Audio Sources)")]
    [SerializeField] private AudioSource ambienteCuadros;
    [SerializeField] private AudioSource cuadroNena;
    [SerializeField] private AudioSource StairsLaught;

    [Header("Archivos de Sonido (Audio Clips)")]
    [SerializeField] private AudioClip clipAmbienteGeneral;
    [SerializeField] private AudioClip clipSonidoNena;
    [SerializeField] private AudioClip cliplaughtChilden;
    [Header("Configuración del Audio Mixer")]
    // Arrastra aquí tu archivo principal de Audio Mixer desde la carpeta Project
    [SerializeField] private AudioMixer miAudioMixer;
    [SerializeField] private float tiempoTransicion = 0.5f; // Segundos que tarda en cambiar el volumen

    private void Start()
    {
        // Forzamos a que el juego empiece en el estado normal ("Snapshot")
        ActivarSnapshotNormal();

        if (ambienteCuadros != null && clipAmbienteGeneral != null)
        {
            ambienteCuadros.clip = clipAmbienteGeneral;
            ambienteCuadros.loop = true;
            ambienteCuadros.Play();
        }
    }

    // FUNCIÓN PARA EL CUADRO NENA: Activa el sonido y cambia al snapshot SFX
    public void ActivarSonidoCuadroNena()
    {
        if (cuadroNena != null && clipSonidoNena != null)
        {
            if (!cuadroNena.isPlaying)
            {
                // Cambia el mezclador al modo "SFX" usando su nombre exacto
                CambiarSnapshotPorNombre("SFX");

                cuadroNena.clip = clipSonidoNena;
                cuadroNena.loop = true;
                cuadroNena.Play();
            }
        }
    }

    // FUNCIÓN PARA APAGAR EL CUADRO: Detiene el sonido y vuelve al snapshot normal
    public void DesactivarSonidoCuadroNena()
    {
        if (cuadroNena != null && cuadroNena.isPlaying)
        {
            ActivarSnapshotNormal();
            cuadroNena.Stop();
        }
    }

    // FUNCIÓN PARA LAS ESCALERAS: Reproduce la risa y cambia al snapshot SFX
    public void ActivarRisaEscaleras()
    {
        if (StairsLaught != null && cliplaughtChilden != null)
        {
            // Cambia el mezclador al modo "SFX"
            CambiarSnapshotPorNombre("SFX");

            StairsLaught.PlayOneShot(cliplaughtChilden);
        }
    }

    // FUNCIÓN AUXILIAR: Para regresar el Audio Mixer a la normalidad
    public void ActivarSnapshotNormal()
    {
        CambiarSnapshotPorNombre("Snapshot");
    }

    // MÁQUINA DE CAMBIO: Busca el Snapshot por texto y hace la transición suave
    private void CambiarSnapshotPorNombre(string nombreSnapshot)
    {
        if (miAudioMixer != null)
        {
            AudioMixerSnapshot snapshotEncontrado = miAudioMixer.FindSnapshot(nombreSnapshot);

            if (snapshotEncontrado != null)
            {
                snapshotEncontrado.TransitionTo(tiempoTransicion);
                Debug.Log("Audio Mixer cambió con éxito al estado: " + nombreSnapshot);
            }
            else
            {
                Debug.LogError("Error: No se encontró ningún Snapshot llamado '" + nombreSnapshot + "' en el Audio Mixer.");
            }
        }
        else
        {
            Debug.LogWarning("Atención: No has asignado el Audio Mixer en el Inspector de este script.");
        }
    }
}