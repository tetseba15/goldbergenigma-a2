using UnityEngine;

public class PlayerPhysicsGrabber : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private PlayerInputHandler _inputHandler;
    [SerializeField] private Camera _mainCamera;

    [Header("Configuración")]
    [SerializeField] private float _reachDistance = 2.5f;
    [SerializeField] private LayerMask _interactableLayer;

    private IPhysicsInteractable _currentGrabbedObject;

    private void Update()
    {
        if (_inputHandler.IsInteracting && _currentGrabbedObject == null)
        {
            TryGrabObject();
        }

        else if (_inputHandler.IsInteracting && _currentGrabbedObject != null)
        {
            _currentGrabbedObject.OnGrabUpdate(_inputHandler.LookInput);
        }

        else if (!_inputHandler.IsInteracting && _currentGrabbedObject != null)
        {
            _currentGrabbedObject.OnGrabEnd();
            _currentGrabbedObject = null;
        }
    }

    private void TryGrabObject()
    {
        Ray ray = _mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, _reachDistance, _interactableLayer))
        {
            IPhysicsInteractable interactable = hit.collider.GetComponent<IPhysicsInteractable>();
            if (interactable != null)
            {
                _currentGrabbedObject = interactable;
                _currentGrabbedObject.OnGrabStart(gameObject);
            }
        }
    }
}