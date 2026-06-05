using UnityEngine;

public class Bonfire : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject _fireEffect;
    [SerializeField, TextArea(2, 5)] private string _noLighterPrompt = "Necesito algo para prender esto.";

    private bool _isLit = false;
    private bool _unlocked = false;

    public bool IsLit => _isLit;

    public void Unlock()
    {
        _unlocked = true;
    }

    public string GetInteractPrompt(GameObject interactor)
    {
        if (!_unlocked || _isLit) return string.Empty;

        PlayerInventory inventory = interactor.GetComponent<PlayerInventory>();
        if (inventory != null && inventory.HasItem(PlayerInventory.ItemType.Lighter))
            return "Prender fogón";

        return _noLighterPrompt;
    }

    public void Interact(GameObject interactor)
    {
        if (!_unlocked || _isLit) return;

        PlayerInventory inventory = interactor.GetComponent<PlayerInventory>();
        if (inventory == null || !inventory.HasItem(PlayerInventory.ItemType.Lighter)) return;

        _isLit = true;

        if (_fireEffect != null)
            _fireEffect.SetActive(true);
    }

    public void Extinguish()
    {
        _isLit = false;
        if (_fireEffect != null)
            _fireEffect.SetActive(false);
    }
}