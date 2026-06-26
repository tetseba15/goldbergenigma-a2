using UnityEngine;

public class EntryDoorEventManager : MonoBehaviour
{
    [Header("Puerta a Controlar")]
    [SerializeField] private InteractableDoor _entryDoor;

    private void OnEnable()
    {
        GenericEventsTrigger.OnTriggerEvent += HandleGameEvent;
    }

    private void OnDisable()
    {
        GenericEventsTrigger.OnTriggerEvent -= HandleGameEvent;
    }

    private void HandleGameEvent(GenericEventsTrigger.GameEventType eventType)
    {
        if (eventType == GenericEventsTrigger.GameEventType.CloseEntryDoor)
        {
            if (_entryDoor != null)
            {
                _entryDoor.ForceSlamShutAndLock();
            }
        }
    }
}