using UnityEngine;
using TMPro;

public class TutorialUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI _tutorialText;
    [SerializeField] private CanvasGroup _canvasGroup;

    [Header("Settings")]
    [SerializeField] private float _fadeSpeed = 3f;

    private float _targetAlpha = 0f;

    private bool _isHiddenBySystem = false;

    private void Awake()
    {
        _canvasGroup.alpha = 0f;
    }

    private void OnEnable()
    {
        TutorialManager.OnShowTutorialUI += HandleShow;
        TutorialManager.OnHideTutorialUI += HandleHide;

        UIManager.OnShowNoteUI += HideTemporarily;
        UIManager.OnHideNoteUI += RestoreVisibility;
    }

    private void OnDisable()
    {
        TutorialManager.OnShowTutorialUI -= HandleShow;
        TutorialManager.OnHideTutorialUI -= HandleHide;

        UIManager.OnShowNoteUI -= HideTemporarily;
        UIManager.OnHideNoteUI -= RestoreVisibility;
    }

    private void HideTemporarily(string noteContent) => _isHiddenBySystem = true;
    private void RestoreVisibility() => _isHiddenBySystem = false;

    private void HandleShow(string text)
    {
        _tutorialText.text = text;
        _targetAlpha = 1f;
    }

    private void HandleHide()
    {
        _targetAlpha = 0f;
    }

    private void Update()
    {
        float finalTargetAlpha = _isHiddenBySystem ? 0f : _targetAlpha;
        _canvasGroup.alpha = Mathf.MoveTowards(_canvasGroup.alpha, finalTargetAlpha, Time.deltaTime * _fadeSpeed);
    }
}