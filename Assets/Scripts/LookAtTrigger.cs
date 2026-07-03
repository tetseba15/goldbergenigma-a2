using UnityEngine;

public class LookAtTrigger : MonoBehaviour
{
    [Header("Opciones de detección")]
    [SerializeField] private float _detectionDistance = 15f;
    [SerializeField] private float _detectionAngle = 15f;
    [SerializeField]
    [Tooltip("Necesario si el objeto implementa la interfaz ILookTriggereable")] private bool _triggerAction;

    [Header("Raycast position offset")]
    [SerializeField] private Vector3 _raycastOffset;

    [Header("Dialogos")]
    [SerializeField, TextArea(3, 10)] private string _dialogue;

    [Header("Tiene doblaje?")]
    [SerializeField] private AudioClip _pensamientoVozClip;

    private RaycastHit hit;
    private Vector3 directionToPlayer;
    private bool _triggered = false;

    private ObjectiveReporter objectiveReporter;

    private void Start()
    {
        objectiveReporter = GetComponent<ObjectiveReporter>();
    }

    private void Update()
    {
        if (_triggered) return;
        if (DialogueManager.Instance == null) return;

        Camera cam = Camera.main;
        if (cam == null) return;

        Vector3 raycastOrigin = transform.position + _raycastOffset;

        directionToPlayer = (cam.transform.position - raycastOrigin).normalized;
        float playerToObjectAngle = Vector3.Angle(cam.transform.forward, raycastOrigin - cam.transform.position);

        if (Physics.Raycast(raycastOrigin, directionToPlayer, out hit, _detectionDistance, LayerMask.GetMask("Player", "Obstacle")) && hit.collider.CompareTag("Player") && playerToObjectAngle <= _detectionAngle)
        {
            if (_triggerAction)
            {
                ILookTriggereable lookTriggereable = GetComponent<ILookTriggereable>();
                lookTriggereable?.ExecuteLookTrigger();

                _triggered = lookTriggereable != null ? lookTriggereable.WasTriggered() : true;
            } else
            {
                _triggered = true;
            }

                DialogueManager.Instance.ShowDialogue(_dialogue, _pensamientoVozClip);

            if (objectiveReporter != null) objectiveReporter.ReportObjective();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position + _raycastOffset, directionToPlayer * _detectionDistance);
    }
}