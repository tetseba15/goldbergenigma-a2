using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class MenuButtonAnimator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Referencias Opcionales")]
    [SerializeField, Tooltip("Si lo dejas vacío, lo buscará automáticamente en los hijos")]
    private TextMeshProUGUI _buttonText;

    [Header("Configuración de Audio")]
    [SerializeField] private AudioClip _hoverSound;
    [SerializeField] private AudioClip _clickSound;

    private string _originalText;

    private void Awake()
    {
        if (_buttonText == null)
        {
            _buttonText = GetComponentInChildren<TextMeshProUGUI>();
        }

        if (_buttonText != null)
        {
            _originalText = _buttonText.text;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_buttonText != null)
        {
            _buttonText.text = "> " + _originalText;
        }

        if (_hoverSound != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(_hoverSound, 0.4f); 
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_buttonText != null)
        {
            _buttonText.text = _originalText;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_clickSound != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(_clickSound, 1f); 
        }
    }
}