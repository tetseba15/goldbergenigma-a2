using System;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    //IN
    public static Action<string, Func<bool>> RequestConditionTutorial;
    public static Action<string, float> RequestTimedTutorial;

    public static Action RequestClearTutorials;

    //OUT
    public static event Action<string> OnShowTutorialUI;
    public static event Action OnHideTutorialUI;


    private class TutorialStep
    {
        public string Text;
        public Func<bool> CompletionCondition;
        public float Timer; 
    }

    private Queue<TutorialStep> _tutorialQueue = new Queue<TutorialStep>();
    private TutorialStep _currentStep;

    private float _transitionDelay = 1f;
    private float _transitionTimer = 0f;

    private void OnEnable()
    {
        RequestConditionTutorial += EnqueueConditionTutorial;
        RequestTimedTutorial += EnqueueTimedTutorial;
        RequestClearTutorials += ClearAllTutorials;
    }

    private void OnDisable()
    {
        RequestConditionTutorial -= EnqueueConditionTutorial;
        RequestTimedTutorial -= EnqueueTimedTutorial;
        RequestClearTutorials -= ClearAllTutorials;
    }

    private void EnqueueConditionTutorial(string message, Func<bool> conditionToComplete)
    {
        _tutorialQueue.Enqueue(new TutorialStep { Text = message, CompletionCondition = conditionToComplete, Timer = -1f });
    }

    private void EnqueueTimedTutorial(string message, float duration)
    {
        _tutorialQueue.Enqueue(new TutorialStep { Text = message, CompletionCondition = null, Timer = duration });
    }

    private void Update()
    {
        if (_transitionTimer > 0)
        {
            _transitionTimer -= Time.deltaTime;
            return;
        }

        if (_currentStep == null)
        {
            if (_tutorialQueue.Count > 0)
            {
                _currentStep = _tutorialQueue.Dequeue();
                OnShowTutorialUI?.Invoke(_currentStep.Text);
            }
            return;
        }

        if (_currentStep.CompletionCondition != null)
        {
            if (_currentStep.CompletionCondition.Invoke())
            {
                FinishCurrentTutorial();
            }
        }
        else if (_currentStep.Timer > 0)
        {
            _currentStep.Timer -= Time.deltaTime;
            if (_currentStep.Timer <= 0)
            {
                FinishCurrentTutorial();
            }
        }
    }

    private void FinishCurrentTutorial()
    {
        OnHideTutorialUI?.Invoke();
        _currentStep = null;
        _transitionTimer = _transitionDelay;
    }

    private void ClearAllTutorials()
    {
        _tutorialQueue.Clear();
        if (_currentStep != null)
        {
            FinishCurrentTutorial();
        }
    }
}