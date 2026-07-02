using UnityEngine;

[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerPhysicsGrabber : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Camera _mainCamera;

    [Header("Configuración")]
    [SerializeField] private float _reachDistance = 2.5f;
    [SerializeField] private LayerMask _interactableMask;

    private PlayerInputHandler _inputHandler;
    private IPhysicsInteractable _currentGrabbedObject;
    private IPhysicsInteractable _hoveredObject; 
    private string _lastPromptMessage = string.Empty;

    private void Awake()
    {
        _inputHandler = GetComponent<PlayerInputHandler>();
    }

    private void Update()
    {
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
            _lastPromptMessage = string.Empty;
            if (UIManager.Instance != null) UIManager.Instance.HideInteractPrompt();
        }
    }

    private void HandleGrabbing()
    {
        // Start grab
        if (_inputHandler.IsPhysicsGrabbing && _currentGrabbedObject == null && _hoveredObject != null)
        {
            _currentGrabbedObject = _hoveredObject;
            _currentGrabbedObject.OnGrabStart(gameObject);

            // Opcional: Ocultar el prompt de texto mientras arrastramos
            if (UIManager.Instance != null) UIManager.Instance.HideInteractPrompt();
        }

        // hold grab
        else if (_inputHandler.IsPhysicsGrabbing && _currentGrabbedObject != null)
        {
            _currentGrabbedObject.OnGrabUpdate(_inputHandler.LookInput);
        }

        // release
        else if (!_inputHandler.IsPhysicsGrabbing && _currentGrabbedObject != null)
        {
            _currentGrabbedObject.OnGrabEnd();
            _currentGrabbedObject = null;
        }
    }
}