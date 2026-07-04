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
    private float _dragSensitivity = 1f;
    [SerializeField] private string _dragPromptText = "Arrastrar puerta";

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
    //[SerializeField, Tooltip("AudioSource adjunto a la puerta física")]
    //private AudioSource _creakAudioSource;

    [SerializeField, Tooltip("Pitch minimo (lento)")] private float _minPitch = 0.8f;
    [SerializeField, Tooltip("Pitch maximo (rapido)")] private float _maxPitch = 1.3f;
    [SerializeField, Tooltip("Velocidad de bisagra necesaria para el maximo ruido")]
    private float _speedForMaxVolume = 100f;

    private bool _wasClosed = true;

    private AudioSource _borrowedCreakSource;

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

        _doorRb.linearDamping= 2f;
        _doorRb.angularDamping = 5f;

        _originalLimits = _hingeJoint.limits;

        if (_isLocked) LockDoorPhysically();

        //if (_creakAudioSource != null && _creakSound != null)
        //{
        //    _creakAudioSource.clip = _creakSound;
        //    _creakAudioSource.loop = true;
        //    _creakAudioSource.volume = 0f;
        //    _creakAudioSource.spatialBlend = 1f; 
        //    _creakAudioSource.Play();
        //}
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

    public void OnGrabStart(GameObject interactor)
    {
        if (_isLocked)
        {
            HandleLockedInteraction(interactor);
            return;
        }

        _hingeJoint.useSpring = false;

        if (_creakSound != null && AudioManager.Instance != null)
        {
            //AudioManager.Instance.PlaySFXAtPosition(_creakSound, transform.position, 1f, Random.Range(0.95f, 1.05f));
            NoiseManager.EmitNoise(transform.position, _creakNoiseRadius);
        }
    }

    public void OnGrabUpdate(Vector2 mouseDelta)
    {
        if (_isLocked) return;

        float appliedForce = mouseDelta.x * _dragSensitivity;
        _doorRb.AddRelativeTorque(Vector3.up * appliedForce, ForceMode.Force);
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

        float currentSpeed = Mathf.Abs(_hingeJoint.velocity);

        if (currentSpeed > 1f)
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
                    _borrowedCreakSource.Play();
                }
            }

            if (_borrowedCreakSource != null)
            {
                float speedPercent = Mathf.Clamp01(currentSpeed / _speedForMaxVolume);

                float targetVolume = speedPercent > 0.05f ? speedPercent : 0f;
                _borrowedCreakSource.volume = Mathf.Lerp(_borrowedCreakSource.volume, targetVolume, Time.deltaTime * 15f);

                float targetPitch = Mathf.Lerp(_minPitch, _maxPitch, speedPercent);
                _borrowedCreakSource.pitch = Mathf.Lerp(_borrowedCreakSource.pitch, targetPitch, Time.deltaTime * 5f);
            }
        }
        else
        {
            ReturnCreakSourceIfNeeded();
        }
    }

    private void ReturnCreakSourceIfNeeded()
    {
        if (_borrowedCreakSource != null)
        {
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
                AudioManager.Instance.PlaySFXAtPosition(_closeSound, transform.position, 1f, Random.Range(0.95f, 1.05f));
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