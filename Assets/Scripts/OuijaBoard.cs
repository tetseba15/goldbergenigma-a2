using System;
using UnityEngine;

public class OuijaBoard : MonoBehaviour, IInteractable
{
    [SerializeField] private GhostAppearance _ghostAppearance;

    [Header("Item Type")]
    [SerializeField] private PlayerInventory.ItemType _itemType;

    [Header("Mensajes")]
    [SerializeField] private string _act1Message = "Habitación. Arriba.";
    [SerializeField] private string _act2Message = "Chimenea.";
    [SerializeField] private string _act3Message = "Fogón. Fuego. Enfrentamiento.";

    [Header("Mensajes de recordatorio")]
    [SerializeField] private string _act1Reminder = "La niña dijo que vaya a la habitación de arriba.";
    [SerializeField] private string _act2Reminder = "La niña dijo que vaya a la chimenea.";
    [SerializeField] private string _act3Reminder = "La niña dijo que vaya al fogón.";

    [Header("Referencias de escena")]
    [SerializeField] private GameObject _fireplaceLookAtDialogue;

    private int _currentAct = 1;
    private bool _isOnCooldown = false;
    private int _useCount = 0;

    public int CurrentAct => _currentAct;
    public bool HasUsedAct2Ouija { get; private set; } = false;

    public static event Action<PlayerInventory.ItemType> OnInteract;

    public string GetInteractPrompt(GameObject interactor)
    {
        return _isOnCooldown ? string.Empty : "Presiona E para usar la ouija";
    }

    public void Interact(GameObject interactor)
    {
        if (_isOnCooldown) return;
        _useCount++;

        if (_currentAct == 2)
            HasUsedAct2Ouija = true;

        if (_useCount == 1 && _ghostAppearance != null)
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

        if (DialogueManager.Instance != null)
            DialogueManager.Instance.ShowDialogue(GetCurrentMessage());

        OnInteract?.Invoke(_itemType);
    }

    public void ResetCooldown()
    {
        _isOnCooldown = false;
    }

    public void AdvanceToNextAct()
    {
        if (_currentAct < 3)
        {
            _currentAct++;
            _useCount = 0;

            if (_currentAct == 2 && _fireplaceLookAtDialogue != null)
                _fireplaceLookAtDialogue.SetActive(true);
        }
    }

    private string GetCurrentMessage()
    {
        if (_useCount > 1)
        {
            switch (_currentAct)
            {
                case 2: return _act2Reminder;
                case 3: return _act3Reminder;
                default: return _act1Reminder;
            }
        }

        switch (_currentAct)
        {
            case 2: return _act2Message;
            case 3: return _act3Message;
            default: return _act1Message;
        }
    }
}