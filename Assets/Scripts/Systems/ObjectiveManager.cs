using UnityEngine;
using System;
using System.Collections.Generic;

public class ObjectiveManager : MonoBehaviour
{
    public enum ObjectiveId
    {
        MerrinCar,
        MansionExploration,
        HolyWater,
        UpstairsExploration,
        Chimney,
        Graveyard,
        Barbecue,
        CommunicateWithOuija,
        Workshop,
        FinalChallenge
    }

    public static ObjectiveManager Instance { get; private set; }

    [Header("Current State")]
    private string currentObjective;

    private Dictionary<ObjectiveId, string> objectives = new Dictionary<ObjectiveId, string>();

    public static event Action<string> OnObjectiveChanged;

    private bool _hasSeenDiaryTutorial = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        UpdateObjective(ObjectiveId.MerrinCar, "Explorar el auto del padre Merrin");
    }

    public void UpdateObjective(ObjectiveId objectiveId, string newObjectiveMessage)
    {
        newObjectiveMessage = newObjectiveMessage.Replace("\\n", "\n");

        if (!objectives.ContainsKey(objectiveId)) objectives.Add(objectiveId, newObjectiveMessage);
        else objectives[objectiveId] = newObjectiveMessage;

        UpdateObjectiveMessage();
        OnObjectiveChanged?.Invoke(currentObjective);

        if (!_hasSeenDiaryTutorial)
        {
            TutorialManager.Instance.ShowTutorial("Se actualizó un objetivo.\nPresiona [Tab] para revisar la libreta",
                () => DiaryManager.Instance.IsOpen());

            _hasSeenDiaryTutorial = true;
        }

        Debug.Log("Objective Added: " + objectiveId);
    }

    public void DeleteObjective(ObjectiveId objectiveId)
    {
        if (objectives.ContainsKey(objectiveId))
        {
            objectives.Remove(objectiveId);
            UpdateObjectiveMessage();

            OnObjectiveChanged?.Invoke(currentObjective);

            Debug.Log("Objective deleted: " + objectiveId);
        }
    }

    private void UpdateObjectiveMessage()
    {
        currentObjective = "";

        foreach (string objectiveMessage in objectives.Values)
        {
            currentObjective += objectiveMessage + " \n";
        }
    }

    //public void UpdateObjective(string newObjective)
    //{

    //    newObjective = newObjective.Replace("\\n", "\n");

    //    if (_currentObjective == newObjective) return;

    //    _currentObjective = newObjective;
    //    OnObjectiveChanged?.Invoke(_currentObjective);

    //    if (!_hasSeenDiaryTutorial)
    //    {
    //        TutorialManager.Instance.ShowTutorial("Se actualizó un objetivo.\nPresiona [Tab] para revisar la libreta",
    //            () => DiaryManager.Instance.IsOpen());

    //        _hasSeenDiaryTutorial = true;
    //    }
    //}

    public string GetCurrentObjective()
    {
        return currentObjective;
    }
}