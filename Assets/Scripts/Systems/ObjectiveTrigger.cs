using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ObjectiveTrigger : MonoBehaviour
{
    [SerializeField, Tooltip("El nuevo objetivo que se le dará al jugador")]
    private string _newObjective;

    [SerializeField, Tooltip("¿Se destruye después de actualizar el objetivo?")]
    private bool _triggerOnce = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ObjectiveManager.Instance.UpdateObjective(_newObjective);

            if (_triggerOnce)
            {
                Destroy(gameObject);
            }
        }
    }
}