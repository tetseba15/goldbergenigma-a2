using UnityEngine;

public class ChangingLight : MonoBehaviour
{
    [Header("Color")]
    [SerializeField] private Color _newColor;
    [SerializeField] private float _changeTime = 2f;

    [Header("Game Event Type")]
    [SerializeField] private GenericEventsTrigger.GameEventType _gameEventType;

    private Color _originalColor;
    private float _time;
    private bool _triggered;

    private void Awake()
    {
        _originalColor = GetComponent<Light>().color;
    }

    private void Update()
    {
        if (_triggered && _time < _changeTime)
        {
            _time += Time.deltaTime;
        }

        if (_time >= _changeTime)
        {
            GetComponent<Light>().color = _originalColor;
            _time = 0;
            _triggered = false;
        }
    }

    private void OnEnable()
    {
        GenericEventsTrigger.OnTriggerEvent += ChangeLightColor;
    }

    private void OnDisable()
    {
        GenericEventsTrigger.OnTriggerEvent -= ChangeLightColor;
    }

    private void ChangeLightColor(GenericEventsTrigger.GameEventType gameEventType)
    {
        if (gameEventType == _gameEventType)
        {
            _triggered = true;
            Light light = GetComponent<Light>();
            if (light != null) light.color = _newColor;
        }
    }
}
