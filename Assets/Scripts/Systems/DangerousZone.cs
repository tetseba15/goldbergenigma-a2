using System;
using UnityEngine;

public class DangerousZone : MonoBehaviour, IPurificable
{
    [Header("Zone Settings")]
    [SerializeField] private float _speedReduction = 0.5f;
    [SerializeField] private float _hitCooldown = 1f;
    [SerializeField] private PurificableData _purificationData;

    private float _timer;
    private bool _purified;

    public PurificableData PurificationData => _purificationData;
    public static event Action OnHitPlayer;

    private ObjectiveReporter _objectiveReporter;

    private void Awake()
    {
        _objectiveReporter = GetComponent<ObjectiveReporter>();
    }

    private void OnEnable()
    {
        CrossController.OnCrossUse += Purify;
    }

    private void OnDisable()
    {
        CrossController.OnCrossUse -= Purify;
    }

    private void OnTriggerStay(Collider other)
    {
        PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();

        if (_purified)
        {
            if (playerMovement != null) playerMovement.ChangeSpeedMultiplier(gameObject, 1f);
            return;
        }

        if (other.CompareTag("Player"))
        {
            if (playerMovement != null) playerMovement.ChangeSpeedMultiplier(gameObject, _speedReduction);

            _timer += Time.deltaTime;
            if (_timer >= _hitCooldown)
            {
                OnHitPlayer?.Invoke();
                _timer = 0f;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
            if (playerMovement != null) playerMovement.ChangeSpeedMultiplier(gameObject, 1f);

            _timer = 0;
        }
    }

    public void Purify(PlayerInventory.ItemType itemType, Vector3 purifySourcePosition)
    {
        if (_purified) return;

        float distance = Vector3.Distance(transform.position, purifySourcePosition);

        if (distance <= _purificationData._purificationDistance && itemType == _purificationData._purificationItem)
        {
            _purified = true;

            if (_objectiveReporter != null)
            {
                _objectiveReporter.ReportObjective();
            }
        }
    }
}
