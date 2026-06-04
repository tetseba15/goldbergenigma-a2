using UnityEngine;
using TMPro; 

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Elements")]
    [SerializeField] private GameObject _notePanel;
    [SerializeField] private TextMeshProUGUI _noteText;
    [SerializeField] private TextMeshProUGUI _interactPromptText;

    private int _frameNoteOpened = -1;

    // Others can know if player is reading something
    public bool IsReadingNote { get; private set; }

    private PlayerInputHandler _inputHandler;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
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

        HideInteractPrompt();
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
        if (IsReadingNote)
        {
            HideNote();
        }
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
        _noteText.text = content;
        _notePanel.SetActive(true);
        IsReadingNote = true;

        _frameNoteOpened = Time.frameCount;

        string stackTrace = StackTraceUtility.ExtractStackTrace();
        Debug.Log("La función fue llamada desde: \n" + stackTrace);
    }

    public void HideNote()
    {
        _notePanel.SetActive(false);
        IsReadingNote = false;

        string stackTrace = StackTraceUtility.ExtractStackTrace();
        Debug.Log("La función fue llamada desde: \n" + stackTrace);

    }

    public void ShowInteractPrompt(string message)
    {
        _interactPromptText.text = message;
        _interactPromptText.gameObject.SetActive(true);
    }
    public void HideInteractPrompt()
    {
        _interactPromptText.gameObject.SetActive(false);
    }
}