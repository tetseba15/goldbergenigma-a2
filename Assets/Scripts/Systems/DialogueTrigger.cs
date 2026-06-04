using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private string _dialogue = "Insert Dialogue";
    [SerializeField, Range(1,3)]
    private int triggerOnAct;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && triggerOnAct == OuijaBoard.Instance.CurrentAct)
        {
            DialogueManager.Instance.ShowDialogue(_dialogue);
            gameObject.SetActive(false);
        }
    }
}
