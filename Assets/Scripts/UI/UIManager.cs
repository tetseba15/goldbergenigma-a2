using UnityEngine;
using TMPro; 

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Elements")]
    [SerializeField] private GameObject _notePanel;
    [SerializeField] private TextMeshProUGUI _noteText;
    [SerializeField] private TextMeshProUGUI _interactPromptText;

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
        }

        HideInteractPrompt();
    }

    private void Update()
    {
        if (IsReadingNote && _inputHandler != null && _inputHandler.CancelInput)
        {
            HideNote();
        }
    }

    public void ShowNote(string content)
    {
        _noteText.text = content;
        _notePanel.SetActive(true);
        IsReadingNote = true;
    }

    public void HideNote()
    {
        _notePanel.SetActive(false);
        IsReadingNote = false;
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