using UnityEngine;

public class SpawnZone : MonoBehaviour
{
    private Collider _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    public bool Contains(Vector3 point)
    {
        return _collider.bounds.Contains(point);
    }
}