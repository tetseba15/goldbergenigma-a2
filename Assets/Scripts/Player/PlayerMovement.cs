using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(PlayerInputHandler))]
public class PlayerMovement : MonoBehaviour
{
    private CharacterController _controller;
    private PlayerInputHandler _inputHandler;

    [Header("Movement Config")]
    [field: SerializeField, Tooltip("Base Speed")]
    public float WalkSpeed { get; private set; } = 3f;

    [field: SerializeField, Tooltip("Sprint speed")]
    public float SprintSpeed { get; private set; } = 6f;

    public float SpeedMultiplier { get; set; } = 1.0f;

    [Header("Noise Settings")]
    [SerializeField] private float _walkNoiseVolume = 5f;
    [SerializeField] private float _sprintNoiseVolume = 15f;
    [SerializeField] private float _stepInterval = 0.5f;

    private float _stepTimer;

    [Header("Physics Config")]
    [SerializeField] private float gravity = -9.81f;
    private Vector3 _velocity; 
    private bool _isGrounded;

    [Header("Animación")]
    [SerializeField] private Animator _animator;
    [SerializeField, Tooltip("Transition Speed")]
    private float _animationBlendSpeed = 10f;

    private float _currentAnimationSpeed;

    public float CurrentSpeed => _currentAnimationSpeed;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _inputHandler = GetComponent<PlayerInputHandler>();

    }


    void Update()
    {
        if (UIManager.Instance != null && UIManager.Instance.IsReadingNote)
        {
            
            UpdateAnimator(0f);
            return;
        }

        HandleMovement();
        HandleGravity();
    }


    private void HandleMovement()
    {
        Vector2 input = _inputHandler.MoveInput;
        
        Vector3 moveDirection = transform.right * input.x + transform.forward * input.y;

        float targetSpeed = 0f;

        if (input.magnitude > 0.1f)
        {
            targetSpeed = _inputHandler.IsSprinting ? SprintSpeed : WalkSpeed;
            _controller.Move(moveDirection * targetSpeed * SpeedMultiplier * Time.deltaTime);

            HandleFootstepNoise();

            // Footsteps SFX
        }
        else
        {
            _stepTimer = _stepInterval;
        }

        UpdateAnimator(targetSpeed);

    }

    private void HandleFootstepNoise()
    {
        _stepTimer -= Time.deltaTime;

        if (_stepTimer <= 0f)
        {
            float currentVolume = _inputHandler.IsSprinting ? _sprintNoiseVolume : _walkNoiseVolume;

            NoiseManager.EmitNoise(transform.position, currentVolume);

            // adjust interval if running 
            _stepTimer = _inputHandler.IsSprinting ? _stepInterval * 0.6f : _stepInterval;
        }
    }

    private void UpdateAnimator(float targetSpeed)
    {
        if (_animator == null) return;

        
        _currentAnimationSpeed = Mathf.Lerp(_currentAnimationSpeed, targetSpeed, Time.deltaTime * _animationBlendSpeed);

        _animator.SetFloat("Speed", _currentAnimationSpeed);
    }

    public void TriggerDeath()
    {
        if (_animator != null)
        {
            _animator.SetTrigger("Die");
            this.enabled = false;
        }
    }

    private void HandleGravity()
    {
        _isGrounded = _controller.isGrounded;

        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2;
        }

        _velocity.y += gravity * Time.deltaTime;

        _controller.Move(_velocity * Time.deltaTime);

    }
}
