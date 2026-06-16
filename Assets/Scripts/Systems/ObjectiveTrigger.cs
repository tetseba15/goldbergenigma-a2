using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ObjectiveTrigger : MonoBehaviour
{
    [SerializeField, Tooltip("El nuevo objetivo que se le dará al jugador")]
    private string _newObjective;

    [SerializeField, Tooltip("Id del objetivo")]
    private ObjectiveManager.ObjectiveId _objectiveId;

    [SerializeField, Tooltip("¿Se destruye después de actualizar el objetivo?")]
    private bool _triggerOnce = true;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OBJECTIVE TRIGGER");

        if (other.CompareTag("Player"))
        {
            ObjectiveManager.Instance.UpdateObjective(_objectiveId, _newObjective);

            if (_triggerOnce)
            {
                Destroy(gameObject);
            }
        }
    }
}