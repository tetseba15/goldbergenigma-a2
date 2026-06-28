using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class IconNotify : MonoBehaviour
{
    [SerializeField] private int showNumber = 3;
    [SerializeField] private float fadeDuration = 1f;

    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void OnEnable()
    {
        ObjectiveManager.OnObjectiveUpdated += ShowIcon;
    }

    private void OnDisable()
    {
        ObjectiveManager.OnObjectiveUpdated -= ShowIcon;
    }

    private void ShowIcon(string objectiveText)
    {
        StartCoroutine(StartBlink());
    }

    private IEnumerator StartBlink()
    {
        for (int i = 0; i < showNumber; i++)
        {
            yield return Fade(0, 1);
            yield return Fade(1, 0);
        }
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float time = 0f;
        Color color = image.color;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;

            color.a = Mathf.Lerp(startAlpha, endAlpha, time / fadeDuration);
            image.color = color;

            yield return null;
        }

        color.a = endAlpha;
        image.color = color;
    }
}
