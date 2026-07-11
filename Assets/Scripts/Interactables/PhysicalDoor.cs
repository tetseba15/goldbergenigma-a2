using UnityEngine;

public class PhysicalDoor : MonoBehaviour, IPhysicsInteractable
{
    // --- EVENTS ---
    public event System.Action OnNarrativeLockHit;
    public static event System.Action<PlayerInventory.ItemType> OnUnlocked;

    [Header("Referencias Físicas")]
    [SerializeField] private Rigidbody _doorRb;
    [SerializeField] private HingeJoint _hingeJoint;

    private JointLimits _originalLimits;

    [Header("Configuración de Arrastre")]
    [SerializeField, Tooltip("Sensibilidad al mover el mouse")]
    private float _dragSensitivity = 50f;
    [SerializeField, Tooltip("Límite de lectura del ratón para evitar fuerzas extremas por DPI altos")]
    private float _maxMouseDelta = 5f;
    [SerializeField, Tooltip("Límite absoluto de velocidad física de giro (Rad/s)")]
    private float _maxRotationSpeed = 3f;
    [SerializeField] private string _dragPromptText = "Arrastrar puerta";

    // ------ calculations data --------

    private Camera _doorCamera;
    private Vector3 _localGrabPoint;
    private float _leverageMultiplier = 1f;

    private Transform _interactorTransform;

    [Header("Configuración de Sprint y Eventos")]
    [SerializeField] private float _sprintBustForce = 150f;
    [SerializeField] private float _maxBustAngle = 35f;
    [SerializeField, Tooltip("Fuerza del resorte usada EXCLUSIVAMENTE para el portazo cinemático")]
    private float _slamSpringForce = 50f;

    [Header("Sistema de Llaves")]
    [SerializeField] private bool _isLocked = false;
    [SerializeField] private PlayerInventory.ItemType _requiredKey;
    private bool _permanentlyLocked = false;

    [Header("Textos de Interacción")]
    [SerializeField] private string _lockedMessage = "Está cerrada con llave.";
    [SerializeField] private string _unlockPromptMessage = "Desbloquear puerta";

    [Header("Audio y Ruido")]
    [SerializeField] private AudioClip _lockedRattleSound;
    [SerializeField] private AudioClip _unlockSound;
    [SerializeField] private AudioClip _slamSound;
    [SerializeField] private AudioClip _creakSound;
    [SerializeField] private AudioClip _closeSound;
    [SerializeField] private float _loudNoiseRadius = 15f;
    [SerializeField] private float _creakNoiseRadius = 2f;

    [Header("Audio Continuo (Motor de Crujido)")]

    [SerializeField, Tooltip("Pitch minimo (lento)")] private float _minPitch = 0.8f;
    [SerializeField, Tooltip("Pitch maximo (rapido)")] private float _maxPitch = 1.3f;
    [SerializeField, Tooltip("Velocidad de bisagra necesaria para el maximo ruido")]
    private float _speedForMaxVolume = 100f;

    private bool _wasClosed = true;

    private AudioSource _borrowedCreakSource;

    private float _savedAudioTime = 0f;

    // --- TIMERS ---
    private float _lastBustTime = 0f;
    private float _bustCooldown = 1f;
    private float _lastRattleTime = 0f;
    private float _rattleCooldown = 1f;





    private void OnDisable()
    {
        ReturnCreakSourceIfNeeded();
    }

    private void Awake()
    {
        if (_doorRb == null) _doorRb = GetComponent<Rigidbody>();
        if (_hingeJoint == null) _hingeJoint = GetComponent<HingeJoint>();

        _doorRb.linearDamping = 2f;
        _doorRb.angularDamping = 5f;

        _doorRb.maxAngularVelocity = _maxRotationSpeed;

        _originalLimits = _hingeJoint.limits;

        if (_isLocked) LockDoorPhysically();


    }

    private void Update()
    {
        if (_doorRb.IsSleeping())
        {
            ReturnCreakSourceIfNeeded();
            CheckIfClosed();
            return;
        }

        HandleContinuousCreak();
        CheckIfClosed();
    }

    // ==========================================
    // INTERFACE
    // ==========================================

    public string GetInteractPrompt(GameObject interactor)
    {
        if (_isLocked)
        {
            if (_permanentlyLocked) return _lockedMessage;

            PlayerInventory inventory = interactor.GetComponent<PlayerInventory>();
            return (inventory != null && inventory.HasItem(_requiredKey)) ? _unlockPromptMessage : _lockedMessage;
        }
        return _dragPromptText;
    }

    public void OnGrabStart(GameObject interactor, Vector3 grabPoint, Camera playerCamera)
    {
        if (_isLocked)
        {
            HandleLockedInteraction(interactor);
            return;
        }

        _hingeJoint.useSpring = false;
        _doorCamera = playerCamera;

        // Guardamos el punto local para que rote con la puerta
        _localGrabPoint = _doorRb.transform.InverseTransformPoint(grabPoint);

        // Mantenemos el apalancamiento (Para que sea fácil empujar de la manija y difícil desde la bisagra)
        Vector3 hingePos = transform.position; hingePos.y = 0;
        Vector3 clickPos = grabPoint; clickPos.y = 0;
        _leverageMultiplier = 1f + (Vector3.Distance(hingePos, clickPos) * 2f);

        if (_creakSound != null && AudioManager.Instance != null && _borrowedCreakSource == null)
        {
            NoiseManager.EmitNoise(transform.position, _creakNoiseRadius);
        }
    }


    public void OnGrabUpdate(Vector2 mouseDelta)
    {
        if (_isLocked || _doorCamera == null) return;

        // 1. Obtenemos el punto 3D exacto donde está agarrada la puerta AHORA mismo
        Vector3 grabPosWorld = _doorRb.transform.TransformPoint(_localGrabPoint);
        Vector3 hingePos = transform.position;

        // 2. Calculamos el vector del radio (desde la bisagra al agarre)
        Vector3 radius = grabPosWorld - hingePos;
        radius.y = 0;

        // Prevención de errores si haces clic en el centro atómico de la bisagra
        if (radius.sqrMagnitude < 0.001f) return;

        // 3. Calculamos la TANGENTE (La dirección 3D hacia donde giraría la puerta en sentido Horario)
        Vector3 tangent3D = Vector3.Cross(transform.up, radius).normalized;

        // 4. PROYECCIÓN A PANTALLA (La verdadera magia)
        Vector3 screenPos1 = _doorCamera.WorldToScreenPoint(grabPosWorld);
        // Sumamos un pedacito de la tangente para ver hacia dónde se mueve en la pantalla
        Vector3 screenPos2 = _doorCamera.WorldToScreenPoint(grabPosWorld + (tangent3D * 0.1f));

        Vector2 screenTangent = (screenPos2 - screenPos1);
        if (screenTangent.sqrMagnitude > 0.0001f) screenTangent.Normalize();
        else screenTangent = Vector2.zero;

        // 5. Comparamos el ratón con la trayectoria en pantalla
        // Si mueves el ratón visualmente hacia donde va la tangente, da positivo. Si no, negativo.
        float forceAmount = Vector2.Dot(mouseDelta, screenTangent);

        // Recorte por seguridad contra movimientos bruscos de DPI
        float clampedForce = Mathf.Clamp(forceAmount, -_maxMouseDelta, _maxMouseDelta);

        // 6. Aplicamos el Torque
        // Moverse a favor de la tangente (+force) = rotación Horaria (-Y)
        Vector3 finalTorque = -transform.up * clampedForce * _dragSensitivity * _leverageMultiplier;

        // IMPORTANTE: Cambiamos AddRelativeTorque por AddTorque porque el cálculo ya es global
        _doorRb.AddTorque(finalTorque, ForceMode.Force);
    }

    public void OnGrabEnd()
    {
        // La puerta queda suelta (física pura) al soltar el clic
    }

    // ==========================================
    // Audio
    // ==========================================

    private void HandleContinuousCreak()
    {
        if (_isLocked) return;

        float currentVelocity = _hingeJoint.velocity;
        float absSpeed = Mathf.Abs(currentVelocity);

        if (absSpeed > 1f)
        {
            if (_borrowedCreakSource == null)
            {
                _borrowedCreakSource = AudioManager.Instance.BorrowAudioSource();

                if (_borrowedCreakSource != null)
                {
                    _borrowedCreakSource.transform.position = transform.position;
                    _borrowedCreakSource.transform.SetParent(transform);
                    _borrowedCreakSource.clip = _creakSound;

                    _borrowedCreakSource.loop = true;
                    _borrowedCreakSource.spatialBlend = 1f;
                    _borrowedCreakSource.volume = 0f;

                    _borrowedCreakSource.time = _savedAudioTime;

                    _borrowedCreakSource.Play();
                }
            }

            if (_borrowedCreakSource != null)
            {
                float speedPercent = Mathf.Clamp01(absSpeed / _speedForMaxVolume);

                float targetVolume = speedPercent > 0.05f ? speedPercent : 0f;
                _borrowedCreakSource.volume = Mathf.Lerp(_borrowedCreakSource.volume, targetVolume, Time.deltaTime * 15f);

                float targetPitch = Mathf.Lerp(_minPitch, _maxPitch, speedPercent);


                if (currentVelocity < 0)
                {
                    targetPitch *= -1f;
                }

                _borrowedCreakSource.pitch = Mathf.Lerp(_borrowedCreakSource.pitch, targetPitch, Time.deltaTime * 10f);
            }
        }
        else
        {
            if (_borrowedCreakSource != null)
            {
                _borrowedCreakSource.volume = Mathf.Lerp(_borrowedCreakSource.volume, 0f, Time.deltaTime * 15f);

                if (_borrowedCreakSource.volume <= 0.01f)
                {
                    ReturnCreakSourceIfNeeded();
                }
            }
        }
    }


    private void ReturnCreakSourceIfNeeded()
    {
        if (_borrowedCreakSource != null)
        {
            _savedAudioTime = _borrowedCreakSource.time;

            AudioManager.Instance.ReturnAudioSource(_borrowedCreakSource);
            _borrowedCreakSource = null;
        }
    }

    private void CheckIfClosed()
    {
        bool isCurrentlyClosed = Mathf.Abs(_hingeJoint.angle) < 1.5f;

        if (isCurrentlyClosed && !_wasClosed)
        {
            if (_closeSound != null && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFXAtPosition(_closeSound, transform.position, 0.5f, Random.Range(0.95f, 1.05f));
            }
        }

        _wasClosed = isCurrentlyClosed;
    }

    // ==========================================
    // Bust Open Relay
    // ==========================================

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

                if (_slamSound != null && AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaySFXAtPosition(_slamSound, transform.position, 1f, Random.Range(0.9f, 1.1f));
                    NoiseManager.EmitNoise(transform.position, _loudNoiseRadius);
                }
            }
        }
    }

    // ==========================================
    // KEYS
    // ==========================================

    private void HandleLockedInteraction(GameObject interactor)
    {
        if (_permanentlyLocked)
        {
            RattleLockedDoor();
            OnNarrativeLockHit?.Invoke();
            return;
        }

        PlayerInventory inventory = interactor.GetComponent<PlayerInventory>();
        if (inventory != null && inventory.HasItem(_requiredKey))
        {
            _isLocked = false;
            _hingeJoint.limits = _originalLimits;

            if (_unlockSound != null && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFXAtPosition(_unlockSound, transform.position, 1f, 1f);
            }

            OnUnlocked?.Invoke(_requiredKey);
        }
        else
        {
            RattleLockedDoor();
        }
    }

    private void RattleLockedDoor()
    {
        if (Time.time >= _lastRattleTime + _rattleCooldown)
        {
            if (_lockedRattleSound != null && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFXAtPosition(_lockedRattleSound, transform.position, 1f, Random.Range(0.95f, 1.05f));
            }

            _doorRb.AddRelativeTorque(Vector3.up * 5f, ForceMode.Impulse);
            _lastRattleTime = Time.time;
        }
    }

    private void LockDoorPhysically()
    {
        JointLimits lockedLimits = _hingeJoint.limits;
        lockedLimits.min = -1f;
        lockedLimits.max = 1f;
        _hingeJoint.limits = lockedLimits;
    }

    // ==========================================
    // NARRATIVE
    // ==========================================

    public void ApplyNarrativeLock()
    {
        _permanentlyLocked = true;
        _isLocked = true;
        LockDoorPhysically();
    }

    public void RemoveNarrativeLock()
    {
        _permanentlyLocked = false;
        //_isLocked = false; 
        _hingeJoint.limits = _originalLimits;
    }

    /// <summary>
    /// Usado por el EntryDoorEventManager para el portazo asustadizo.
    /// </summary>
    public void ForceSlamShutAndLock()
    {
        _hingeJoint.useSpring = true;

        JointSpring spring = _hingeJoint.spring;
        spring.targetPosition = 0f;
        spring.spring = _slamSpringForce * 1.5f;
        spring.damper = 1f;
        _hingeJoint.spring = spring;

        if (_slamSound != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFXAtPosition(_slamSound, transform.position, 1.2f, Random.Range(0.85f, 0.95f), 20);
        }

        ApplyNarrativeLock();
    }
}