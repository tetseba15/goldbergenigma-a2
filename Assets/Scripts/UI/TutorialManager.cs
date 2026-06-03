using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI _tutorialText;
    [SerializeField] private CanvasGroup _canvasGroup;

    [Header("Settings")]
    [SerializeField] private float _fadeSpeed = 3f;

    private class TutorialStep
    {
        public string Text;
        public Func<bool> CompletionCondition;
        public float Timer;
    }

    // If multiple tutorials triggers, add them to the Queue
    private Queue<TutorialStep> _tutorialQueue = new Queue<TutorialStep>();
    private TutorialStep _currentStep;
    private bool _isFadingOut = false;
    private float _currentTimer;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        _canvasGroup.alpha = 0f;
    }

    /// <summary>
    /// Añade un tutorial a la cola. 
    /// Ejemplo de uso: TutorialManager.Instance.ShowTutorial("Presiona F", () => _linterna.EstaPrendida);
    /// </summary>
    public void ShowTutorial(string message, Func<bool> conditionToComplete)
    {
        _tutorialQueue.Enqueue(new TutorialStep { Text = message, CompletionCondition = conditionToComplete });
    }

    public void ShowTutorial(string message, float durationInSeconds)
    {
        _tutorialQueue.Enqueue(new TutorialStep { Text = message, CompletionCondition = null, Timer = durationInSeconds });
    }

    private void Update()
    {
        if (_currentStep == null)
        {
            if (_tutorialQueue.Count > 0)
            {
                _currentStep = _tutorialQueue.Dequeue();
                _tutorialText.text = _currentStep.Text;
                _currentTimer = _currentStep.Timer; 
                _isFadingOut = false;
            }
            else
            {
                Fade(0f);
                return;
            }
        }

        // Fade
        bool isReading = UIManager.Instance != null && UIManager.Instance.IsReadingNote;

        if (!_isFadingOut)
        {
            Fade(isReading ? 0f : 1f);

            if (!isReading)
            {
                if (_currentStep.CompletionCondition != null)
                {
                    if (_currentStep.CompletionCondition.Invoke())
                    {
                        _isFadingOut = true;
                    }
                }
                else
                {
                    _currentTimer -= Time.deltaTime;
                    if (_currentTimer <= 0f)
                    {
                        _isFadingOut = true;
                    }
                }
            }
        }
        else 
        {
            Fade(0f);
            if (_canvasGroup.alpha <= 0.01f)
            {
                _currentStep = null;
            }
        }
    }

    private void Fade(float targetAlpha)
    {
        _canvasGroup.alpha = Mathf.MoveTowards(_canvasGroup.alpha, targetAlpha, Time.deltaTime * _fadeSpeed);
    }
}