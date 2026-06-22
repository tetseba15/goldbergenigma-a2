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

    private class ObjectiveData
    {
        public string Message;
        public bool IsCompleted;
    }

    public static ObjectiveManager Instance { get; private set; }

    //Events
    public static event Action<string> OnObjectiveUpdated;

    public static event Action<ObjectiveId> OnObjectiveCompleted;


    private Dictionary<ObjectiveId, ObjectiveData> _activeObjectives = new Dictionary<ObjectiveId, ObjectiveData>();
    private bool _hasSeenDiaryTutorial = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        AddObjective(ObjectiveId.MerrinCar, "Explorar el auto del padre Merrin");
    }

    public void AddObjective(ObjectiveId id, string message)
    {
        if (!_activeObjectives.ContainsKey(id))
        {
            _activeObjectives.Add(id, new ObjectiveData { Message = message, IsCompleted = false });
            RefreshObjectiveText();
            ShowTutorialIfNeeded();
        }
    }

    // On objective completed, the objective will be look crossed out
    public void CompleteObjective(ObjectiveId id)
    {

        if (_activeObjectives.ContainsKey(id) && !_activeObjectives[id].IsCompleted)
        {
            _activeObjectives[id].IsCompleted = true;

            OnObjectiveCompleted?.Invoke(id);

            RefreshObjectiveText();
            ShowTutorialIfNeeded();
        }
    }

    public void RemoveObjective(ObjectiveId id)
    {
        if (_activeObjectives.ContainsKey(id))
        {
            _activeObjectives.Remove(id);
            RefreshObjectiveText();
        }
    }

    public void ClearAllObjectives()
    {
        _activeObjectives.Clear();
        RefreshObjectiveText();
    }

    public void UpdateObjective(ObjectiveId id, string message)
    {
        message = message.Replace("\\n", "\n");

        if (!_activeObjectives.ContainsKey(id))
        {
            _activeObjectives.Add(id, new ObjectiveData { Message = message, IsCompleted = false });
        }
        else
        {
            _activeObjectives[id].Message = message;
            _activeObjectives[id].IsCompleted = false; 
        }

        RefreshObjectiveText();
        ShowTutorialIfNeeded();
    }

    private void RefreshObjectiveText()
    {
        string formattedText = "";

        foreach (var kvp in _activeObjectives)
        {
            if (kvp.Value.IsCompleted)
            {
                // Using <color> with a hexadecimal code to make it gray and <s> to cross it out
                formattedText += $"<color=#888888><s>- {kvp.Value.Message}</s></color>\n\n";
            }
            else
            {
                formattedText += $"- {kvp.Value.Message}\n\n";
            }
        }

        if (string.IsNullOrEmpty(formattedText))
        {
            formattedText = "Nada por ahora...";
        }

        OnObjectiveUpdated?.Invoke(formattedText);
    }

    private void ShowTutorialIfNeeded()
    {
        if (!_hasSeenDiaryTutorial)
        {
            TutorialManager.RequestConditionTutorial?.Invoke(
                "Se actualizó un objetivo.\nPresiona [Tab] para revisar la libreta",
                () => DiaryManager.Instance != null && DiaryManager.Instance.IsOpen());

            _hasSeenDiaryTutorial = true;
        }
    }
}