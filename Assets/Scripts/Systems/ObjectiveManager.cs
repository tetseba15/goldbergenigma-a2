using UnityEngine;
using System;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance { get; private set; }

    [Header("Current State")]
    [SerializeField] private string _currentObjective = "Explorar la mansión.";

    public static event Action<string> OnObjectiveChanged;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        UpdateObjective(_currentObjective);
    }

    public void UpdateObjective(string newObjective)
    {
        if (_currentObjective == newObjective) return; 

        _currentObjective = newObjective;

        OnObjectiveChanged?.Invoke(_currentObjective);
    }

    public string GetCurrentObjective()
    {
        return _currentObjective;
    }
}