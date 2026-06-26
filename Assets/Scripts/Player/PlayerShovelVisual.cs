using UnityEngine;

public class PlayerShovelVisual : MonoBehaviour
{
    [Header("Referencias Visuales")]
    [SerializeField] private GameObject _shovelVisual;
    [SerializeField] private Animator _shovelAnimator;

    private void Start()
    {
        if (_shovelVisual != null) _shovelVisual.SetActive(false);
    }

    public void ShowAndDig()
    {
        if (_shovelVisual != null) _shovelVisual.SetActive(true);
        if (_shovelAnimator != null) _shovelAnimator.SetTrigger("Dig");
    }

    public void HideShovel()
    {
        if (_shovelVisual != null) _shovelVisual.SetActive(false);
    }
}