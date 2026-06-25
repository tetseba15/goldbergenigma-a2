using System.Collections;
using UnityEngine;

public class Block : MonoBehaviour
{
    [Header("Act progression")]
    [SerializeField]
    [Tooltip("Define en qué acto cambia su posición")]
    private GameFlowManager.Act _actTrigger;

    [Header("Audio")]
    [SerializeField] private AudioClip _fallClip;
    [SerializeField] private float _clipDelay = 2f;
    [SerializeField] private float _maxDistance = 50;

    private bool positionChanged = false;

    private void OnEnable()
    {
        GameFlowManager.OnActChanged += ChangePosition;
    }

    private void OnDisable()
    {
        GameFlowManager.OnActChanged -= ChangePosition;
    }

    private void ChangePosition(GameFlowManager.Act currentAct)
    {
        if (currentAct == _actTrigger && !positionChanged)
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
        AudioManager.Instance.PlaySFXAtPosition(_fallClip, transform.position, 1f, Random.Range(0.8f, 1f), _maxDistance);
    }
}
