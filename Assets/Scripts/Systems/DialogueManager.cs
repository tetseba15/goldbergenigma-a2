using System.Collections;
using UnityEngine;
using BitWave_Labs.AnimatedTextReveal;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [SerializeField] private AnimatedTextReveal _textReveal;
    [SerializeField] private float _defaultDisplayDuration = 4f;

    private Coroutine _currentDialogue;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ShowDialogue(string text, AudioClip voiceClip = null)
    {
        if (_currentDialogue != null)
            StopCoroutine(_currentDialogue);

        _currentDialogue = StartCoroutine(PlayDialogue(text, voiceClip));
    }

    public void ShowDialogueWithDelay(string text, float delay, AudioClip voiceClip = null)
    {
        StartCoroutine(PlayDialogueWithDelayRoutine(text, delay, voiceClip));
    }

    private IEnumerator PlayDialogueWithDelayRoutine(string text, float delay, AudioClip voiceClip)
    {
        yield return new WaitForSeconds(delay);
        ShowDialogue(text, voiceClip);
    }

    private IEnumerator PlayDialogue(string text, AudioClip voiceClip)
    {
        if (AudioManager.Instance != null) AudioManager.Instance.SetDialogueState(true);

        if (voiceClip != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(voiceClip);
        }

        _textReveal.TextMesh.text = text;
        _textReveal.SetAllCharactersAlpha(0);
        yield return StartCoroutine(_textReveal.FadeText(true));

        // Subtitles duration tied to audio clip duration
        float finalDuration = (voiceClip != null) ? voiceClip.length : _defaultDisplayDuration;
        yield return new WaitForSeconds(finalDuration);

        // Desvanecer el texto
        yield return StartCoroutine(_textReveal.FadeText(false));

        if (AudioManager.Instance != null) AudioManager.Instance.SetDialogueState(false);
    }

    //private IEnumerator PlayDialogue(string text, float delay)
    //{
    //    yield return new WaitForSeconds(delay);
    //    ShowDialogue(text);
    //}

    //private IEnumerator PlayDialogue(string text)
    //{
    //    if (AudioManager.Instance != null) AudioManager.Instance.SetDialogueState(true);

    //    _textReveal.TextMesh.text = text;
    //    _textReveal.SetAllCharactersAlpha(0);
    //    yield return StartCoroutine(_textReveal.FadeText(true));
    //    yield return new WaitForSeconds(_defaultDisplayDuration);
    //    yield return StartCoroutine(_textReveal.FadeText(false));

    //    if (AudioManager.Instance != null) AudioManager.Instance.SetDialogueState(false);
    //}
}