using System.Collections.Generic;
using UnityEngine;

public class ObjectiveReporter : MonoBehaviour
{
    [Header("Objetivo a Agregar (Pendiente)")]
    [SerializeField] private bool _addObjective;

    [TextArea(2, 5)]
    [SerializeField] private List<string> _objectiveMessages;

    [SerializeField] private ObjectiveManager.ObjectiveId _objectiveId;

    [Header("Objetivo a Completar (Tachado)")]
    [SerializeField] private bool _completeObjective;
    [SerializeField] private ObjectiveManager.ObjectiveId _objectiveIdToComplete;

    [Header("Objetivo a Eliminar (Borrado total)")]
    [SerializeField] private bool _deleteObjective;
    [SerializeField] private ObjectiveManager.ObjectiveId _objectiveIdToRemove;

    private bool _ableToUpdate = true;
    private int _objectiveIndex = 0;
    private bool _objectiveCompleted = false;
    private bool _objectiveDeleted = false;

    public void ReportObjective()
    {
        if (_addObjective && _ableToUpdate && _objectiveIndex < _objectiveMessages.Count)
        {
            ObjectiveManager.Instance.UpdateObjective(_objectiveId, _objectiveMessages[_objectiveIndex]);
            _objectiveIndex++;
            _ableToUpdate = false;
        }

        if (_completeObjective && !_objectiveCompleted)
        {
            ObjectiveManager.Instance.CompleteObjective(_objectiveIdToComplete);
            _objectiveCompleted = true;
        }

        if (_deleteObjective && !_objectiveDeleted)
        {
            ObjectiveManager.Instance.RemoveObjective(_objectiveIdToRemove);
            _objectiveDeleted = true;
        }
    }

    public void AllowUpdate()
    {
        _ableToUpdate = true;
    }
}