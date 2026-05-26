using UnityEngine;
using UnityEngine.UI;
public class SliderBottleHoly : MonoBehaviour
{
    public Image BottleHolyWaterSlider;

    private void OnEnable()
    {
        GameEvent.OnHolyWater += UpdateHolyWater;

    }
    private void OnDestroy()
    {
        GameEvent.OnHolyWater -= UpdateHolyWater;
    }
    private void UpdateHolyWater(float current, float max)
    {
        BottleHolyWaterSlider.fillAmount = (current / max);

    }
}
