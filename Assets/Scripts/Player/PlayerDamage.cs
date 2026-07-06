using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerDamage : MonoBehaviour
{
    [Header("Volume Settings")]
    [SerializeField] private Volume _damagedVolume;
    [SerializeField] private float _volumeEffectSpeed = 2f;

    [Header("Health Settings")]
    [SerializeField] private int _maxHits = 3;
    [SerializeField] private float _healthRestoreCooldown = 3.5f;

    [Header("Damage SFX")]
    [SerializeField] private List<AudioClip> _gruntClips;
    [SerializeField] private AudioClip _reliefClip;

    private float _percentageIncrement;
    private float _actualWeightTarget;

    private float _successiveHits;
    private float _timeWithNoHit;

    private Coroutine _increaseVolumeCoroutine;
    private Coroutine _restoreVolumeCoroutine;

    public float Health { get; set; } = 1f;

    private void Awake()
    {
        _percentageIncrement = 1f / _maxHits;
    }

    private void Update()
    {
        if (_successiveHits > 0)
        {
            _timeWithNoHit += Time.deltaTime;
            if (_timeWithNoHit >= _healthRestoreCooldown)
            {
                _successiveHits = 0;
                _actualWeightTarget = 0;

                if (_reliefClip != null) AudioManager.Instance.PlaySFX(_reliefClip);

                if (_increaseVolumeCoroutine != null) StopCoroutine( _increaseVolumeCoroutine);
                _restoreVolumeCoroutine = StartCoroutine(RestoreVolume());
            }

            if (_successiveHits > _maxHits) GameManager.Instance.GameOver();
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
        if (_gruntClips.Count > 0)
        {
            AudioManager.Instance.PlaySFX(_gruntClips[Random.Range(0, _gruntClips.Count)]);
        }

        _successiveHits++;
        _timeWithNoHit = 0;
        _actualWeightTarget = Mathf.Min(_actualWeightTarget + _percentageIncrement, 1f);

        if (_increaseVolumeCoroutine != null) StopCoroutine(_increaseVolumeCoroutine);
        if (_restoreVolumeCoroutine != null) StopCoroutine(_restoreVolumeCoroutine);

        _increaseVolumeCoroutine = StartCoroutine(IncreaseVolumeEffect());
    }

    private IEnumerator IncreaseVolumeEffect()
    {
        if (_damagedVolume != null)
        {
            while (_damagedVolume.weight < _actualWeightTarget)
            {
                _damagedVolume.weight = Mathf.MoveTowards(_damagedVolume.weight, _actualWeightTarget, Time.deltaTime * _volumeEffectSpeed);

                yield return null;
            }
        }
    }

    private IEnumerator RestoreVolume()
    {
        if (_damagedVolume != null)
        {
            while (_damagedVolume.weight > 0)
            {
                _damagedVolume.weight = Mathf.MoveTowards(_damagedVolume.weight, _actualWeightTarget, (Time.deltaTime * _volumeEffectSpeed) / 2);

                yield return null;
            }
        }
    }
}
