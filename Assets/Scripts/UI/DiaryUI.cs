using UnityEngine;
using TMPro;

public class DiaryUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject _diaryPanel;
    [SerializeField] private TextMeshProUGUI _objectiveText;
    [SerializeField] private TextMeshProUGUI _keysText;
    [SerializeField] private TextMeshProUGUI _batteryText;

    private void Awake()
    {
        _diaryPanel.SetActive(false);
    }

    private void OnEnable()
    {
        DiaryManager.OnDiaryStateChanged += HandleDiaryState;
        DiaryManager.OnDiaryDataUpdated += HandleDiaryData;
    }

    private void OnDisable()
    {
        DiaryManager.OnDiaryStateChanged -= HandleDiaryState;
        DiaryManager.OnDiaryDataUpdated -= HandleDiaryData;
    }

    private void HandleDiaryState(bool isOpen)
    {
        _diaryPanel.SetActive(isOpen);
    }

    private void HandleDiaryData(string objective, string keys, string battery)
    {
        _objectiveText.text = objective;
        _keysText.text = keys;
        _batteryText.text = battery;
    }
}