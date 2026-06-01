using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private float _updateInterval = 3f; 
    [SerializeField] private int _maxNearestZones = 3;

    private SpawnZone[] _allZones;
    private List<SpawnZone> _nearestZones = new List<SpawnZone>();
    private float _updateTimer = 0f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        _allZones = FindObjectsByType<SpawnZone>(FindObjectsSortMode.None);
    }

    private void Update()
    {
        if (PlayerTarget.Instance == null) return;

        _updateTimer += Time.deltaTime;
        if (_updateTimer >= _updateInterval)
        {
            _updateTimer = 0f;
            UpdateNearestZones();
        }
    }

    private void UpdateNearestZones()
    {
        Vector3 playerPos = PlayerTarget.Instance.PlayerTransform.position;

        _nearestZones = _allZones
            .OrderBy(z => Vector3.Distance(z.transform.position, playerPos))
            .Take(_maxNearestZones)
            .ToList();
    }

    public SpawnZone GetRandomNearestZone()
    {
        if (_nearestZones.Count == 0) return null;
        return _nearestZones[Random.Range(0, _nearestZones.Count)];
    }
}