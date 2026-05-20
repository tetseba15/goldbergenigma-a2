using UnityEngine;
using UnityEngine.UI;

public class UILinterna : MonoBehaviour
{
    public Slider BatterySlider;

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
        BatterySlider.value = (current / max);

    }
}
