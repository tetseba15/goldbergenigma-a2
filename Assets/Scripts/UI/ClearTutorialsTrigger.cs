using UnityEngine;

public class ClearTutorialsTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TutorialManager.Instance.ClearAllTutorials();
        }
    }
}
