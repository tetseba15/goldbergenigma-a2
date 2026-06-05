using UnityEngine;

public class WorkshopExitTrigger : MonoBehaviour
{
    [SerializeField, TextArea(2, 5)] private string _exitDialogue;
    [SerializeField] private Bonfire _bonfire;

    private int _triggerCount = 0;
    private bool _triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (_triggered) return;
        if (!other.CompareTag("Player")) return;

        _triggerCount++;
        if (_triggerCount >= 2)
        {
            _triggered = true;
            DialogueManager.Instance.ShowDialogue(_exitDialogue);
            if (_bonfire != null)
                _bonfire.Unlock();
        }
    }
}