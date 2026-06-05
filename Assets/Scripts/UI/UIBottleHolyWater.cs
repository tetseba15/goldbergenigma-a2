using UnityEngine;
using UnityEngine.UI;
public class SliderBottleHoly : MonoBehaviour
{
    public Image BottleHolyWaterSlider;

    private void Start()
    {
        // 1. Forzamos visualmente a la imagen a estar en 0 al arrancar el nivel.
        
        if (BottleHolyWaterSlider != null)
        {
            BottleHolyWaterSlider.fillAmount = 0f;
        }
    }
    private void OnEnable()
    {
        GameEvent.OnHolyWater += UpdateHolyWater;

    }
    private void OnDisable()
    {
        GameEvent.OnHolyWater -= UpdateHolyWater;
    }
    private void UpdateHolyWater(float current, float max)
    {
        BottleHolyWaterSlider.fillAmount = (current / max);

    }
}
