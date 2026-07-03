using UnityEngine;

public class LookAtDialogue : MonoBehaviour
{
    [SerializeField, TextArea(3, 10)] private string _dialogue;
    [SerializeField] private float _detectionDistance = 15f;
    [SerializeField] private float _detectionAngle = 15f;
    private bool _triggered = false;

    [Header("Tiene doblaje?")]
    [SerializeField] private AudioClip _pensamientoVozClip;

    private RaycastHit hit;
    private Vector3 directionToPlayer;

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

        directionToPlayer = (cam.transform.position - transform.position).normalized;
        float angle = Vector3.Angle(cam.transform.forward, transform.position - cam.transform.position);

        if (gameObject.name == "Monalisa") Debug.Log("Angulo: " + angle);

        if (Physics.Raycast(transform.position, directionToPlayer, out hit, _detectionDistance, LayerMask.GetMask("Player", "Obstacle")) && hit.collider.CompareTag("Player") && angle <= _detectionAngle)
        {
            _triggered = true;
            DialogueManager.Instance.ShowDialogue(_dialogue, _pensamientoVozClip);

            if (objectiveReporter != null) objectiveReporter.ReportObjective();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, directionToPlayer * _detectionDistance);
    }
}