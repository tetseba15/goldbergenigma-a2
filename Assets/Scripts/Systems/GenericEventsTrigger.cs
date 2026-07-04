using System;
using UnityEngine;

public class GenericEventsTrigger : MonoBehaviour
{
    public enum GameEventType
    {
        GhostAppearance = 1,
        BloodBath = 2,
        GhostlyWhispering = 3,
        Radio = 4,
        CloseEntryDoor = 5,
        Piano = 6,
        GhostLook = 7,
        DiningRoomVisions = 8,
    }

    [Header("Trigger Type")]
    [SerializeField] private GameEventType _triggerType;

    private bool _triggered = false;
    public static Action<GameEventType> OnTriggerEvent;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_triggered) return;

            OnTriggerEvent?.Invoke(_triggerType);
            _triggered = true;
        }
        
    }
}
