using UnityEngine;
using System.Collections;

public class GhostAppearance : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _ghostModel;

    [Header("Settings")]
    [SerializeField] private float _appearDuration = 5f;
    [SerializeField] private float _roarDuration = 2f;

    [Header("Sound")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _disappearSound;
    
    private Coroutine _disappearCoroutine;

    public void Appear()
    {
        Appear(_ghostModel.transform.position);
    }

    public void Appear(Vector3 position)
    {
        if (_ghostModel == null) return;
        if (_disappearCoroutine != null)
            StopCoroutine(_disappearCoroutine);

        _ghostModel.transform.position = position;

        // Mirar hacia el jugador
        Vector3 directionToCamera = Camera.main.transform.position - position;
        directionToCamera.y = 0f;
        if (directionToCamera != Vector3.zero)
            _ghostModel.transform.rotation = Quaternion.LookRotation(directionToCamera);

        _ghostModel.SetActive(true);
        EnemyAI.TriggerRoar(_roarDuration, _roarDuration);
        _disappearCoroutine = StartCoroutine(DisappearAfterDelay());
    }

    private IEnumerator DisappearAfterDelay()
    {
        yield return new WaitForSeconds(_appearDuration);
        _ghostModel.SetActive(false);

        OuijaBoard ouija = FindObjectOfType<OuijaBoard>();
        if (ouija != null) ouija.ResetCooldown();
    }
}