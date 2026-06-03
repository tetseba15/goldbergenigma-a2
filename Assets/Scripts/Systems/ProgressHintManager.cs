using UnityEngine;

public class ProgressHintManager : MonoBehaviour
{
    [SerializeField] private string firstActHint = "Debería revisar el segundo piso.";

    private bool interactedWithOuija = false;
    private bool wentUpstairs = false;

    private void OnEnable()
    {
        ItemPickup.OnInteract += GiveHint;
        OuijaBoard.OnInteract += InteractedWithOuija;
        UpstairsTrigger.OnTrigger += WentUpstairs;
    }

    private void GiveHint(PlayerInventory playerInventory)
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
