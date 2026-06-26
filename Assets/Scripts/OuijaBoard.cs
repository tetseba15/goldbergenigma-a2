using System;
using System.Collections.Generic;
using UnityEngine;

public class OuijaBoard : MonoBehaviour, IInteractable
{
    [SerializeField] private GhostAppearance _ghostAppearance;

    [Header("Item Type")]
    [SerializeField] private PlayerInventory.ItemType _itemType;

    [Header("Mensajes")]
    [SerializeField] private List<string> _actMessages;

    [Header("Mensajes de recordatorio")]
    [SerializeField] private List<string> _remindMessages;

    [Header("Referencias de escena")]
    [SerializeField] private GameObject _fireplaceLookAtDialogue;

    // -1 porque en la introducción no se usa la Ouija
    private int _messageIndex = -1;
    private int _remindMessageIndex = -1;

    private bool _isOnCooldown = false;

    private ObjectiveReporter objectiveReporter;
    public bool HasUsedAct2Ouija { get; private set; } = false; // REVISAR
    public static OuijaBoard Instance { get; private set; }

    public static event Action<PlayerInventory.ItemType> OnOuijaUse;

    private void OnEnable()
    {
        GameFlowManager.OnActChanged += IncrementMessageIndex;
        ItemPickup.OnInteract += IncrementMessageIndex;
    }

    private void OnDisable()
    {
        GameFlowManager.OnActChanged -= IncrementMessageIndex;
        ItemPickup.OnInteract -= IncrementMessageIndex;
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        objectiveReporter = GetComponent<ObjectiveReporter>();
    }

    public string GetInteractPrompt(GameObject interactor)
    {
        return "Presiona [E] para usar la ouija";
    }

    public void Interact(GameObject interactor)
    {
        if (!_isOnCooldown)
        {
            //if (_currentAct == 2)
            //    HasUsedAct2Ouija = true;

            if (_ghostAppearance != null)
            {
                _isOnCooldown = true;

                Vector3 spawnPos = interactor.transform.position + interactor.transform.forward * 3f;
                spawnPos.y = interactor.transform.position.y;

                if (UnityEngine.AI.NavMesh.SamplePosition(spawnPos, out UnityEngine.AI.NavMeshHit hit, 3f, UnityEngine.AI.NavMesh.AllAreas))
                {
                    spawnPos = hit.position;
                }

                _ghostAppearance.Appear(spawnPos);
            }

            if (DialogueManager.Instance != null && _messageIndex < _actMessages.Count)
                DialogueManager.Instance.ShowDialogue(_actMessages[_messageIndex]);

            if (objectiveReporter != null) objectiveReporter.ReportObjective();
            OnOuijaUse?.Invoke(_itemType);
        }
        else
        {
            if (DialogueManager.Instance != null && _messageIndex < _remindMessages.Count)
            {
                DialogueManager.Instance.ShowDialogue(_remindMessages[_remindMessageIndex]);
            }
        }
    }

    public void ResetCooldown()
    {
        _isOnCooldown = false;
    }

    private void IncrementMessageIndex(GameFlowManager.Act currentAct)
    {
        _messageIndex++;
        _remindMessageIndex++;

        _isOnCooldown = false;

        Debug.Log("Msj index" + _messageIndex);
        Debug.Log("Remind msj" + _remindMessageIndex);
    }

    private void IncrementMessageIndex(PlayerInventory playerInventory, PlayerInventory.ItemType itemType)
    {
        if (itemType == PlayerInventory.ItemType.WorkshopKey || itemType == PlayerInventory.ItemType.QuinchoKey || itemType == PlayerInventory.ItemType.PatioKey)
        {
            _messageIndex++;
            _remindMessageIndex++;

            _isOnCooldown = false;

            Debug.Log("Msj index" + _messageIndex);
            Debug.Log("Remind msj" + _remindMessageIndex);
        }
    }
}