using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FaithBar : MonoBehaviour
{
    [Header("Faith Settings")]
    [SerializeField] private float _faithConsumption = 0.6f;

    [Header("Slider Settings")]
    [SerializeField] private float _sliderUpdateSpeed = 3f;

    private Slider _slider;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
    }

    private void OnEnable()
    {
        FaithController.OnFaithChange += UpdateFaithBar;
    }

    private void OnDisable()
    {
        FaithController.OnFaithChange -= UpdateFaithBar;
    }

    private void UpdateFaithBar(float actualFaith)
    {
        StartCoroutine(MoveTowardsValue(actualFaith));
    }

    private IEnumerator MoveTowardsValue(float actualFaith)
    {
        while (true)
        {
            _slider.value = Mathf.MoveTowards(_slider.value, actualFaith, _sliderUpdateSpeed * Time.deltaTime);

            if (_slider.value == actualFaith) yield break;

            yield return null;
        }
    }
}
