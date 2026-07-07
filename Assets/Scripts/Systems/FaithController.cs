using System;
using UnityEngine;

public class FaithController : MonoBehaviour
{
    [Header("Faith Values")]
    [SerializeField] private float _faithReload = 0.06f;
    [SerializeField] private float _reloadTime = 2f;

    public static FaithController Instance { get; private set; }

    public float CrossConsumption { get; private set; } = 0.6f;
    public float ActualFaith { get; private set; } = 1f;

    private float _maxFaith = 1f;
    private float _time;

    public static event Action<float> OnFaithChange;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (ActualFaith < _maxFaith)
        {
            _time += Time.deltaTime;
            if (_time >= _reloadTime)
            {
                ActualFaith = ActualFaith = Math.Clamp(ActualFaith + _faithReload, 0f, 1f);
                _time = 0;

                OnFaithChange?.Invoke(ActualFaith);
            }
        }
    }

    private void OnEnable()
    {
        CrossController.OnCrossUse += DecreaseFaith;
    }

    private void OnDisable()
    {
        CrossController.OnCrossUse -= DecreaseFaith;
    }

    private void DecreaseFaith(PlayerInventory.ItemType itemType)
    {
        if (itemType == PlayerInventory.ItemType.Cross) ActualFaith = Math.Clamp(ActualFaith - CrossConsumption, 0f, 1f);

        OnFaithChange?.Invoke(ActualFaith);
    }
}
