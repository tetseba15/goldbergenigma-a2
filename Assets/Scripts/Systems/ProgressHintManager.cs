using UnityEngine;

public class ProgressHintManager : MonoBehaviour
{
    [SerializeField] private string firstActHint = "Debería revisar el segundo piso.";
    [SerializeField] private string secondActHint = "Debería usar la ouija.";

    [SerializeField] private float secondActHintDelay = 20f;

    private bool interactedWithOuija = false;
    private bool wentUpstairs = false;

    private void OnEnable()
    {
        ItemPickup.OnInteract += GiveHint;
        OuijaBoard.OnInteract += InteractedWithOuija;
        UpstairsTrigger.OnTrigger += WentUpstairs;
    }

    private void GiveHint(PlayerInventory playerInventory, PlayerInventory.ItemType itemType)
    {
        if (
            playerInventory.HasItem(PlayerInventory.ItemType.BathroomKey) &&
            playerInventory.HasItem(PlayerInventory.ItemType.Bottle) &&
            interactedWithOuija &&
            !wentUpstairs
        )
        {
            DialogueManager.Instance.ShowDialogue(firstActHint);
        }

        if (
            itemType == PlayerInventory.ItemType.Cross
        )
        {
            DialogueManager.Instance.ShowDialogueWithDelay(secondActHint, secondActHintDelay);
        }
    }

    private void InteractedWithOuija(PlayerInventory.ItemType item)
    {
        if (item.Equals(PlayerInventory.ItemType.OuijaBoard))
        {
            interactedWithOuija = true;
        }
    }

    private void WentUpstairs()
    {
        wentUpstairs = true;
    }
}
