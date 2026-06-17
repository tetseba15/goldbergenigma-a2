using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public static event Action<string> OnShowNoteUI;
    public static event Action OnHideNoteUI;
    public static event Action<string> OnShowInteractPromptUI;
    public static event Action OnHideInteractPromptUI;

    public bool IsReadingNote { get; private set; }

    private int _frameNoteOpened = -1;
    private PlayerInputHandler _inputHandler;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            _inputHandler = player.GetComponent<PlayerInputHandler>();

            _inputHandler.OnCancelTriggered += HandleCancelAction;
            _inputHandler.OnInteractTriggered += HandleInteractAction;
        }
    }

    private void OnDestroy()
    {
        if (_inputHandler != null)
        {
            _inputHandler.OnCancelTriggered -= HandleCancelAction;
            _inputHandler.OnInteractTriggered -= HandleInteractAction;
        }
    }

    private void HandleCancelAction()
    {
        if (IsReadingNote) HideNote();
    }

    private void HandleInteractAction()
    {
        if (IsReadingNote && Time.frameCount > _frameNoteOpened)
        {
            HideNote();

            if (_inputHandler != null)
            {
                _inputHandler.ConsumeInteractInput();
            }
        }
    }


    public void ShowNote(string content)
    {
        IsReadingNote = true;
        _frameNoteOpened = Time.frameCount;

        OnShowNoteUI?.Invoke(content);
    }

    public void HideNote()
    {
        IsReadingNote = false;

        OnHideNoteUI?.Invoke();
    }

    public void ShowInteractPrompt(string message)
    {
        OnShowInteractPromptUI?.Invoke(message);
    }

    public void HideInteractPrompt()
    {
        OnHideInteractPromptUI?.Invoke();
    }
}