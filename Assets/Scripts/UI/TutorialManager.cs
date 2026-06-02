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
    }

    // If multiple tutorials triggers, add them to the Queue
    private Queue<TutorialStep> _tutorialQueue = new Queue<TutorialStep>();
    private TutorialStep _currentStep;
    private bool _isFadingOut = false;

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

    private void Update()
    {
        if (_currentStep == null)
        {
            if (_tutorialQueue.Count > 0)
            {
                _currentStep = _tutorialQueue.Dequeue();
                _tutorialText.text = _currentStep.Text;
                _isFadingOut = false;
            }
            else
            {
                Fade(0f); 
                return;
            }
        }

        // Fade
        if (!_isFadingOut)
        {
            Fade(1f); 

            if (_currentStep.CompletionCondition != null && _currentStep.CompletionCondition.Invoke())
            {
                _isFadingOut = true;
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