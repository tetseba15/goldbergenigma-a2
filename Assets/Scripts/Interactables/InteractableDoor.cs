using UnityEngine;


public class InteractableDoor : MonoBehaviour, IInteractable
{
    [Header("References")]
    [SerializeField] private Rigidbody _doorRigidbody;
    [SerializeField] private HingeJoint _hingeJoint;

    [Header("Door Settings")]
    [SerializeField] private bool _isLocked = false;
    [SerializeField] private PlayerInventory.ItemType _requiredKey;

    [Header("RE7 Motor Settings")]
    [SerializeField, Tooltip("Ángulo máximo de apertura")]
    private float _openAngle = 120f;
    [SerializeField, Tooltip("Fuerza del resorte al abrir caminando/botón")]
    private float _walkSpringForce = 15f;
    [SerializeField, Tooltip("Fuerza del resorte al abrir corriendo")]
    private float _sprintSpringForce = 50f;

    [Header("Interaction Prompts")]
    [SerializeField] private string _lockedMessage = "Está cerrada con llave.";
    [SerializeField] private string _unlockedMessage = "Abrir puerta";
    [SerializeField] private string _closedMessage = "Cerrar puerta";

    [Header("Audio")]
    [SerializeField] private AudioSource _doorAudioSource;
    [SerializeField] private AudioClip _lockedRattleSound;
    [SerializeField] private AudioClip _unlockSound;
    [SerializeField] private AudioClip _slamSound;
    [SerializeField] private AudioClip _creakSound;

    [Header("Noise & Stealth")]
    [SerializeField] private float _loudNoiseRadius = 15f;
    [SerializeField] private float _creakNoiseRadius = 2f;

    private JointLimits _originalLimits;

    private bool _isOpen = false;

    private void Awake()
    {
        
        _originalLimits = _hingeJoint.limits;

        _hingeJoint.useSpring = true;

        if (_isLocked) LockDoorPhysically();
    }

    public string GetInteractPrompt(GameObject interactor)
    {
        if (_isLocked)
        {
            PlayerInventory inventory = interactor.GetComponent<PlayerInventory>();
            return (inventory != null && inventory.HasItem(_requiredKey)) ? _unlockedMessage : _lockedMessage;
        }

        return _isOpen ? _closedMessage : _unlockedMessage;
    }

    public void Interact(GameObject interactor)
    {
        if (_isLocked)
        {
            HandleLockedInteraction(interactor);
            return;
        }

        if (_isOpen) CloseDoor();
        else OpenDoor(interactor.transform.position, false); 
    }

    public void PhysicalPush(Vector3 interactorPosition, bool isSprinting)
    {
        if (_isLocked || _isOpen) return; 

        OpenDoor(interactorPosition, isSprinting);
    }

    private void OpenDoor(Vector3 interactorPosition, bool isSprinting)
    {
        _isOpen = true;

        // Push based on player's position
        Vector3 dirToPlayer = (interactorPosition - transform.position).normalized;
        float dot = Vector3.Dot(transform.forward, dirToPlayer);
        float targetAngle = dot > 0 ? -_openAngle : _openAngle;

        JointSpring spring = _hingeJoint.spring;
        spring.targetPosition = targetAngle;
        spring.spring = isSprinting ? _sprintSpringForce : _walkSpringForce;
        spring.damper = 3f; 
        _hingeJoint.spring = spring;

        // SFX
        if (isSprinting)
        {
            if (_doorAudioSource) _doorAudioSource.PlayOneShot(_slamSound);
            NoiseManager.EmitNoise(transform.position, _loudNoiseRadius);
        }
        else
        {
            if (_doorAudioSource) _doorAudioSource.PlayOneShot(_creakSound);
            NoiseManager.EmitNoise(transform.position, _creakNoiseRadius);
        }
    }

    private void CloseDoor()
    {
        _isOpen = false;

        JointSpring spring = _hingeJoint.spring;
        spring.targetPosition = 0f;
        spring.spring = _walkSpringForce; 
        _hingeJoint.spring = spring;

        if (_doorAudioSource) _doorAudioSource.PlayOneShot(_creakSound);
    }

    private void HandleLockedInteraction(GameObject interactor)
    {
        PlayerInventory inventory = interactor.GetComponent<PlayerInventory>();
        if (inventory != null && inventory.HasItem(_requiredKey))
        {
            _isLocked = false;
            _hingeJoint.limits = _originalLimits;
            if (_doorAudioSource) _doorAudioSource.PlayOneShot(_unlockSound);
        }
        else
        {
            if (_doorAudioSource) _doorAudioSource.PlayOneShot(_lockedRattleSound);
            
            _doorRigidbody.AddRelativeTorque(Vector3.up * 5f, ForceMode.Impulse);
        }
    }

    private void LockDoorPhysically()
    {
        JointLimits lockedLimits = _hingeJoint.limits;
        lockedLimits.min = -1f;
        lockedLimits.max = 1f;
        _hingeJoint.limits = lockedLimits;
    }
}