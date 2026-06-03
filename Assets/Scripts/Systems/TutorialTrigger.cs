using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    [Header("Configuración del Tutorial")]
    [SerializeField, TextArea(2, 4)] private string _tutorialMessage;
    [SerializeField] private float _durationOnScreen = 5f;
    [SerializeField] private bool _triggerOnlyOnce = true;

    private bool _hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (_hasTriggered && _triggerOnlyOnce) return;

        if (other.CompareTag("Player")) 
        {
            TutorialManager.Instance.ShowTutorial(_tutorialMessage, _durationOnScreen);

            _hasTriggered = true;
        }
    }
}