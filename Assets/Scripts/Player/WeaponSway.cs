using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInputHandler _inputHandler;

    [Header("Intertia")]
    [SerializeField, Tooltip("Overdone delay")]
    private float _swayMultiplier = 2f;

    [SerializeField, Tooltip("Center speed")]
    private float _smoothStep = 8f;

    [SerializeField, Tooltip("Limit to avoid breaking the arm")]
    private float _maxSwayAmount = 5f;

    private Quaternion _initialLocalRotation;

    private void Start()
    {
        _initialLocalRotation = transform.localRotation;
    }

    private void Update()
    {
        if (UIManager.Instance != null && UIManager.Instance.IsReadingNote) return;

        HandleSway();
    }

    private void HandleSway()
    {
        Vector2 lookInput = _inputHandler.LookInput;

        // Calculate inverted rotation
        float swayX = Mathf.Clamp(lookInput.x * _swayMultiplier, -_maxSwayAmount, _maxSwayAmount);
        float swayY = Mathf.Clamp(lookInput.y * _swayMultiplier, -_maxSwayAmount, _maxSwayAmount);

        // Substract (-swayX) to generate a drag effect.
        Quaternion targetRotation = Quaternion.Euler(swayY, -swayX, 0f) * _initialLocalRotation;

        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * _smoothStep);
    }
}