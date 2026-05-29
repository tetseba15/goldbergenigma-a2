using UnityEngine;
using System.Collections;

public class GhostAppearance : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _ghostModel;

    [Header("Settings")]
    [SerializeField] private float _appearDuration = 5f;
    [SerializeField] private float _roarDuration = 2f;

    private Coroutine _disappearCoroutine;

    public void Appear()
    {
        if (_ghostModel == null) return;

        if (_disappearCoroutine != null)
            StopCoroutine(_disappearCoroutine);

        _ghostModel.SetActive(true);
        EnemyAI.TriggerRoar(_roarDuration, _roarDuration);
        _disappearCoroutine = StartCoroutine(DisappearAfterDelay());
    }

    private IEnumerator DisappearAfterDelay()
    {
        yield return new WaitForSeconds(_appearDuration);
        _ghostModel.SetActive(false);
    }
}