using UnityEngine;

public class GhostSneakyLook : MonoBehaviour, ILookTriggereable
{
    [Header("Position")]
    [SerializeField] private Vector3 _destinyPosition;
    [SerializeField] private Vector3 _destinyRotation;
    [SerializeField] private float _duration;
    [SerializeField]
    [Tooltip("Distancia máxima antes de desaparecer")] private float _dissapearDistance = 2f;

    [Header("Game Event")]
    [SerializeField] private GenericEventsTrigger.GameEventType _gameEventType;

    [Header("Mesh Renderer")]
    [SerializeField] private SkinnedMeshRenderer _meshRenderer;


    private bool _triggered;
    private float time;

    private void Update()
    {
        if (_triggered && time < _duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / _duration);

            transform.SetPositionAndRotation(
                Vector3.Lerp(transform.position, _destinyPosition, t),
                Quaternion.Lerp(transform.rotation, Quaternion.Euler(_destinyRotation), t)
            );
        }

        if (time >= _duration) Destroy(gameObject);
        
        float distanceWithPlayer = Vector3.Distance(Camera.main.transform.position, transform.position);
        if (distanceWithPlayer < _dissapearDistance && _meshRenderer.enabled) Destroy(gameObject);
    }

    private void OnEnable()
    {
        GenericEventsTrigger.OnTriggerEvent += Sneak;
    }

    private void OnDisable()
    {
        GenericEventsTrigger.OnTriggerEvent -= Sneak;
    }

    private void Sneak(GenericEventsTrigger.GameEventType gameEventType)
    {
        if (gameEventType == _gameEventType && _meshRenderer != null) _meshRenderer.enabled = true;
    }

    public void ExecuteLookTrigger()
    {
        if (_meshRenderer.enabled) _triggered = true;
    }

    public bool WasTriggered()
    {
        return _triggered;
    }
}
