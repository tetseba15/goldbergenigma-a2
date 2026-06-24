using System;
using UnityEngine;

public class GenericEventsTrigger : MonoBehaviour
{
    public enum GameEventType
    {
        GhostAppearance,
        BloodBath,
        GhostlyWhispering
    }

    [Header("Trigger Type")]
    [SerializeField] private GameEventType _triggerType;

    private bool _triggered = false;
    public static Action<GameEventType> OnTriggerEvent;

    private void OnTriggerEnter(Collider other)
    {
        if (_triggered) return;

        OnTriggerEvent?.Invoke(_triggerType);
        _triggered = true;
    }
}
