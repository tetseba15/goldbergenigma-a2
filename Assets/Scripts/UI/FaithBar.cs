using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FaithBar : MonoBehaviour
{
    [Header("Faith Settings")]
    [SerializeField] private float _faithConsumption = 0.6f;

    [Header("Slider Settings")]
    [SerializeField] private float _sliderUpdateSpeed = 3f;
    [SerializeField] private float _timeToDissapear = 2f;
    [SerializeField] private float _fadeSpeed = 1.5f;

    private Slider _slider;
    private CanvasGroup _canvasGroup;

    private float _dissapearTime;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        FaithController.OnFaithChange += UpdateFaithBar;
    }

    private void OnDisable()
    {
        FaithController.OnFaithChange -= UpdateFaithBar;
    }

    private void Update()
    {
        if (_canvasGroup != null && _canvasGroup.alpha == 1)
        {
            _dissapearTime += Time.deltaTime;
            if (_dissapearTime >= _timeToDissapear)
            {
                StartCoroutine(UpdateSliderAlpha(0));
                _dissapearTime = 0;
            }
        }
    }

    private void UpdateFaithBar(float actualFaith)
    {
        StartCoroutine(MoveTowardsValue(actualFaith));
        StartCoroutine(UpdateSliderAlpha(1));
        _dissapearTime = 0;
    }

    private IEnumerator UpdateSliderAlpha(float alphaDestiny)
    {
        while (true)
        {
            _canvasGroup.alpha = Mathf.MoveTowards(_canvasGroup.alpha, alphaDestiny, _fadeSpeed * Time.deltaTime);

            if (_canvasGroup.alpha == alphaDestiny) yield break;
            yield return null;
        }
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
