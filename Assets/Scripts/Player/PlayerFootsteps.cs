using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerFootsteps : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioSource _footstepSource;
    [SerializeField] private CharacterController _controller; 

    [Header("Audio Clips")]
    [SerializeField] private AudioClip[] _footstepClips;

    [Header("Settings")]
    [SerializeField, Tooltip("Time between steps when walking normally")]
    private float _baseStepInterval = 0.5f;
    [SerializeField, Tooltip("Minimum speed required to trigger footsteps")]
    private float _speedThreshold = 0.1f;
    [SerializeField, Tooltip("Minimum speed required to trigger running interval")]
    private float _runningThreshold = 3f;

    private float _stepTimer = 0f;

    private Vector3 _lastPosition;

    private void Start()
    {
        _lastPosition = transform.position;
    }

    private void Update()
    {
        HandleFootsteps();

        Debug.Log(_controller.velocity.magnitude);
    }

    private void HandleFootsteps()
    {

        ////if (_controller.isGrounded && _controller.velocity.magnitude > _speedThreshold)
        //if (_controller.velocity.magnitude > _speedThreshold)
        ////if (Keyboard.current.lKey.wasPressedThisFrame)
        //{
        //    _stepTimer += Time.deltaTime;

        //    //float currentInterval = isRunning ? _baseStepInterval * 0.7f : _baseStepInterval;

        //    if (_stepTimer >= _baseStepInterval)
        //    {
        //        PlayFootstep();
        //        _stepTimer = 0f;
        //    }
        //}
        //else
        //{
        //    _stepTimer = 0f;
        //}

        Vector3 currentPosition = transform.position;
        Vector3 movement = currentPosition - _lastPosition;

        movement.y = 0f;

        float flatSpeed = movement.magnitude / Time.deltaTime;

        _lastPosition = currentPosition;

        
        if (flatSpeed > _speedThreshold)
        {
            _stepTimer += Time.deltaTime;

            float currentInterval = (flatSpeed > _runningThreshold) ? _baseStepInterval * 0.7f : _baseStepInterval;

            if (_stepTimer >= _baseStepInterval)
            {
                PlayFootstep();
                _stepTimer = 0f;
            }
        }
        else
        {
            _stepTimer = 0f;
        }
    }

    private void PlayFootstep()
    {
        if (_footstepClips == null || _footstepClips.Length == 0) return;

        _footstepSource.pitch = Random.Range(0.9f, 1.1f);

        _footstepSource.volume = Random.Range(0.1f, 0.2f);

        AudioClip randomStep = _footstepClips[Random.Range(0, _footstepClips.Length)];
        _footstepSource.PlayOneShot(randomStep);
    }
}