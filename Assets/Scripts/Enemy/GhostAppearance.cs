using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostAppearance : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _ghostModel;
    [SerializeField] private SkinnedMeshRenderer _meshRenderer;

    [Header("Settings")]
    [SerializeField] private float _appearDuration = 5f;
    [SerializeField] private float _roarDuration = 2f;

    [Header("Sound")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _disappearSound;

    [Header("Trigger positions")]
    [SerializeField] private List<GameObject> _settedPositions;

    [Header("Game event type")]
    [SerializeField] private GenericEventsTrigger.GameEventType _gameEventType;

    private int _positionIndex = 0;

    private Coroutine _disappearCoroutine;

    private void OnEnable()
    {
        GenericEventsTrigger.OnTriggerEvent += Appear;
        OuijaBoard.OnOuijaUse += Appear;
    }

    private void OnDisable()
    {
        GenericEventsTrigger.OnTriggerEvent -= Appear;
        OuijaBoard.OnOuijaUse -= Appear;
    }

    private void Awake()
    {
        if (_meshRenderer == null) return;
        _meshRenderer.enabled = false;

        foreach (GameObject position in _settedPositions)
        {
            position.transform.parent = null;
        }
    }

    private void Appear(GenericEventsTrigger.GameEventType gameEventType)
    {
        if (_positionIndex >= _settedPositions.Count || gameEventType != _gameEventType) return;

        AppearByPosition(_settedPositions[_positionIndex].transform.position);
        _positionIndex++;
    }

    private void Appear(OuijaBoard.OuijaInfo ouijaInfo)
    {
        AppearByPosition(ouijaInfo.Position);
    }

    private void AppearByPosition(Vector3 position)
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

        _meshRenderer.enabled = true;
        EnemyAI.TriggerRoar(_roarDuration, _roarDuration);
        _disappearCoroutine = StartCoroutine(DisappearAfterDelay());
    }

    private IEnumerator DisappearAfterDelay()
    {
        yield return new WaitForSeconds(_appearDuration);
        _meshRenderer.enabled = false;
    }
}