using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueTest : MonoBehaviour
{
    private void Update()
    {
        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            DialogueManager.Instance.ShowDialogue("Prueba de subtítulo");
        }
    }
}