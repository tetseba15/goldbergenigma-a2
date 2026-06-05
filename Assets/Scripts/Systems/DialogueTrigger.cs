using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private string _dialogue = "Insert Dialogue";
    [SerializeField, Range(1, 3)]
    private int _triggerOnAct;
    [SerializeField] private bool _requiresItem = false;
    [SerializeField] private PlayerInventory.ItemType _requiredItem;

    private void OnTriggerEnter(Collider other)
    {
        PlayerInventory inventory = other.GetComponent<PlayerInventory>();
        if (inventory == null) return;
        if (_triggerOnAct != OuijaBoard.Instance.CurrentAct) return;
        if (_requiresItem && !inventory.HasItem(_requiredItem)) return;

        DialogueManager.Instance.ShowDialogue(_dialogue);
        gameObject.SetActive(false);
    }
}