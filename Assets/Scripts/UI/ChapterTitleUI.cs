using System;
using System.Collections;
using UnityEngine;
using TMPro;
using BitWave_Labs.AnimatedTextReveal;

public class ChapterTitleUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private AnimatedTextReveal _titleReveal;
    [SerializeField] private AnimatedTextReveal _subtitleReveal;

    [Header("Cinematic Timing")]
    [SerializeField, Tooltip("Tiempo que el texto se queda en pantalla completo")]
    private float _displayDuration = 4f;
    [SerializeField, Tooltip("Pausa entre la aparición del título y el subtítulo")]
    private float _delayBetweenTexts = 1f;

    private Coroutine _currentRoutine;

    private void Awake()
    {
        if (_titleReveal != null) _titleReveal.TextMesh.text = "";
        if (_subtitleReveal != null) _subtitleReveal.TextMesh.text = "";
    }

    private void OnEnable()
    {
        GameFlowManager.OnChapterTitleRequested += PlayChapterTitle;
    }

    private void OnDisable()
    {
        GameFlowManager.OnChapterTitleRequested -= PlayChapterTitle;
    }

    private void PlayChapterTitle(string mainTitle, string subTitle, Func<bool> waitCondition)
    {
        if (_currentRoutine != null) StopCoroutine(_currentRoutine);
        _currentRoutine = StartCoroutine(TitleSequenceRoutine(mainTitle, subTitle, waitCondition));
    }

    private IEnumerator TitleSequenceRoutine(string mainTitle, string subTitle, Func<bool> waitCondition)
    {
        if (waitCondition != null)
        {
            yield return new WaitUntil(waitCondition);
        }

        _titleReveal.TextMesh.text = mainTitle;
        _titleReveal.TextMesh.ForceMeshUpdate(); 
        _titleReveal.SetAllCharactersAlpha(0);   

        bool hasSubtitle = _subtitleReveal != null && !string.IsNullOrEmpty(subTitle);
        if (hasSubtitle)
        {
            _subtitleReveal.TextMesh.text = subTitle;
            _subtitleReveal.TextMesh.ForceMeshUpdate(); 
            _subtitleReveal.SetAllCharactersAlpha(0);
        }

        yield return StartCoroutine(_titleReveal.FadeText(true));

        if (hasSubtitle)
        {
            yield return new WaitForSeconds(_delayBetweenTexts);
            yield return StartCoroutine(_subtitleReveal.FadeText(true));
        }

        yield return new WaitForSeconds(_displayDuration);

        Coroutine fadeOutTitle = StartCoroutine(_titleReveal.FadeText(false));
        Coroutine fadeOutSub = null;

        if (hasSubtitle)
        {
            fadeOutSub = StartCoroutine(_subtitleReveal.FadeText(false));
        }

        yield return fadeOutTitle;
        if (fadeOutSub != null) yield return fadeOutSub;
    }
}