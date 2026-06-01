using System.Collections;
using UnityEngine;
using BitWave_Labs.AnimatedTextReveal;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [SerializeField] private AnimatedTextReveal _textReveal;
    [SerializeField] private float _displayDuration = 4f;

    private Coroutine _currentDialogue;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ShowDialogue(string text)
    {
        if (_currentDialogue != null)
            StopCoroutine(_currentDialogue);

        _currentDialogue = StartCoroutine(PlayDialogue(text));
    }

    private IEnumerator PlayDialogue(string text)
    {
        _textReveal.TextMesh.text = text;
        _textReveal.SetAllCharactersAlpha(0);
        yield return StartCoroutine(_textReveal.FadeText(true));
        yield return new WaitForSeconds(_displayDuration);
        yield return StartCoroutine(_textReveal.FadeText(false));
    }
}