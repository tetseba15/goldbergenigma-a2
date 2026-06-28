using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Dialogue")]
    [SerializeField] private string _dialogue = "Insert Dialogue";

    [Header("Conditions")]
    [SerializeField] private GameFlowManager.Act _triggerOnAct;
    [SerializeField] private bool _requiresItem = false;
    [SerializeField] private PlayerInventory.ItemType _requiredItem;

    private void OnTriggerEnter(Collider other)
    {
        PlayerInventory inventory = other.GetComponent<PlayerInventory>();
        if (inventory == null || GameFlowManager.Instance.CurrentAct != _triggerOnAct) return;
        if (_requiresItem && !inventory.HasItem(_requiredItem)) return;

        DialogueManager.Instance.ShowDialogue(_dialogue);
        gameObject.SetActive(false);
    }
}