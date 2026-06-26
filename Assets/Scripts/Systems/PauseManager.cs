using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [Header("Referencias Principales")]
    [SerializeField] private GameObject _pauseCanvas;
    [SerializeField] private PlayerInputHandler _inputHandler;

    [Header("Paneles de Menú")]
    [SerializeField, Tooltip("El panel base que contiene Continuar, Opciones, Salir")]
    private GameObject _mainPausePanel;

    private bool _isPaused = false;

    private Stack<GameObject> _menuStack = new Stack<GameObject>();

    private void OnEnable()
    {
        if (_inputHandler != null)
        {
            _inputHandler.OnPauseTriggered += PauseGame;

            _inputHandler.OnCancelTriggered += HandleCancelInput;
        }
    }

    private void OnDisable()
    {
        if (_inputHandler != null)
        {
            _inputHandler.OnPauseTriggered -= PauseGame;
            _inputHandler.OnCancelTriggered -= HandleCancelInput;
        }
    }

    private void Start()
    {
        _pauseCanvas.SetActive(false);
    }

    public void PauseGame()
    {
        if (_isPaused) return;

        _isPaused = true;
        _pauseCanvas.SetActive(true);
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        _inputHandler.EnableUIControls();

        _menuStack.Clear();

        OpenPanel(_mainPausePanel);
    }

    public void ResumeGame()
    {
        if (!_isPaused) return;

        _isPaused = false;
        _pauseCanvas.SetActive(false);
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _inputHandler.EnableGameplayControls();
    }


    /// <summary>
    /// Llama a esta función desde el evento OnClick() de tus botones de la UI
    /// </summary>
    public void OpenPanel(GameObject newPanel)
    {
        if (_menuStack.Count > 0)
        {
            _menuStack.Peek().SetActive(false);
        }

        _menuStack.Push(newPanel);
        newPanel.SetActive(true);
    }

    public void ExitMenu()
    {
        _isPaused = false;
        Time.timeScale = 1f;
        GameManager.Instance.LoadScene(0);
    }

    /// <summary>
    /// Llama a esta función desde el botón "Atrás" de la UI, o mediante la tecla Escape
    /// </summary>
    public void HandleCancelInput()
    {
        if (!_isPaused) return;

        if (_menuStack.Count > 1)
        {
            GameObject topPanel = _menuStack.Pop();
            topPanel.SetActive(false);

            _menuStack.Peek().SetActive(true);
        }
        else
        {
            ResumeGame();
        }
    }
}