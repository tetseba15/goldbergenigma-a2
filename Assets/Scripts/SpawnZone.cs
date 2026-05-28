using UnityEngine;

public class SpawnZone : MonoBehaviour
{
    [SerializeField] private int _waypointCount = 4;
    private Collider _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    public bool Contains(Vector3 point)
    {
        return _collider.bounds.Contains(point);
    }

    public Vector3 GetRandomPointInside()
    {
        Bounds bounds = _collider.bounds;
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            bounds.min.y,
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }

    public Vector3[] GetPatrolWaypoints()
    {
        Vector3[] waypoints = new Vector3[_waypointCount];
        for (int i = 0; i < _waypointCount; i++)
        {
            waypoints[i] = GetRandomPointInside();
        }
        return waypoints;
    }
}