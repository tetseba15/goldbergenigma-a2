using System.Collections.Generic;
using UnityEngine;

public class ObjectiveReporter : MonoBehaviour
{
    [Header("Objetivo a agregar")]
    [SerializeField] private bool _addObjective;
    [SerializeField] private List<string> _objectiveMessages;
    [SerializeField] private ObjectiveManager.ObjectiveId _objectiveId;

    private bool ableToUpdate = true;

    private int objectiveIndex = 0;

    [Header("Objetivo a eliminar")]
    [SerializeField] private bool _deleteObjective;
    [SerializeField] private ObjectiveManager.ObjectiveId _objectiveIdRemove;

    private bool objectiveDeleted;

    public void ReportObjective()
    {
        if (_addObjective && ableToUpdate && objectiveIndex < _objectiveMessages.Count)
        {
            ObjectiveManager.Instance.UpdateObjective(_objectiveId, _objectiveMessages[objectiveIndex]);
            objectiveIndex++;

            ableToUpdate = false;
        }
            
        if (_deleteObjective && !objectiveDeleted)
        {
            ObjectiveManager.Instance.DeleteObjective(_objectiveIdRemove);
            objectiveDeleted = true;
        }
    }

    public void AllowUpdate()
    {
        ableToUpdate = true;
    }
}
