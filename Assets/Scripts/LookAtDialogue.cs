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

    private void Update()
    {
        if (_triggered) return;
        if (DialogueManager.Instance == null) return;

        Camera cam = Camera.main;
        if (cam == null) return;

        directionToPlayer = (cam.transform.position - transform.position).normalized;
        float distance = Vector3.Distance(cam.transform.position, transform.position);
        //float angle = Vector3.Angle(cam.transform.forward, directionToPlayer);

        if (Physics.Raycast(transform.position, directionToPlayer, out hit, _detectionDistance, LayerMask.GetMask("Player", "Obstacle")) && hit.collider.CompareTag("Player"))
        {
            _triggered = true;
            DialogueManager.Instance.ShowDialogue(_dialogue, _pensamientoVozClip);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, directionToPlayer * _detectionDistance);
    }
}