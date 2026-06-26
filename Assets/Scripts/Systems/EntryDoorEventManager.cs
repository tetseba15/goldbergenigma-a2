using UnityEngine;

public class EntryDoorEventManager : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private InteractableDoor _entryDoor;
    [SerializeField] private LightningController _lightningController; // <--- Nueva referencia

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

            if (_lightningController != null)
            {
                _lightningController.StopStorm();
            }
        }
    }
}