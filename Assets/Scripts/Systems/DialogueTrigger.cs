using Unity.Burst;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private string _dialogue = "Insert Dialogue";
    [SerializeField, Range(1,3)]
    private int _triggerOnAct;

    [SerializeField] private PlayerInventory.ItemType item;

    private void OnTriggerEnter(Collider other)
    {
        PlayerInventory inventory = other.GetComponent<PlayerInventory>();
        if (
            inventory != null &&
            _triggerOnAct == OuijaBoard.Instance.CurrentAct &&
            (item == null || inventory.HasItem(PlayerInventory.ItemType.WorkshopKey))
        )
        {
            DialogueManager.Instance.ShowDialogue(_dialogue);
            gameObject.SetActive(false);
        }
    }
}
