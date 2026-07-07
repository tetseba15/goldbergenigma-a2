using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CrossController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private PlayerInventory _inventory;
    [SerializeField] private GameObject _crossVisual;
    [SerializeField] private Animator _crossAnimator;

    [Header("Audio")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _crossSound;

    [Header("Configuración")]
    [SerializeField] private float _animationDuration;
    [SerializeField] private float _effectDistance;
    [SerializeField] private float _completeCastTime = 1.5f;
    [SerializeField] private float _stunDuration;
    [SerializeField] private float _lightChangeSpeed = 10f;

    [Header("Partículas")]
    [SerializeField] private ParticleSystem _holyParticles;
    [SerializeField] private ParticleSystem _castingParticles;

    private PlayerInputHandler _playerInputHandler;
    private Light _crossLight;

    private bool _isUsing = false;
    private float _castingTime;

    private Coroutine _lightCoroutine;

    public static event Action<PlayerInventory.ItemType> OnCrossUse;

    void Start()
    {
        _playerInputHandler = GetComponent<PlayerInputHandler>();
        _crossLight = _crossVisual.GetComponent<Light>();
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        if (_playerInputHandler != null && _playerInputHandler.IsCastingCross && !_isUsing && _inventory.HasItem(PlayerInventory.ItemType.Cross)) // Start casting
        {
            _castingTime += Time.deltaTime;
            _crossAnimator.SetBool("IsCasting", true);
            if (_castingParticles != null) _castingParticles.Play();
        }
        else if (_castingTime >= _completeCastTime && _playerInputHandler != null &&
            !_playerInputHandler.IsCastingCross &&
            !_isUsing &&
            _inventory.HasItem(PlayerInventory.ItemType.Cross) &&
            FaithController.Instance.ActualFaith >= FaithController.Instance.CrossConsumption) // Use cross
        {
            _castingTime = 0;
            if (_castingParticles != null) _castingParticles.Stop();

            StartCoroutine(UseCrossRoutine());
        }
        else if (_castingTime > 0 && !_playerInputHandler.IsCastingCross) // Cancel casting
        {
            _castingTime = 0;
            _crossAnimator.SetBool("IsCasting", false);
            if (_castingParticles != null) _castingParticles.Stop();
        }

        PlayerMovement playerMovement = transform.root.GetComponent<PlayerMovement>();
        if (_playerInputHandler.IsCastingCross)
        {
            if (playerMovement != null) playerMovement.SpeedMultiplier = 0.5f;
        }
        else
        {
            if (playerMovement != null) playerMovement.SpeedMultiplier = 1f;
        }
    }

    private void OnEnable()
    {
        InteractableDoor.OnUnlocked += UseCross;
    }

    private void OnDisable()
    {
        InteractableDoor.OnUnlocked -= UseCross;
    }

    private void UseCross(PlayerInventory.ItemType itemType)
    {
        if (itemType == PlayerInventory.ItemType.Cross) StartCoroutine(UseCrossRoutine());
    }

    private IEnumerator UseCrossRoutine()
    {
        OnCrossUse?.Invoke(PlayerInventory.ItemType.Cross);

        _isUsing = true;

        if (_audioSource != null && _crossSound != null)
        {
            _audioSource.PlayOneShot(_crossSound);
        }

        if (_crossAnimator != null)
        {
            _crossAnimator.SetBool("IsUsing", true);
            _crossAnimator.SetBool("IsCasting", false);
            _crossAnimator.SetTrigger("Cross");

            if (_holyParticles != null) _holyParticles.Play();

            _lightCoroutine = StartCoroutine(ChangeLightIntensity(8f));
        }

        GameObject enemyObject = GameObject.FindWithTag("Enemy");
        if (enemyObject != null)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemyObject.transform.position);

            if (distanceToEnemy <= _effectDistance)
            {
                EnemyAI enemyAI = enemyObject.GetComponent<EnemyAI>();
                if (enemyAI != null)
                {
                    enemyAI.CrossImpact(_stunDuration); //volver a poner en caso de emergencia
                }
            }
        }

        yield return new WaitForSeconds(_animationDuration);

        _crossAnimator.SetBool("IsUsing", false);
        _isUsing = false;

        if (_holyParticles != null) _holyParticles.Stop();

        if (_lightCoroutine != null) StopCoroutine(_lightCoroutine);
        _lightCoroutine = StartCoroutine(ChangeLightIntensity(0f));
    }

    private IEnumerator ChangeLightIntensity(float intensity)
    {
        if (_crossLight == null) yield break;

        while (true)
        {
            _crossLight.intensity = Mathf.MoveTowards(_crossLight.intensity, intensity, Time.deltaTime * _lightChangeSpeed);

            if (_crossLight.intensity == intensity) break;

            yield return null;
        }
    }
}
