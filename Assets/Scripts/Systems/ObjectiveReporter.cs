using UnityEngine;

public class ObjectiveReporter : MonoBehaviour
{
    [SerializeField] private string _objectiveMessage;
    [SerializeField] private ObjectiveManager.ObjectiveId _objectiveId;

    [Header("Objetivo a eliminar")]
    [SerializeField] private bool deleteObjective;
    [SerializeField] private ObjectiveManager.ObjectiveId _objectiveIdRemove;

    private bool objetiveChanged;

    public void ReportObjective()
    {
        if (objetiveChanged) return;

        ObjectiveManager.Instance.UpdateObjective(_objectiveId, _objectiveMessage);
        if (deleteObjective)
        {
            ObjectiveManager.Instance.DeleteObjective(_objectiveIdRemove);
        }

        objetiveChanged = true;
    }
}
