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

    private PlayerInputHandler _playerInputHandler;
    private bool _isUsing = false;
    private float _castingTime;

    public static event Action<PlayerInventory.ItemType> OnCrossUse;

    void Start()
    {
        //if (_crossVisual != null)
        //{
        //    _crossVisual.SetActive(false);
        //}

        _playerInputHandler = GetComponent<PlayerInputHandler>();
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        if (_playerInputHandler != null && _playerInputHandler.IsCastingCross && !_isUsing && _inventory.HasItem(PlayerInventory.ItemType.Cross)) // Start casting
        {
            _castingTime += Time.deltaTime;
            _crossAnimator.SetBool("IsCasting", true);
        }
        else if (_castingTime >= _completeCastTime && _playerInputHandler != null && !_playerInputHandler.IsCastingCross && !_isUsing && _inventory.HasItem(PlayerInventory.ItemType.Cross)) // Use cross
        {
            _castingTime = 0;
            StartCoroutine(UseCrossRoutine());
        }
        else if (_castingTime > 0 && !_playerInputHandler.IsCastingCross) // Cancel casting
        {
            _castingTime = 0;
            _crossAnimator.SetBool("IsCasting", false);
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
        //_crossVisual.SetActive(true);

        if (_audioSource != null && _crossSound != null)
        {
            _audioSource.PlayOneShot(_crossSound);
        }

        if (_crossAnimator != null)
        {
            _crossAnimator.SetBool("IsUsing", true);
            _crossAnimator.SetBool("IsCasting", false);
            _crossAnimator.SetTrigger("Cross");
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
        //_crossVisual.SetActive(false);

        _crossAnimator.SetBool("IsUsing", false);
        _isUsing = false;
    }
}
