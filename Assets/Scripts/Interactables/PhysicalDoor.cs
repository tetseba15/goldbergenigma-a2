using UnityEngine;
using UnityEngine.UIElements;


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
    [SerializeField, Tooltip("Solo se puede patear si la puerta está abierta menos de estos grados")]
    private float _maxBustAngle = 35f; 

    private float _lastBustTime = 0f;
    private float _bustCooldown = 1f;

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

    public void TryBustOpen(Collider other)
    {
        if (Time.time < _lastBustTime + _bustCooldown) return;

        if (Mathf.Abs(_hingeJoint.angle) > _maxBustAngle) return;

        if (other.CompareTag("Player") && !_isLocked)
        {
            PlayerInputHandler playerInput = other.GetComponent<PlayerInputHandler>();

            if (playerInput != null && playerInput.IsSprinting)
            {
                Vector3 pushDir = other.transform.forward;
                pushDir.y = 0f;
                pushDir.Normalize();

                _doorRb.AddForce(pushDir * _sprintBustForce, ForceMode.Impulse);

                _lastBustTime = Time.time;

                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaySFXAtPosition(_slamSound, transform.position, 0.9f, Random.Range(0.9f, 1.1f));
                    NoiseManager.EmitNoise(transform.position, 15f);
                }
            }
        }
    }
}

