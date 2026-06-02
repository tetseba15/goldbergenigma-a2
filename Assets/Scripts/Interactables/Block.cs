using UnityEngine;

public class Block : MonoBehaviour
{
    private AudioSource _audioSource;
    [SerializeField] private AudioClip _fallClip;

    private bool positionChanged = false;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

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
            if (_fallClip != null)
            {
                _audioSource.PlayOneShot(_fallClip);
            }
            positionChanged = true;
            GetComponent<BoxCollider>().enabled = false;

            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(transform.childCount - 1).gameObject.SetActive(true);
        }
    }
}
