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

    private string _lastPromptMessage = string.Empty;

    private void Awake()
    {
        _inputHandler = GetComponent<PlayerInputHandler>();
    }

    private void Update()
    {
        if (UIManager.Instance != null && UIManager.Instance.IsReadingNote)
        {
            return;
        }

        HandleRaycast();
        HandleInteraction();
    }

    private void HandleRaycast()
    {
        Ray ray = new Ray(_mainCamera.transform.position, _mainCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, InteractionDistance, _interactableMask))
        {
            IInteractable interactable = hitInfo.collider.GetComponentInParent<IInteractable>();

            if (interactable != null)
            {
                if (_currentInteractable != interactable)
                {
                    _currentInteractable = interactable;
                }

                string currentPrompt = _currentInteractable.GetInteractPrompt(gameObject);

                if (_lastPromptMessage != currentPrompt)
                {
                    UIManager.Instance.ShowInteractPrompt(currentPrompt);
                    _lastPromptMessage = currentPrompt;
                }

                return;
            }
        }

        if (_currentInteractable != null)
        {
            _currentInteractable = null;
            _lastPromptMessage = string.Empty; 
            UIManager.Instance.HideInteractPrompt();
        }
    }

    private void HandleInteraction()
    {
        if (_inputHandler.IsInteracting && _currentInteractable != null)
        {
            _currentInteractable.Interact(this.gameObject);

            // Avoid multiple interactions while pressing the button 
            _inputHandler.ConsumeInteractInput();
        }
    }
}