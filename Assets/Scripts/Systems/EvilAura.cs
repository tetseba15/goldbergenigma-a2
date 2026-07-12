using UnityEngine;

public class EvilAura : MonoBehaviour, IPurificable
{
    [SerializeField] private PurificableData _purificableData;
    public PurificableData PurificationData => _purificableData;

    private ObjectiveReporter _objectiveReporter;

    private void Awake()
    {
        _objectiveReporter = GetComponent<ObjectiveReporter>();
    }

    private void OnEnable()
    {
        CrossController.OnCrossUse += Purify;
    }

    private void OnDisable()
    {
        CrossController.OnCrossUse -= Purify;
    }

    public void Purify(PlayerInventory.ItemType itemType, Vector3 purifySourcePosition)
    {
        float distance = Vector3.Distance(transform.position, purifySourcePosition);

        if (itemType == _purificableData._purificationItem && distance <= _purificableData._purificationDistance)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, 5f, LayerMask.GetMask("Interactable"));
            InteractableDoor interactableDoor = null;

            foreach (Collider collider in colliders)
            {
                InteractableDoor door = collider.GetComponent<InteractableDoor>();
                if (door != null)
                {
                    interactableDoor = door;
                    break;
                }
            }

            if (interactableDoor != null)
            {
                interactableDoor.Unlock();
                Destroy(gameObject);
            }

            if (_objectiveReporter != null)
            {
                _objectiveReporter.ReportObjective();
            }
        }
    }
}
