using UnityEngine;
using UnityEngine.UI;
public class SliderBottleHoly : MonoBehaviour
{
    public Slider BottleHolyWaterSlider;

    private void OnEnable()
    {
        GameEvent.HolyWater += UpdateHolyWater;

    }
    private void OnDestroy()
    {
        GameEvent.HolyWater -= UpdateHolyWater;
    }
    private void UpdateHolyWater(float current, float max)
    {
        BottleHolyWaterSlider.value = (current / max);

    }
}
