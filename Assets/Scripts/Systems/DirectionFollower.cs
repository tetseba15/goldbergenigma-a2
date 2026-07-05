using UnityEngine;

public class DirectionFollower : MonoBehaviour
{
    [Header("Rotation")]
    [SerializeField] private Vector3 _rotationOffset;

    [Header("Appear")]
    [SerializeField]
    [Tooltip("Cuanto tiempo se muestra el objeto. Si está en 0 no se destruye.")] private float _showedTime;

    [Header("Game Event Type")]
    [SerializeField] private GenericEventsTrigger.GameEventType _gameEventType;

    private MeshRenderer _meshRenderer;
    private float _time;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        Vector3 directionToPlayer = cam.transform.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(directionToPlayer) * Quaternion.Euler(_rotationOffset);

        transform.rotation = rotation;

        if (_meshRenderer.enabled && _time < _showedTime && _showedTime != 0)
        {
            _time += Time.deltaTime;
        }

        if (_meshRenderer.enabled && _time >= _showedTime) Destroy(gameObject);
    }

    private void OnEnable()
    {
        GenericEventsTrigger.OnTriggerEvent += Appear;
    }

    private void OnDisable()
    {
        GenericEventsTrigger.OnTriggerEvent -= Appear;
    }

    private void Appear(GenericEventsTrigger.GameEventType gameEventType)
    {
        if (gameEventType == _gameEventType)
        {
            if (_meshRenderer != null) _meshRenderer.enabled = true;
        }
    }
}
