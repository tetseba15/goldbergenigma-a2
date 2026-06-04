using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using TMPro;

public class InspectableObject : MonoBehaviour, IInteractable
{
    [Header("1. TEXTO DEL CARTELITO (HUD)")]
    [SerializeField, Tooltip("Lo que dice la pantalla al mirar el objeto")]
    private string _interactPromptText = "Presiona E para examinar";

    [Header("2. TEXTO DE DESCRIPCIÓN")]
    [SerializeField, TextArea(3, 10), Tooltip("Descripción del objeto")]
    private string _inspectionText;

    [Header("3. DIÁLOGO DEL JUGADOR")]
    [SerializeField, TextArea(3, 10), Tooltip("Lo que dice el protagonista")]
    private string _playerDialogue;

    [Header("Configuración del Canvas de UI")]
    [SerializeField, Tooltip("Arrastra aquí el componente de texto de tu HUD Canvas (TextMeshPro)")]
    private TextMeshProUGUI _uiTextComponent;

    [Header("Configuración de Cámara")]
    [SerializeField, Tooltip("El objeto vacío que marca hacia dónde viaja la cámara")]
    private Transform _cameraTargetPoint;
    [SerializeField] private float _transitionDuration = 1.0f;

    private Camera _mainCamera;
    private Vector3 _originalCamPosition;
    private Quaternion _originalCamRotation;
    private Transform _originalCamParent;

    private bool _isInspecting = false;
    private bool _isTransitioning = false;

    private PlayerMovement _playerMovement;
    private PlayerLook _playerLook;
    private List<MonoBehaviour> _disabledCameraScripts = new List<MonoBehaviour>();

    
    private List<EnemyAI> _frozenAIs = new List<EnemyAI>();
    private List<NavMeshAgent> _frozenAgents = new List<NavMeshAgent>();
    private List<Animator> _frozenAnimators = new List<Animator>();

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
        return _isInspecting ? string.Empty : _interactPromptText;
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

        
        _frozenAIs.Clear();
        _frozenAgents.Clear();
        _frozenAnimators.Clear();

        EnemyAI[] enemiesInScene = FindObjectsByType<EnemyAI>(FindObjectsSortMode.None);
        foreach (EnemyAI enemy in enemiesInScene)
        {
            if (enemy != null)
            {
                
                if (enemy.enabled)
                {
                    enemy.enabled = false;
                    _frozenAIs.Add(enemy);
                }

                
                if (enemy.TryGetComponent(out NavMeshAgent agent))
                {
                    if (agent.enabled)
                    {
                        agent.enabled = false; 
                        _frozenAgents.Add(agent);
                    }
                }

                
                if (enemy.TryGetComponent(out Animator anim))
                {
                    anim.speed = 0f; 
                    _frozenAnimators.Add(anim);
                }
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

        
        foreach (NavMeshAgent agent in _frozenAgents)
        {
            if (agent != null) agent.enabled = true;
        }
        _frozenAgents.Clear();

        
        foreach (Animator anim in _frozenAnimators)
        {
            if (anim != null) anim.speed = 1f;
        }
        _frozenAnimators.Clear();

        
        foreach (EnemyAI enemy in _frozenAIs)
        {
            if (enemy != null) enemy.enabled = true;
        }
        _frozenAIs.Clear();

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