using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class InspectableBoook : MonoBehaviour, IInteractable
{
    [Header("Configuración de Cámara")]
    [SerializeField] private Transform _cameraTargetPoint;
    [SerializeField] private float _transitionDuration = 1.0f;

    [Header("Descipción texto")]
    [SerializeField, Tooltip("Text canvas")]
    private TextMeshProUGUI _uiTextComponent;

    [SerializeField, TextArea(3, 10), Tooltip("Texto")]
    private string _inspectionText;

    [Header("Dialogue")]
    [SerializeField, TextArea(3, 10), Tooltip("Dialogo del player al inspeccionar")]
    private string _playerDialogue;

    private Camera _mainCamera;
    private Vector3 _originalCamPosition;
    private Quaternion _originalCamRotation;
    private Transform _originalCamParent;

    private bool _isInspecting = false;
    private bool _isTransitioning = false;

    private PlayerMovement _playerMovement;
    private PlayerLook _playerLook;
    private List<MonoBehaviour> _disabledCameraScripts = new List<MonoBehaviour>();

    void Start()
    {
        _mainCamera = Camera.main;

        if (_uiTextComponent != null)
        {
            _uiTextComponent.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (_isInspecting && !_isTransitioning && Keyboard.current.eKey.wasPressedThisFrame)
        {
            StopAllCoroutines();
            StartCoroutine(ExitInspectionRoutine());
        }
    }

    public string GetInteractPrompt(GameObject player)
    {
        return _isInspecting ? string.Empty : "Rituales para la Ascensión: poder y riqueza. [E] Inspeccionar";
    }

    public void Interact(GameObject player)
    {
        if (_isTransitioning) return;

        if (!_isInspecting)
        {
            StopAllCoroutines();
            StartCoroutine(EnterInspectionRoutine(player));
        }
    }

    private IEnumerator EnterInspectionRoutine(GameObject player)
    {
        _isTransitioning = true;
        _isInspecting = true;

        _originalCamPosition = _mainCamera.transform.localPosition;
        _originalCamRotation = _mainCamera.transform.localRotation;
        _originalCamParent = _mainCamera.transform.parent;

        _playerMovement = player.GetComponent<PlayerMovement>();
        if (_playerMovement != null) _playerMovement.enabled = false;

        _playerLook = player.GetComponent<PlayerLook>();
        if (_playerLook != null) _playerLook.enabled = false;

        _disabledCameraScripts.Clear();
        MonoBehaviour[] allCamScripts = _mainCamera.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in allCamScripts)
        {
            if (script != null && script.enabled)
            {
                script.enabled = false;
                _disabledCameraScripts.Add(script);
            }
        }

        _mainCamera.transform.SetParent(null);

        float elapsedTime = 0f;
        Vector3 startingPos = _mainCamera.transform.position;
        Quaternion startingRot = _mainCamera.transform.rotation;

        while (elapsedTime < _transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / _transitionDuration;
            t = t * t * (3f - 2f * t);

            _mainCamera.transform.position = Vector3.Lerp(startingPos, _cameraTargetPoint.position, t);
            _mainCamera.transform.rotation = Quaternion.Lerp(startingRot, _cameraTargetPoint.rotation, t);
            yield return null;
        }

        _mainCamera.transform.position = _cameraTargetPoint.position;
        _mainCamera.transform.rotation = _cameraTargetPoint.rotation;

        if (_uiTextComponent != null && !string.IsNullOrEmpty(_inspectionText))
        {
            _uiTextComponent.text = _inspectionText;
            _uiTextComponent.gameObject.SetActive(true);
        }

        if (DialogueManager.Instance != null && !string.IsNullOrEmpty(_playerDialogue))
        {
            DialogueManager.Instance.ShowDialogue(_playerDialogue);
        }

        _isTransitioning = false;
    }

    private IEnumerator ExitInspectionRoutine()
    {
        _isTransitioning = true;

        if (_uiTextComponent != null)
        {
            _uiTextComponent.gameObject.SetActive(false);
        }

        float elapsedTime = 0f;
        Vector3 startingPos = _mainCamera.transform.position;
        Quaternion startingRot = _mainCamera.transform.rotation;

        Vector3 targetGlobalPos = _originalCamParent.TransformPoint(_originalCamPosition);
        Quaternion targetGlobalRot = _originalCamParent.rotation * _originalCamRotation;

        while (elapsedTime < _transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / _transitionDuration;
            t = t * t * (3f - 2f * t);

            _mainCamera.transform.position = Vector3.Lerp(startingPos, targetGlobalPos, t);
            _mainCamera.transform.rotation = Quaternion.Lerp(startingRot, targetGlobalRot, t);
            yield return null;
        }

        _mainCamera.transform.SetParent(_originalCamParent);
        _mainCamera.transform.localPosition = _originalCamPosition;
        _mainCamera.transform.localRotation = _originalCamRotation;

        if (_playerMovement != null) _playerMovement.enabled = true;
        if (_playerLook != null) _playerLook.enabled = true;

        foreach (MonoBehaviour script in _disabledCameraScripts)
        {
            if (script != null) script.enabled = true;
        }
        _disabledCameraScripts.Clear();

        _isInspecting = false;
        _isTransitioning = false;
    }
}