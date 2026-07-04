using UnityEngine;

public class TriggerRelay : MonoBehaviour
{
    [SerializeField, Tooltip("Objeto que contiene script PhysicalDoor")]
    private PhysicalDoor _parentDoor;

    private void OnTriggerEnter(Collider other)
    {
        if (_parentDoor != null)
        {
            _parentDoor.TryBustOpen(other);
        }
    }
}