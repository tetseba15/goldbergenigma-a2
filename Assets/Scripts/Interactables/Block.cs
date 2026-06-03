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
        OuijaBoard.OnInteract += ChangePosition;
    }

    private void OnDisable()
    {
        OuijaBoard.OnInteract -= ChangePosition;
    }

    private void ChangePosition(PlayerInventory.ItemType item)
    {
        if (item.Equals(PlayerInventory.ItemType.OuijaBoard) && !positionChanged)
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
