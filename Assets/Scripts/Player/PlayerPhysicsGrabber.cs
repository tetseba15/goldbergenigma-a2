using UnityEngine;

[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerPhysicsGrabber : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Camera _mainCamera;

    [Header("Configuración de Agarre Base")]
    [SerializeField, Tooltip("Distancia máxima para iniciar el agarre")]
    private float _reachDistance = 2.5f;
    [SerializeField] private LayerMask _interactableMask;

    [Header("Seguros (Rompe-Agarre Corregidos)")]
    [SerializeField, Tooltip("Distancia máxima antes de que el brazo virtual se estire demasiado y suelte")]
    private float _breakDistance = 6.0f; 

    [SerializeField, Tooltip("Ángulo máximo de visión lateral antes de soltar (Permite mirar de reojo de forma muy cómoda)")]
    private float _breakAngle = 80f; 

    private PlayerInputHandler _inputHandler;

    private IPhysicsInteractable _currentGrabbedObject;
    private IPhysicsInteractable _hoveredObject;

    private Transform _grabbedTransform;
    private Transform _hoveredTransform;
    private Vector3 _exactHitPoint;

    private Vector3 _grabPointLocal;

    private string _lastPromptMessage = string.Empty;
    private bool _requireNewClick = false;


    

    private void Awake()
    {
        _inputHandler = GetComponent<PlayerInputHandler>();
    }

    private void Update()
    {
        if (!_inputHandler.IsPhysicsGrabbing)
        {
            _requireNewClick = false;
        }

        HandleHover();
        HandleGrabbing();
    }

    private void HandleHover()
    {
        if (_currentGrabbedObject != null) return;

        Ray ray = new Ray(_mainCamera.transform.position, _mainCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, _reachDistance, _interactableMask))
        {
            IPhysicsInteractable interactable = hitInfo.collider.GetComponentInParent<IPhysicsInteractable>();

            if (interactable != null)
            {
                _hoveredObject = interactable;
                _hoveredTransform = hitInfo.collider.transform;
                _exactHitPoint = hitInfo.point;

                string currentPrompt = interactable.GetInteractPrompt(gameObject);

                if (_lastPromptMessage != currentPrompt && UIManager.Instance != null)
                {
                    UIManager.Instance.ShowInteractPrompt(currentPrompt);
                    _lastPromptMessage = currentPrompt;
                }
                return;
            }
        }

        if (_hoveredObject != null)
        {
            _hoveredObject = null;
            _hoveredTransform = null;
            _lastPromptMessage = string.Empty;
            if (UIManager.Instance != null) UIManager.Instance.HideInteractPrompt();
        }
    }

    private void HandleGrabbing()
    {
        // Grab
        if (_inputHandler.IsPhysicsGrabbing && !_requireNewClick && _currentGrabbedObject == null && _hoveredObject != null)
        {
            _currentGrabbedObject = _hoveredObject;
            _grabbedTransform = _hoveredTransform;
            
            _currentGrabbedObject.OnGrabStart(gameObject, _exactHitPoint, _mainCamera); 
            
            if (UIManager.Instance != null) UIManager.Instance.HideInteractPrompt();
        }

        // Hold
        else if (_inputHandler.IsPhysicsGrabbing && _currentGrabbedObject != null)
        {
            if (ShouldBreakGrab())
            {
                ForceRelease();
                return;
            }

            _currentGrabbedObject.OnGrabUpdate(_inputHandler.RawLookInput);
        }

        // Release
        else if (!_inputHandler.IsPhysicsGrabbing && _currentGrabbedObject != null)
        {
            ForceRelease();
        }
    }

    private bool ShouldBreakGrab()
    {
        if (_grabbedTransform == null) return true;

        Vector3 currentHitPointWorld = _grabbedTransform.TransformPoint(_grabPointLocal);

        float distance = Vector3.Distance(_mainCamera.transform.position, currentHitPointWorld);
        Vector3 directionToObject = (currentHitPointWorld - _mainCamera.transform.position).normalized;
        float angle = Vector3.Angle(_mainCamera.transform.forward, directionToObject);

        if (distance < 1.5f)
        {
            angle = 0f; 
        }

        return distance > _breakDistance || angle > _breakAngle;
    }

    private void ForceRelease()
    {
        if (_currentGrabbedObject != null)
        {
            _currentGrabbedObject.OnGrabEnd();
            _currentGrabbedObject = null;
            _grabbedTransform = null;
            _requireNewClick = true;
        }
    }
}