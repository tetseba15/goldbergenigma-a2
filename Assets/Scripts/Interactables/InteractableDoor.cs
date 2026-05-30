using UnityEngine;


public class InteractableDoor : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private bool _isLocked = false;
    [SerializeField] private PlayerInventory.ItemType _requiredKey;
    [SerializeField, Tooltip("Fuerza al abrirla con el botón")]
    private float _gentlePushForce = 2f;

    [Header("Audio")]
    [SerializeField] private AudioSource _doorAudioSource;
    [SerializeField] private AudioClip _lockedRattleSound;
    [SerializeField] private AudioClip _unlockSound;
    [SerializeField] private AudioClip _slamSound;
    [SerializeField] private AudioClip _creakSound; 

    [Header("Noise & Stealth")]
    [SerializeField, Tooltip("Velocidad angular mínima para considerar un portazo")]
    private float _slamThreshold = 2.5f;
    [SerializeField] private float _loudNoiseRadius = 15f;
    [SerializeField] private float _creakNoiseRadius = 2f;

    [Header("References")]
    [SerializeField]private Rigidbody _doorRigidbody;
    [SerializeField]private HingeJoint _hingeJoint;

    private float _lastCreakTime = 0f;
    private float _creakCooldown = 1.5f;

    private JointLimits _originalLimits;
    private bool _hasSlammed = false;

    private void Awake()
    {
        

        _originalLimits = _hingeJoint.limits;

        if (_isLocked)
        {
            LockDoorPhysically();
        }
    }

    private void Update()
    {
        HandleAudioAndNoise();
    }

    
    public void Interact(GameObject interactor)
    {
        if (_isLocked)
        {
            PlayerInventory inventory = interactor.GetComponent<PlayerInventory>();
            if (inventory != null && inventory.HasItem(_requiredKey))
            {
                UnlockDoor();
            }
            else
            {
                RattleLockedDoor();
            }
        }
        else
        {
            _doorRigidbody.AddRelativeTorque(Vector3.up * _gentlePushForce, ForceMode.Impulse);
        }
    }

    private void LockDoorPhysically()
    {
        JointLimits lockedLimits = _hingeJoint.limits;
        lockedLimits.min = 0f;
        lockedLimits.max = 0f;
        _hingeJoint.limits = lockedLimits;
    }

    private void UnlockDoor()
    {
        _isLocked = false;

        _hingeJoint.limits = _originalLimits;

        if (_doorAudioSource && _unlockSound)
            _doorAudioSource.PlayOneShot(_unlockSound);
    }

    private void RattleLockedDoor()
    {
        if (_doorAudioSource && _lockedRattleSound)
            _doorAudioSource.PlayOneShot(_lockedRattleSound);

        _doorRigidbody.AddRelativeTorque(Vector3.up * (_gentlePushForce * 2f), ForceMode.Impulse);
    }

    private void HandleAudioAndNoise()
    {
        if (_isLocked) return;

        float doorSpeed = _doorRigidbody.angularVelocity.magnitude;

        if (doorSpeed > _slamThreshold && !_hasSlammed)
        {
            NoiseManager.EmitNoise(transform.position, _loudNoiseRadius);

            if (_doorAudioSource && _slamSound)
            {
                _doorAudioSource.pitch = Random.Range(0.9f, 1.1f);
                _doorAudioSource.PlayOneShot(_slamSound);
            }

            _hasSlammed = true; 
        }
        else if (doorSpeed > 0.1f && doorSpeed <= _slamThreshold)
        {
            if (Time.time >= _lastCreakTime + _creakCooldown)
            {
                if (_doorAudioSource && _creakSound)
                {
                    _doorAudioSource.pitch = Random.Range(0.8f, 1.2f);
                    _doorAudioSource.PlayOneShot(_creakSound);
                    NoiseManager.EmitNoise(transform.position, _creakNoiseRadius);
                }
                _lastCreakTime = Time.time;
            }
        }

        if (doorSpeed < 0.1f)
        {
            _hasSlammed = false;
        }
    }
}