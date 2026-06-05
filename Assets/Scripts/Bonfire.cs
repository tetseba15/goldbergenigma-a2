using System;
using UnityEngine;

public class Bonfire : MonoBehaviour, IInteractable
{
    public static event Action OnFireExtinguished;

    [SerializeField] private GameObject _fireEffect;
    [SerializeField, TextArea(2, 5)] private string _noLighterPrompt = "Necesito algo para prender esto.";
    [SerializeField] private float timer = 10f;//para la SCENE WIN

    private bool _isLit = false;
    private bool _unlocked = false;

    public bool IsLit => _isLit;

    private void Update()//agregado scene WIN
    {
        timer -= Time.deltaTime;//Agregado WIN
    }
    private void OnEnable()
    {
        WorkshopExitTrigger.OnPlayerFinalObjective += Unlock;
    }

    private void OnDisable()
    {
        WorkshopExitTrigger.OnPlayerFinalObjective -= Unlock;

    }

    public void Unlock(bool isUnlocked)
    {
        _unlocked = isUnlocked;
    }

    public string GetInteractPrompt(GameObject interactor)
    {
        if (!_unlocked || _isLit) return string.Empty;

        PlayerInventory inventory = interactor.GetComponent<PlayerInventory>();
        if (inventory != null && inventory.HasItem(PlayerInventory.ItemType.Lighter))
            return "[E] Prender fogón";

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

        OnFireExtinguished?.Invoke();

        if (_fireEffect != null)
            _fireEffect.SetActive(false);

        if (timer <= 0)//Agregado SCENE WIN
        {
            GameManager.Instance.Win();
        }

    }
}