using UnityEngine;

public class OuijaBoard : MonoBehaviour, IInteractable
{
    [SerializeField] private GhostAppearance _ghostAppearance;

    [Header("Mensajes")]
    [SerializeField] private string _act1Message = "Habitación. Arriba.";
    [SerializeField] private string _act2Message = "Chimenea.";
    [SerializeField] private string _act3Message = "Fogón. Fuego. Enfrentamiento.";

    private int _currentAct = 1;

    public string GetInteractPrompt(GameObject interactor)
    {
        return "Presiona E para usar la ouija";
    }

    public void Interact(GameObject interactor)
    {
        if (_ghostAppearance != null)
        {
            Vector3 spawnPos = interactor.transform.position + interactor.transform.forward * 2f;
            spawnPos.y = interactor.transform.position.y;
            _ghostAppearance.Appear(spawnPos);
        }

        if (DialogueManager.Instance != null)
            DialogueManager.Instance.ShowDialogue(GetCurrentMessage());
    }

    public void AdvanceToNextAct()
    {
        if (_currentAct < 3)
            _currentAct++;
    }

    private string GetCurrentMessage()
    {
        switch (_currentAct)
        {
            case 2: return _act2Message;
            case 3: return _act3Message;
            default: return _act1Message;
        }
    }
}