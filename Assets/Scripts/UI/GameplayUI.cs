using UnityEngine;
using TMPro;

public class GameplayUI : MonoBehaviour
{
    [Header("Notes UI Elements")]
    [SerializeField] private GameObject _notePanel;
    [SerializeField] private TextMeshProUGUI _noteText;

    [Header("Interact Prompt Elements")]
    [SerializeField] private TextMeshProUGUI _interactPromptText;

    private void OnEnable()
    {
        UIManager.OnShowNoteUI += HandleShowNote;
        UIManager.OnHideNoteUI += HandleHideNote;
        UIManager.OnShowInteractPromptUI += HandleShowInteractPrompt;
        UIManager.OnHideInteractPromptUI += HandleHideInteractPrompt;
    }

    private void OnDisable()
    {
        UIManager.OnShowNoteUI -= HandleShowNote;
        UIManager.OnHideNoteUI -= HandleHideNote;
        UIManager.OnShowInteractPromptUI -= HandleShowInteractPrompt;
        UIManager.OnHideInteractPromptUI -= HandleHideInteractPrompt;
    }

    private void Start()
    {
        HandleHideNote();
        HandleHideInteractPrompt();
    }

    private void HandleShowNote(string content)
    {
        _noteText.text = content;
        _notePanel.SetActive(true);
    }

    private void HandleHideNote()
    {
        _notePanel.SetActive(false);
    }

    private void HandleShowInteractPrompt(string message)
    {
        _interactPromptText.text = message;
        _interactPromptText.gameObject.SetActive(true);
    }

    private void HandleHideInteractPrompt()
    {
        _interactPromptText.gameObject.SetActive(false);
    }
}