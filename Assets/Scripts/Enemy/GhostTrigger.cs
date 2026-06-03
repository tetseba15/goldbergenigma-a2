using UnityEngine;

public class GhostTrigger : MonoBehaviour
{
    [SerializeField] private GhostAppearance _ghostAppearance;
    private bool _triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (_triggered) return;
        if (other.CompareTag("Player"))
        {
            _triggered = true;
            _ghostAppearance.Appear();
        }
    }
}