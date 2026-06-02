using UnityEngine;

public class Block : MonoBehaviour
{
    private bool positionChanged = false;

    private void OnEnable()
    {
        ItemPickup.OnInteract += ChangePosition;
    }

    private void OnDisable()
    {
        ItemPickup.OnInteract -= ChangePosition;
    }

    private void ChangePosition(PlayerInventory.ItemType item)
    {
        if (item.Equals(PlayerInventory.ItemType.Bottle) && !positionChanged)
        {
            positionChanged = true;
            GetComponent<BoxCollider>().enabled = false;

            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(transform.childCount - 1).gameObject.SetActive(true);
        }
    }
}
