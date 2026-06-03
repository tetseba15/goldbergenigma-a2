using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class ControllerAudioExt1 : MonoBehaviour
{
    [Header("Referencia al Mixer y Snapshots")]
    public AudioMixer mainMixer;
    public AudioMixerSnapshot snapshotNormal;
    public AudioMixerSnapshot snapshotHouse;

    

    [Header("Sliders de la Interfaz (Opcionales)")]
    public Slider sliderMusica;
    public Slider sliderSFX;
    public Slider sliderDialogue;

    

    void Start()
    {
        // Configuramos automáticamente los Sliders si los dejás asignados
        ConfigurarSlider(sliderMusica, "VolMusica");
        ConfigurarSlider(sliderSFX, "VolSFX");
        ConfigurarSlider(sliderDialogue, "VolDialogue");
    }

    

    
   

    // --- LÓGICA PARA LOS SLIDERS DE VOLUMEN ---

    void ConfigurarSlider(Slider slider, string nombreParametro)
    {
        if (slider == null) return;

        slider.minValue = 0.0001f;
        slider.maxValue = 1f;

        if (mainMixer.GetFloat(nombreParametro, out float valorActualDb))
        {
            slider.value = Mathf.Pow(10, valorActualDb / 20);
        }

        slider.onValueChanged.AddListener((valor) => {
            float dB = Mathf.Log10(valor) * 20;
            mainMixer.SetFloat(nombreParametro, dB);
        });
    }
    public void EntrarALaCasa()
    {
        
        if (snapshotHouse != null)
        {
            snapshotHouse.TransitionTo(2.5f);
           
        }
    }

    public void SalirDeLaCasa()
    {
        
        if (snapshotNormal != null)
        {
            snapshotNormal.TransitionTo(2.5f);
            
        }
    }
}
