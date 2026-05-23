using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Note : MonoBehaviour, IInteractable
{
    [Header("Interact prompt")]
    [SerializeField] private string _promptText = "[E] Recoger objeto";
    [Header("Note Content")]
    [SerializeField, TextArea(3, 10)]
    private string _noteContent;
    [Header("Mannequin")]
    [SerializeField] private GameObject _mannequin;
    [Header("End Demo")]
    [SerializeField] private bool _isEndNote = false;
    private bool _mannequinSpawned = false;
    private bool _waitingToRestart = false;

    [Header("Final Note Activator")]
    [SerializeField] private GameObject _finalNote;
    [SerializeField] private bool _isKitchenNote = false;

    public string GetInteractPrompt(GameObject interactor)
    {
        return _promptText;
    }

    public void Interact(GameObject interactor)
    {
        if (!UIManager.Instance.IsReadingNote)
        {
            UIManager.Instance.ShowNote(_noteContent);
            UIManager.Instance.HideInteractPrompt();

            if (_mannequin != null && !_mannequinSpawned)
            {
                _mannequin.SetActive(true);
                _mannequinSpawned = true;
            }
            if (_isEndNote)
            {
                _waitingToRestart = true;
            }

            if (_isKitchenNote && _finalNote != null)
            {
                _finalNote.SetActive(true);
            }
        }
    }

    private void Update()
    {
        if (_waitingToRestart && !UIManager.Instance.IsReadingNote)
        {
            _waitingToRestart = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}