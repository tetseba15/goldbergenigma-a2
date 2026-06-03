using UnityEngine;
using UnityEngine.Events; 

public class Note : MonoBehaviour, IInteractable
{
    [Header("Note Settings")]
    [SerializeField] private string _promptText = "[E] Leer nota";
    [SerializeField, TextArea(3, 10)] private string _noteContent;
    [SerializeField] private AudioClip _readNoteClip;

    [Header("Eventos Especiales")]
    [Tooltip("¿El evento debe ocurrir solo la primera vez que se lee la nota?")]
    [SerializeField] private bool _triggerEventOnlyOnce = true;

    [Tooltip("Arrastra aquí lo que quieres que pase al leer la nota.")]
    public UnityEvent OnNoteRead;

    private bool _hasBeenRead = false;

    public string GetInteractPrompt(GameObject interactor) => _promptText;

    public void Interact(GameObject interactor)
    {
        if (!UIManager.Instance.IsReadingNote)
        {
            AudioManager.Instance.PlaySFX(_readNoteClip, 0.5f); 
            UIManager.Instance.ShowNote(_noteContent);
            UIManager.Instance.HideInteractPrompt();

            if (!_hasBeenRead || !_triggerEventOnlyOnce)
            {
                OnNoteRead?.Invoke(); 
                _hasBeenRead = true;
            }
        }
    }
}