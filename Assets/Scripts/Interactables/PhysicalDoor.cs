using UnityEngine;


public class PhysicalDoor : MonoBehaviour, IPhysicsInteractable
{
    [Header("Referencias Físicas")]
    [SerializeField] private Rigidbody _doorRb;
    [SerializeField] private HingeJoint _hingeJoint;

    [Header("Configuración de Arrastre")]
    [SerializeField, Tooltip("Qué tan sensible es la puerta al mover el mouse")]
    private float _dragSensitivity = 50f;
    [SerializeField] private string _promptText = "Mantener presionado para arrastrar";

    [Header("Configuración de Sprint (Bust Open)")]
    [SerializeField] private float _sprintBustForce = 150f;

    [Header("Audio")]
    [SerializeField] private AudioClip _lockedRattleSound;
    [SerializeField] private AudioClip _unlockSound;
    [SerializeField] private AudioClip _slamSound;
    [SerializeField] private AudioClip _creakSound;

    // Variables para la lógica de llaves que puedes copiar después de tu puerta original
    private bool _isLocked = false;

    private void Awake()
    {
        if (_doorRb == null) _doorRb = GetComponent<Rigidbody>();
        if (_hingeJoint == null) _hingeJoint = GetComponent<HingeJoint>();

        // Hacemos que la puerta sea ligeramente pesada para que no vuele con el mouse
        _doorRb.linearDamping = 2f;
        _doorRb.angularDamping = 5f;
    }

    // --- IPhysicsInteractable Implementation ---

    public string GetInteractPrompt(GameObject interactor)
    {
        return _isLocked ? "Cerrado con llave" : _promptText;
    }

    public void OnGrabStart(GameObject interactor)
    {
        if (_isLocked)
        {
            // Reproducir sonido de manija trabada
            return;
        }

        // Opcional: Detener temporalmente el HingeJoint Spring si quieres que sea libre
        _hingeJoint.useSpring = false;
    }

    public void OnGrabUpdate(Vector2 mouseDelta)
    {
        if (_isLocked) return;

        // Convertimos el movimiento horizontal (o vertical) del ratón en fuerza de rotación.
        // Eje Y (Vector3.up) es la bisagra de la puerta. Multiplicamos por la sensibilidad.
        float appliedForce = mouseDelta.x * _dragSensitivity;

        _doorRb.AddRelativeTorque(Vector3.up * appliedForce, ForceMode.Force);
    }

    public void OnGrabEnd()
    {
        // Al soltarla, puedes volver a activar el resorte suave para que se cierre sola 
        // o dejarla completamente suelta (useSpring = false)
    }

    // --- Sprint logic ---

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !_isLocked)
        {
            PlayerInputHandler playerInput = collision.gameObject.GetComponent<PlayerInputHandler>();

            if (playerInput != null && playerInput.IsSprinting)
            {
                Vector3 pushDir = collision.contacts[0].normal * -1f;
                _doorRb.AddForceAtPosition(pushDir * _sprintBustForce, collision.contacts[0].point, ForceMode.Impulse);

                AudioManager.Instance.PlaySFXAtPosition(_slamSound, transform.position, .45f);
            }
        }
    }
}