using System.Collections;
using UnityEngine;

public class Block : MonoBehaviour
{
    [SerializeField] private AudioClip _fallClip;
    [SerializeField] private float _clipDelay = 2f;

    private AudioSource audioSource;
    private bool positionChanged = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
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
                StartCoroutine(PlayClip());
            }
            positionChanged = true;
            GetComponent<BoxCollider>().enabled = false;

            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(transform.childCount - 1).gameObject.SetActive(true);
        }
    }

    private IEnumerator PlayClip()
    {
        yield return new WaitForSeconds(_clipDelay);
        audioSource.PlayOneShot(_fallClip);
    }
}
