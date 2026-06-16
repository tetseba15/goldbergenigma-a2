using UnityEngine;

public class ObjectiveReporter : MonoBehaviour
{
    [Header("Objetivo a agregar")]
    [SerializeField] private bool _addObjective;
    [SerializeField] private string _objectiveMessage;
    [SerializeField] private ObjectiveManager.ObjectiveId _objectiveId;

    [Header("Objetivo a eliminar")]
    [SerializeField] private bool _deleteObjective;
    [SerializeField] private ObjectiveManager.ObjectiveId _objectiveIdRemove;

    private bool objectiveChanged;

    public void ReportObjective()
    {
        if (objectiveChanged) return;

        if (_addObjective) ObjectiveManager.Instance.UpdateObjective(_objectiveId, _objectiveMessage);
        if (_deleteObjective) ObjectiveManager.Instance.DeleteObjective(_objectiveIdRemove);

        objectiveChanged = true;
    }
}
