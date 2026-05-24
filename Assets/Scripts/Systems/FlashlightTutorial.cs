using UnityEngine;
using TMPro;

public class FlashlightTutorial : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField, Tooltip("The TextMeshPro UI element for the subtitles")]
    private TextMeshProUGUI _tutorialText;
    [SerializeField, Tooltip("CanvasGroup attached to the text to control fading")]
    private CanvasGroup _canvasGroup;

    [Header("Dependencies")]
    [SerializeField] private PlayerInputHandler _inputHandler;
    [SerializeField] private PlayerFlashlight _flashlight;

    [Header("Settings")]
    [SerializeField] private float _fadeSpeed = 3f;

    private bool _isActive = false;
    private bool _isCompleted = false;

    private bool _hasToggledF = false;
    private bool _hasInspectedR = false;

    private void Start()
    {
        _canvasGroup.alpha = 0f;
    }

    private void Update()
    {
        if (_isCompleted)
        {
            HandleFade();
            return;
        }

        if (!_isActive) return;

        HandleFade();
        CheckPlayerProgression();
    }

    public void TriggerTutorial()
    {
        if (_isCompleted) return;

        _isActive = true;
        UpdateTutorialText();
    }

    private void CheckPlayerProgression()
    {
        if (!_hasToggledF && _flashlight.IsOn())
        {
            _hasToggledF = true;
            UpdateTutorialText();
        }

        if (!_hasInspectedR && _inputHandler.IsInspectingFlashlight)
        {
            _hasInspectedR = true;
            UpdateTutorialText();
        }

        if (_hasToggledF && _hasInspectedR)
        {
            CompleteTutorial();
        }
    }

    private void UpdateTutorialText()
    {
        if (_hasToggledF && !_hasInspectedR)
        {
            _tutorialText.text = "Hold [R] to check battery";
        }
        else if (!_hasToggledF && _hasInspectedR)
        {
            _tutorialText.text = "Press [F] to toggle flashlight";
        }
        else
        {
            _tutorialText.text = "Press [F] to toggle flashlight\nHold [R] to check battery";
        }
    }

    private void CompleteTutorial()
    {
        _isActive = false;
        _isCompleted = true;
    }

    private void HandleFade()
    {
        float targetAlpha = (_isActive && !_isCompleted) ? 1f : 0f;
        _canvasGroup.alpha = Mathf.MoveTowards(_canvasGroup.alpha, targetAlpha, Time.deltaTime * _fadeSpeed);

        if (_isCompleted && _canvasGroup.alpha <= 0f)
        {
            _tutorialText.gameObject.SetActive(false);
            enabled = false;
        }
    }
}