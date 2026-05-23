using System;
using UnityEngine;

[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerInteractor : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera _mainCamera;

    [Header("Interaction Config")]
    [field: SerializeField, Tooltip("Max distance for interaction")]
    public float InteractionDistance { get; private set; } = 2.5f;

    [SerializeField, Tooltip("Layer that contains interactuable objects")]
    private LayerMask _interactableMask;

    private PlayerInputHandler _inputHandler;
    private IInteractable _currentInteractable;

    private void Awake()
    {
        _inputHandler = GetComponent<PlayerInputHandler>();
    }

    private void Update()
    {
        HandleRaycast();
        HandleInteraction();
    }
    private void HandleRaycast()
    {
        //Emit a raycast from the center of the camera
        Ray ray = new Ray(_mainCamera.transform.position, _mainCamera.transform.forward);

        RaycastHit hitInfo;


        if (Physics.Raycast(ray, out hitInfo, InteractionDistance, _interactableMask))
        {
            IInteractable interactable = hitInfo.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                if(_currentInteractable != interactable)
                {
                    _currentInteractable = interactable;

                    //CALL UIMANAGER TO SHOW THE TEXT
                    UIManager.Instance.ShowInteractPrompt(_currentInteractable.GetInteractPrompt(gameObject));
                }

                return;

            }
        }

        if(_currentInteractable != null)
        {
            _currentInteractable = null;
            //HIDE TEXT
            UIManager.Instance.HideInteractPrompt();
        }
    }

    private void HandleInteraction()
    {
        if(_inputHandler.IsInteracting && _currentInteractable != null)
        {
            _currentInteractable.Interact(this.gameObject);


            //Avoid multiple interactions while pressing the button 
            _inputHandler.ConsumeInteractInput();
        }
    }
}
