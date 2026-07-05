using System;
using UnityEngine;

public class DangerousZone : MonoBehaviour
{
    [Header("Zone Settings")]
    [SerializeField] private float _speedReduction = 0.5f;
    [SerializeField] private float _hitCooldown = 1f;

    private float _timer;

    public static event Action OnHitPlayer;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
            if (playerMovement != null) playerMovement.SpeedMultiplier = _speedReduction;

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
            if (playerMovement != null) playerMovement.SpeedMultiplier = 1f;

            _timer = 0;
        }
    }
}
