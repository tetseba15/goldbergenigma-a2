using UnityEngine;
using UnityEngine.UI;

public class FlashlightUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerFlashlight _flashlightLogic;
    [SerializeField] private Image _batteryLedBar;

    [Header("Colors")]
    [SerializeField] private Color _highColor = Color.green;
    [SerializeField] private Color _medColor = Color.yellow;
    [SerializeField] private Color _lowColor = Color.red;

    private void OnEnable()
    {
        if (_flashlightLogic != null)
            _flashlightLogic.OnBatteryChanged += UpdateLED;
    }

    private void OnDisable()
    {
        if (_flashlightLogic != null)
            _flashlightLogic.OnBatteryChanged -= UpdateLED;
    }

    private void UpdateLED(float batteryPercentage)
    {
        _batteryLedBar.fillAmount = batteryPercentage;

        if (batteryPercentage > 0.60f)
            _batteryLedBar.color = _highColor;
        else if (batteryPercentage > 0.30f)
            _batteryLedBar.color = _medColor;
        else
            _batteryLedBar.color = _lowColor;
    }
}
