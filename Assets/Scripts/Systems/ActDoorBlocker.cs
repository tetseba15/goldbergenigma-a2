using UnityEngine;

[RequireComponent(typeof(InteractableDoor))]
public class ActDoorBlocker : MonoBehaviour
{
    [Header("Configuración del Candado")]
    [SerializeField, Tooltip("¿En qué Acto se debe desbloquear esta puerta?")]
    private GameFlowManager.Act _unlocksAtAct = GameFlowManager.Act.Act1_GroundFloor;

    [Header("Feedback Narrativo (Opcional)")]
    [SerializeField, TextArea]
    private string _narrativeLockDialogue = "Quizás debería terminar de revisar el coche de Merrin antes de entrar.";

    private InteractableDoor _door;

    private void Awake()
    {
        _door = GetComponent<InteractableDoor>();
    }

    private void OnEnable()
    {
        GameFlowManager.OnActChanged += HandleActChange;

        _door.OnNarrativeLockHit += TriggerDialogue;
    }

    private void OnDisable()
    {
        GameFlowManager.OnActChanged -= HandleActChange;
        _door.OnNarrativeLockHit -= TriggerDialogue;
    }

    private void Start()
    {
        if (GameFlowManager.Instance != null && GameFlowManager.Instance.CurrentAct < _unlocksAtAct)
        {
            _door.ApplyNarrativeLock();
        }
    }

    private void TriggerDialogue()
    {
        if (DialogueManager.Instance != null && !string.IsNullOrEmpty(_narrativeLockDialogue))
        {
            DialogueManager.Instance.ShowDialogue(_narrativeLockDialogue);
        }
    }

    private void HandleActChange(GameFlowManager.Act newAct)
    {
        if (newAct == _unlocksAtAct)
        {
            _door.RemoveNarrativeLock();

            enabled = false; 
        }
    }
}