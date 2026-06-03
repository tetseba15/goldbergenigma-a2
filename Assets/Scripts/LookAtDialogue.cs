using UnityEngine;

public class LookAtDialogue : MonoBehaviour
{
    [SerializeField, TextArea(3, 10)] private string _dialogue;
    [SerializeField] private float _detectionDistance = 15f;
    [SerializeField] private float _detectionAngle = 15f;
    private bool _triggered = false;

    private void Update()
    {
        if (_triggered) return;
        if (DialogueManager.Instance == null) return;

        Camera cam = Camera.main;
        if (cam == null) return;

        Vector3 directionToObject = (transform.position - cam.transform.position).normalized;
        float distance = Vector3.Distance(cam.transform.position, transform.position);
        float angle = Vector3.Angle(cam.transform.forward, directionToObject);

        if (distance <= _detectionDistance && angle <= _detectionAngle)
        {
            _triggered = true;
            DialogueManager.Instance.ShowDialogue(_dialogue);
        }
    }
}