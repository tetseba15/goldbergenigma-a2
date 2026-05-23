using UnityEngine;
using UnityEngine.UI;

public class UITorch : MonoBehaviour
{
    public Slider BatterySlider;
    public AudioSource BatteryEnd;
    private bool yaSono = false;
    private void OnEnable()
    {
        GameEvent.OnBattery += UpdateBattery;

    }
    private void OnDestroy()
    {
        GameEvent.OnBattery -= UpdateBattery;
    }
    private void UpdateBattery(float current, float max)
    {
        float porcentaje = current / max;
        BatterySlider.value = porcentaje;

        if (porcentaje <= 0f)
        {
            // Solo reproduce si no ha sonado antes
            if (!yaSono && BatteryEnd != null)
            {
                BatteryEnd.Play();
                yaSono = true; // Marcamos que ya se reprodujo
            }
        }
        else
        {
            // Si la batería vuelve a cargarse o sube de 0, reiniciamos el estado
            yaSono = false;
        }

    }

}
