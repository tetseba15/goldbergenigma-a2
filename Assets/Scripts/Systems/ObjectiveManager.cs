using UnityEngine;
using System;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance { get; private set; }

    [Header("Current State")]
    [SerializeField] private string _currentObjective = "Explorar el auto del padre Merrin";

    public static event Action<string> OnObjectiveChanged;

    private bool _hasSeenDiaryTutorial = false;

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

        newObjective = newObjective.Replace("\\n", "\n");

        if (_currentObjective == newObjective) return;

        _currentObjective = newObjective;
        OnObjectiveChanged?.Invoke(_currentObjective);

        if (!_hasSeenDiaryTutorial)
        {
            TutorialManager.Instance.ShowTutorial("Se actualizó un objetivo.\nPresiona [Tab] para revisar la libreta",
                () => DiaryManager.Instance.IsOpen());

            _hasSeenDiaryTutorial = true;
        }
    }

    public string GetCurrentObjective()
    {
        return _currentObjective;
    }
}