using UnityEngine;

public class ActBlocker : MonoBehaviour
{
    [SerializeField] private GameFlowManager.Act _unlocksAtAct;

    private void OnEnable() => GameFlowManager.OnActChanged += HandleActChange;
    private void OnDisable() => GameFlowManager.OnActChanged -= HandleActChange;

    private void HandleActChange(GameFlowManager.Act newAct)
    {
        if (newAct == _unlocksAtAct)
        {
            gameObject.SetActive(false); 
        }
    }
}