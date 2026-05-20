using UnityEngine;
using UnityEngine.UI;

public class UITorch : MonoBehaviour
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
