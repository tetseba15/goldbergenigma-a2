using UnityEngine;
using UnityEngine.Rendering;

public class PlayerDamage : MonoBehaviour
{
    [Header("Volume Settings")]
    [SerializeField] private Volume _damagedVolume;

    [Header("Health Settings")]
    [SerializeField] private int _maxHits = 3;
    [SerializeField] private float _healthRestoreCooldown = 3.5f;

    private float _successiveHits;
    private float _timeWithNoHit;

    public float Health { get; set; } = 1f;

    private void Update()
    {
        if (_successiveHits > 0)
        {
            _timeWithNoHit += Time.deltaTime;
            if (_timeWithNoHit >= _healthRestoreCooldown) _successiveHits = 0;

            if (_successiveHits == _maxHits) Debug.Log("Die");
        }
    }

    private void OnEnable()
    {
        DangerousZone.OnHitPlayer += GetHit;
    }

    private void OnDisable()
    {
        DangerousZone.OnHitPlayer -= GetHit;
    }

    private void GetHit()
    {
        _successiveHits++;
    }
}
